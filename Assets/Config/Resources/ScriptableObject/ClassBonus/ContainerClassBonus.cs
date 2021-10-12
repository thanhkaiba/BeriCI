using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "ContainerClassBonus", menuName = "config/ContainerClassBonus")]
public class ContainerClassBonus : ScriptableObjectPro
{
    public List<ClassBonus> classBonusList = new List<ClassBonus>();

#if UNITY_EDITOR
    [ContextMenu("Make ClassBonus")]
    private void MakeNewClassBionus()
    {
        ClassBonus damageType = ScriptableObject.CreateInstance<ClassBonus>();
        damageType.name = "New Damage Type";
        damageType.Initalise(this);

        classBonusList.Add(damageType);

        AssetDatabase.AddObjectToAsset(damageType, this);

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(damageType);
    }
#endif

#if UNITY_EDITOR
    [ContextMenu("Delete All")]
    private void DeleteAll()
    {
        for (int i = classBonusList.Count; i-- > 0;)
        {
            ClassBonus tmp = classBonusList[i];

            classBonusList.Remove(tmp);
            Undo.DestroyObjectImmediate(tmp);
        }
        AssetDatabase.SaveAssets();
    }
#endif

    // function
    private ClassBonus GetPassiveConfig(SailorType type)
    {
        foreach (ClassBonus item in classBonusList)
        {
            if (item.type == type) return item;
        }
        return null;
    }
    public bool HaveCombine(SailorType type)
    {
        var config = GetPassiveConfig(type);
        return config != null;
    }
    public List<int> GetMilestones(SailorType type)
    {
        var config = GetPassiveConfig(type);
        List<int> result = new List<int>();
        if (config != null)
        {
            foreach (ClassBonusLevel l in config.levels)
            {
                result.Add(l.pop);
            }
        }
        return result;
    }
    public int GetMaxPopNeed(SailorType type)
    {
        var config = GetPassiveConfig(type);
        int result = 0;
        if (config != null)
        {
            foreach (ClassBonusLevel l in config.levels)
            {
                if (l.pop > result) result = l.pop;
            }
        }
        return result;
    }
    public List<float> GetParams(SailorType type, int level)
    {
        var config = GetPassiveConfig(type);
        List<float> result = new List<float>();
        if (config != null)
        {
            ClassBonusLevel l = config.levels[level];
            if (l != null) result = l._params;
        }
        return result;
    }
}