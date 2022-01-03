#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using Unity.EditorCoroutines.Editor;
using Piratera.Constance;

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
        WindowBuildVersion.Instance.UpdateVersionNumber();
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
        OnPostprocessBuild(SyncWindowVersion);


    }

    [MenuItem("Builds/Android/Build")]
    public static void AndroidBuild()
    {
        AndroidBuildVersion.Instance.UpdateVersionNumber();
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", BuildFolder, "Android");
        if (path.Length > 0)
        {
            OnPreprocessBuild(AndroidBuildVersion.OnPreprocessBuild);
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
                OnPostprocessBuild(SyncAndroidVersion);
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

    public static void OnPostprocessBuild(Action syncAction)
    {
        bool shouldIncrement = EditorUtility.DisplayDialog(
            "Sync Game Version?",
            $"Current: {PlayerSettings.bundleVersion}",
            "Yes",
            "No"
        );

        if (shouldIncrement)
        {
            syncAction();
        }
    }

    [MenuItem("Builds/Window/Sync Version")]
    public static void SyncWindowVersion()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(SyncVersionCode(GameConst.WINDOW_VERSION_URL, WindowBuildVersion.Instance.CurrentVersion));
    }

    [MenuItem("Builds/Android/Sync Version")]
    public static void SyncAndroidVersion()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(SyncVersionCode(GameConst.ANDROID_VERSION_URL, AndroidBuildVersion.Instance.CurrentVersion));
    }

    public static IEnumerator SyncVersionCode(string url, string version)
    {
        var myData = new
        {
            min_version = version,
            start_maintain_time = -1,
            end_maintain_time = -1,
            download_url = "https://drive.google.com/drive/folders/1VfM__mNr_SkPSs3rMfwluTy-iGhqH3Nx"
        };
        var request = new UnityWebRequest(url, "PATCH");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(myData));
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.error != null)
        {
            UnityEngine.Debug.Log("Error: " + request.error);
        }
        else
        {
            UnityEngine.Debug.Log("All OK");
            UnityEngine.Debug.Log("Status Code: " + request.responseCode);
        }

    }

 
}
#endif