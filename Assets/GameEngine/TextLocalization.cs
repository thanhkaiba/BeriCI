using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class TextLocalization
{
    public bool useCoroutine;



    public static string Text(string key)
    {
        try {

            return LocalizationSettings.StringDatabase.GetLocalizedString(key);

        } catch (Exception e)
        {  
            Debug.LogError(e);
            return key;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject">A GameObject has a Text component</param>
    /// <param name="key">key for localize string</param>
    /// <param name="table">localize table database</param>
    /// <returns</returns>
    public static LocalizeStringEvent Text(GameObject gameObject, string key, string table = "TextLocalization")
    {
        
        LocalizeStringEvent localizeStringEvent = gameObject.AddComponent<LocalizeStringEvent>();
        localizeStringEvent.StringReference.SetReference(table, key);
        localizeStringEvent.OnUpdateString.AddListener(text => gameObject.GetComponent<Text>().text = text);
        return localizeStringEvent;
    }

}
