﻿#if UNITY_EDITOR 
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[InitializeOnLoad]
public class AndroidBuildVersion : EditorResourceSingleton<AndroidBuildVersion>
{
    public int MajorVersion;
    public int MinorVersion = 1;
    public int BuildVersion;
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

    void IncrementVersion(int majorIncr, int minorIncr, int buildIncr)
    {
        MajorVersion += majorIncr;
        MinorVersion += minorIncr;
        BuildVersion += buildIncr;

        UpdateVersionNumber();
    }


    [MenuItem("Builds/Android/Create Version File")]
    private static void Create()
    {
        Instance.Make();
    }

    [MenuItem("Builds/Android/Increase Major Version")]
    protected static void IncreaseMajor()
    {
        Instance.MajorVersion++;
        Instance.MinorVersion = 0;
        Instance.BuildVersion = 0;
        Instance.UpdateVersionNumber();
    }
    [MenuItem("Builds/Android/Increase Minor Version")]
    protected static void IncreaseMinor()
    {
        Instance.MinorVersion++;
        Instance.BuildVersion = 0;
        Instance.UpdateVersionNumber();
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
public class CustomAndroidVersionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AndroidBuildVersion customType = (AndroidBuildVersion)target;

        int MajorVersion = EditorGUILayout.IntField("MajorVersion", customType.MajorVersion);
        int MinorVersion = EditorGUILayout.IntField("MinorVersion", customType.MinorVersion);
        int BuildVersion = EditorGUILayout.IntField("BuildVersion", customType.BuildVersion);

        if (MajorVersion != customType.MajorVersion || MinorVersion != customType.MinorVersion || BuildVersion != customType.BuildVersion)
        {
            customType.MajorVersion = MajorVersion;
            customType.MinorVersion = MinorVersion;
            customType.BuildVersion = BuildVersion;
            customType.UpdateVersionNumber();
        }

        EditorGUILayout.LabelField($"Current Version: " + customType.CurrentVersion);
    }
}


#endif