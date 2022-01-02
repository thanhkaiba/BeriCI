#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Linq;
using System.IO;
using System.Diagnostics;

// Output the build size or a failure depending on BuildPlayer.

public class MyBuildProcessor : Editor
{
    public static string BuildFolder = $"{Directory.GetCurrentDirectory()}/Build";
    public static string InnoSetupFile = $"{BuildFolder}/Installer/piratera_installer_l2cpp.iss";

    [MenuItem("Builds/Reveal In Finder")]
    public static void OpenBuildFolder()
    {
        EditorUtility.RevealInFinder(BuildFolder);
    }

    [MenuItem("Builds/Window/Build")]
    public static void BuildWindow()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", BuildFolder, "l2cpp");
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

            WindowBuildVersion.OnPreprocessBuild();
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
                BuildInnoSetup();
            }

            if (summary.result == BuildResult.Failed)
            {
                UnityEngine.Debug.Log("Build Window failed");
            }

        }
    }

    [MenuItem("Builds/Window/Build Installer")]
    public static void BuildInnoSetup()
    {
        Process process = new Process();
        Process p = new Process();
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = "D:/Program Files/Inno Setup 6/iscc.exe";
        psi.Arguments = $" /DMyAppVersion={WindowBuildVersion.Instance.CurrentVersion} " + InnoSetupFile;

        p.StartInfo = psi;
        p.Start();
  
        p.WaitForExit();


    }

    [MenuItem("Builds/Android/Build")]
    public static void AndroidBuild()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", BuildFolder, "Android");
        if (path.Length > 0)
        {
            AndroidBuildVersion.OnPreprocessBuild();
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            buildPlayerOptions.locationPathName = path + "/Piratera.apk";
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
}
#endif