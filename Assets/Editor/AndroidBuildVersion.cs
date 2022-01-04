#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[InitializeOnLoad]
public class AndroidBuildVersion : EditorResourceSingleton<AndroidBuildVersion>
{
    public int MajorVersion;
    public int MinorVersion = 1;
    public int BuildVersion;
    [HideInInspector]
    public string CurrentVersion;

    protected override void OnInstanceLoaded()
    {
        UpdateVersionNumber();
    }

    [PostProcessBuild(1)]
    public static void OnPreprocessBuild()
    {
        Debug.Log("Build v" + Instance.CurrentVersion);
        IncreaseBuild();
    }

    [MenuItem("Builds/Android/Create Version File", false, 2)]
    private static void Create()
    {
        Instance.Make();
    }

    private static void IncreaseBuild()
    {
        Instance.BuildVersion++;
        Instance.UpdateVersionNumber();
    }

    public void UpdateVersionNumber()
    {
        //Make your custom version layout here.
        CurrentVersion = MajorVersion.ToString("0") + "." + MinorVersion.ToString("00") + "." + BuildVersion.ToString("000");
        PlayerSettings.Android.bundleVersionCode = MajorVersion * 10000 + MinorVersion * 1000 + BuildVersion;
        PlayerSettings.bundleVersion = CurrentVersion;
    }
}

[CustomEditor(typeof(AndroidBuildVersion))]
public class AndroidBuildEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AndroidBuildVersion myScript = (AndroidBuildVersion)target;
        if (GUILayout.Button("Force Update"))
        {
            myScript.UpdateVersionNumber();
        }
        EditorGUILayout.LabelField("Current Version", Application.version);
    }
}

#endif