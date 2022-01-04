#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class WindowBuildVersion : EditorResourceSingleton<WindowBuildVersion>
{
    public int MajorVersion;
    public int MinorVersion = 1;
    public int BuildVersion;
    [HideInInspector]
    public string CurrentVersion;

    protected override void OnInstanceLoaded()
    {
        base.OnInstanceLoaded();
        UpdateVersionNumber();
    }

    public static void OnPreprocessBuild()
    {
        Debug.Log("Build v" + Instance.CurrentVersion);
        IncreaseBuild();
    }

 

    [MenuItem("Builds/Window/Create Version File", false, 2)]
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


[CustomEditor(typeof(WindowBuildVersion))]
public class WindowBuildEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WindowBuildVersion myScript = (WindowBuildVersion)target;
        if (GUILayout.Button("Force Update"))
        {
            myScript.UpdateVersionNumber();
        }
        EditorGUILayout.LabelField("Current Version", Application.version);
    }
}


#endif