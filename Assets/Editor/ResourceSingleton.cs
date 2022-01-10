using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class EditorResourceSingleton<T> : ScriptableObject
       where T : ScriptableObject
{
    public static T m_Instance;
    const string AssetPath = "Assets/Resources";

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                string path = Path.Combine(typeof(T).ToString());
                m_Instance = Resources.Load<T>(path);
#if UNITY_EDITOR
                if (m_Instance == null)
                {
                    Debug.LogError("ResourceSingleton Error: Fail load at " + AssetPath + "/" + typeof(T).ToString());
                    CreateAsset();
                }
                else
                {
                    Debug.Log("ResourceSingleton Loaded: " + typeof (T).ToString());
                }
#endif
                var inst = m_Instance as EditorResourceSingleton<T>;
                if (inst != null)
                {
                    inst.OnInstanceLoaded();
                }
            }
            return m_Instance;
        }
    }

    public void Make() { }
    static void CreateAsset()
    {
        m_Instance = CreateInstance<T>();
        string path = Path.Combine(AssetPath, typeof(T).ToString() + ".asset");
        Debug.Log("Create path: " + path);
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(m_Instance, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = m_Instance;
    }

    protected virtual void OnInstanceLoaded()
    {
    }
}