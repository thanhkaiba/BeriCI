#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System;

using Unity.EditorCoroutines.Editor;
using Piratera.Constance;

// Output the build size or a failure depending on BuildPlayer.

namespace Piratera.Build
{
    public class MyBuildProcessor : Editor
    {
        public static string BuildFolder = $"{Directory.GetCurrentDirectory()}/Build";
        public static string InnoSetupFile = $"{BuildFolder}/Installer/piratera_installer_l2cpp.iss";

        [MenuItem("Builds/Reveal In Finder", false, 0)]
        public static void OpenBuildFolder()
        {
            EditorUtility.RevealInFinder(BuildFolder);
        }

        [MenuItem("Builds/Window/Build", false, 0)]
        public static void BuildWindow()
        {
            WindowBuildVersion.Instance.UpdateVersionNumber();
            string path = $"{BuildFolder}/l2cpp";
            if (path.Length != 0)
            {

                if (File.Exists($"{path}/Piratera_BackUpThisFolder_ButDontShipItWithYourGame"))
                {
                    File.Delete($"{path}/Piratera_BackUpThisFolder_ButDontShipItWithYourGame");
                }

                if (File.Exists($"{path}/Piratera_Data"))
                {
                    File.Delete($"{path}/Piratera_Data");
                }

                OnPreprocessBuild(WindowBuildVersion.OnPreprocessBuild);
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
                buildPlayerOptions.locationPathName = path + "/Piratera.exe";
                buildPlayerOptions.target = BuildTarget.StandaloneWindows;
                buildPlayerOptions.options = BuildOptions.None;


                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                BuildSummary summary = report.summary;

                if (summary.result == BuildResult.Succeeded)
                {
                    UnityEngine.Debug.Log("Build Window succeeded: " + summary.totalSize + " bytes");
                    EditorUtility.RevealInFinder(buildPlayerOptions.locationPathName);
                    BuildInnoSetup(WindowBuildVersion.Instance.CurrentVersion);
                }

                if (summary.result == BuildResult.Failed)
                {
                    UnityEngine.Debug.Log("Build Window failed");
                }

            }
        }

        public static void BuildInnoSetup( string version)
        {
            Process process = new Process();
            Process p = new Process();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"D:/Program Files/Inno Setup 6/iscc.exe";
            psi.Arguments = $" /DMyAppVersion={version} " + InnoSetupFile;

            p.StartInfo = psi;
            p.Start();

            p.WaitForExit();
        }

       

        [MenuItem("Builds/Android/Build", false, 0)]
        public static void AndroidBuild()
        {
            AndroidBuildVersion.Instance.UpdateVersionNumber();

            string path = $"{BuildFolder}/Android";
            if (path.Length > 0)
            {
                OnPreprocessBuild(AndroidBuildVersion.OnPreprocessBuild);
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
                buildPlayerOptions.locationPathName = $"{BuildFolder}/Android" + $"/Piratera v{AndroidBuildVersion.Instance.CurrentVersion}.apk";
                buildPlayerOptions.target = BuildTarget.Android;
                buildPlayerOptions.options = BuildOptions.None;


                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                BuildSummary summary = report.summary;

                if (summary.result == BuildResult.Succeeded)
                {
                    UnityEngine.Debug.Log("Build Android succeeded: " + summary.totalSize + " bytes");
                    EditorUtility.RevealInFinder(buildPlayerOptions.locationPathName);

                }

                if (summary.result == BuildResult.Failed)
                {
                    UnityEngine.Debug.Log("Build Android failed");
                }
            }
        }

        public static void OnPreprocessBuild(Action increaseAction)
        {
            bool shouldIncrement = EditorUtility.DisplayDialog(
                "Increment Version?",
                $"Current: {PlayerSettings.bundleVersion}",
                "Yes",
                "No"
            );

            if (shouldIncrement)
            {
                increaseAction();
            }
        }

   
        [MenuItem("Builds/Window/Upload", false, 1)]
        public static void SyncWindowVersion()
        {
            SyncGameVersion.Sync(BuildType.WINDOW_DEV, WindowBuildVersion.Instance.CurrentVersion);
        }

        [MenuItem("Builds/Android/Upload", false, 1)]
        public static void SyncAndroidVersion()
        {
            SyncGameVersion.Sync(BuildType.ANDROID_DEV, AndroidBuildVersion.Instance.CurrentVersion);
        }
    }
}
#endif