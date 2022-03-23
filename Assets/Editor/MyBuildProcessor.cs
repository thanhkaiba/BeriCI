#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

// Output the build size or a failure depending on BuildPlayer.

namespace Piratera.Build
{
    public class MyBuildProcessor : Editor
    {
        public static string BuildFolder = $"{Directory.GetCurrentDirectory()}/Build";
        public static string InnoSetupFile = $"{BuildFolder}/Installer/piratera_installer_l2cpp_DEV.iss";

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
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
                {
                    scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                    locationPathName = path + "/Piratera.exe",
                    target = BuildTarget.StandaloneWindows,
                    options = BuildOptions.None
                };


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

        public static void BuildInnoSetup(string version)
        {

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
            PlayerSettings.Android.keystorePass = "depvuimoingay";
            PlayerSettings.Android.keyaliasPass = "depvuimoingay";
            string path = $"{BuildFolder}/Android";

            EditorUserBuildSettings.buildAppBundle = EditorUtility.DisplayDialog(
              "Build Type",
              "Choose Android Build Type",
              "AAB",
              "APK"
            );

            string ext = EditorUserBuildSettings.buildAppBundle ? "aab" : "apk";
            if (path.Length > 0)
            {
                OnPreprocessBuild(AndroidBuildVersion.OnPreprocessBuild);
                BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
                buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
                buildPlayerOptions.locationPathName = $"{BuildFolder}/Android" + $"/Piratera v{AndroidBuildVersion.Instance.CurrentVersion}.{ext}";
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


        [MenuItem("Builds/Window/Upload/Dev", false, 1)]
        public static void SyncWindowVersion()
        {
            SyncGameVersion.Sync(BuildType.WINDOW_DEV, WindowBuildVersion.Instance.CurrentVersion);
        }

        [MenuItem("Builds/Window/Upload/Live", false, 1)]
        public static void SyncWindowVersionLive()
        {
            SyncGameVersion.Sync(BuildType.WINDOW_LIVE, WindowBuildVersion.Instance.CurrentVersion);
        }

        [MenuItem("Builds/Android/Upload/Dev", false, 1)]
        public static void SyncAndroidVersion()
        {
            SyncGameVersion.Sync(BuildType.ANDROID_DEV, AndroidBuildVersion.Instance.CurrentVersion);
        }

        [MenuItem("Builds/Android/Upload/Live", false, 1)]
        public static void SyncAndroidVersionLive()
        {
            SyncGameVersion.Sync(BuildType.ANDROID_LIVE, AndroidBuildVersion.Instance.CurrentVersion);
        }
    }
}
#endif