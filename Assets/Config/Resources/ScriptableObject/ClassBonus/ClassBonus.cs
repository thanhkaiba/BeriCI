using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClassBonus : ScriptableObjectPro
{
    public ContainerClassBonus container;

    public SailorClass type;
    public List<ClassBonusLevel> levels;

#if UNITY_EDITOR
    public void Initalise(ContainerClassBonus _container)
    {
        container = _container;
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Save")]
    private void Rename()
    {
        this.name = type.ToString();
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(this);

        levels.ForEach(level =>
        {
            level.name = type.ToString() + "_" + levels.IndexOf(level);
            AssetDatabase.SaveAssets();
            EditorUtility.SetDirty(level);
        });
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Delete This")]
    private void DeleteThis()
    {
        container.classBonusList.Remove(this);
        Undo.DestroyObjectImmediate(this);
        AssetDatabase.SaveAssets();
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Make New Level")]
    private void MakeNewDamageType()
    {
        ClassBonusLevel level = CreateInstance<ClassBonusLevel>();
        level.name = type.ToString() + "_" + (levels.Count);
        level.Initalise(this);

        levels.Add(level);

        AssetDatabase.AddObjectToAsset(level, this);

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(level);
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Delete All Levels")]
    private void DeleteAllLevels()
    {
        for (int i = levels.Count; i-- > 0;)
        {
            ClassBonusLevel tmp = levels[i];

            levels.Remove(tmp);
            Undo.DestroyObjectImmediate(tmp);
        }
        AssetDatabase.SaveAssets();
    }
#endif
}
