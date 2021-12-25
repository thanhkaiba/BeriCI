using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClassBonusLevel : ScriptableObjectPro
{
    public ClassBonus container;
    public int pop;
    public List<float> _params;

#if UNITY_EDITOR
    public void Initalise(ClassBonus _container)
    {
        container = _container;
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Save Name")]
    private void Rename()
    {
        this.name = container.type.ToString() + "_" + container.levels.IndexOf(this);
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(this);
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Delete This")]
    private void DeleteThis()
    {
        container.levels.Remove(this);
        Undo.DestroyObjectImmediate(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
