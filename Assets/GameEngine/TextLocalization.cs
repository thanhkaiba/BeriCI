using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

public delegate void LocalizeCallBack(string text);
public class TextLocalization : MonoBehaviour
{
    public bool useCoroutine;



    public static string Text(string key)
    {
        AsyncOperationHandle<string> stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(key);
        if (stringOperation.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError("Failed to load string");
            return key;
        }
        else
        {
            return stringOperation.Result;
        }
    }

    public static void Text(string key, LocalizeCallBack localizeCallBack)
    {
        AsyncOperationHandle<string> stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(key);
        if (stringOperation.Status == AsyncOperationStatus.Failed)
        {
            localizeCallBack(stringOperation.Result);
        }
        else
        {
            LoadStringWithCoroutine(stringOperation, key, localizeCallBack);
        }
    }



    private static IEnumerator LoadStringWithCoroutine(AsyncOperationHandle<string> stringOperation, string key, LocalizeCallBack localizeCallBack)
    {
        yield return stringOperation;
        SetString(stringOperation, key, localizeCallBack);
    }

    private static void SetString(AsyncOperationHandle<string> stringOperation, string key, LocalizeCallBack localizeCallBack)
    {
        // Its possible that something may have gone wrong during loading. We can handle this locally
        // or ignore all errors as they will still be captured and reported by the Localization system.
        if (stringOperation.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError("Failed to load string " + key);
            localizeCallBack(key);
        }
        else
        {
            localizeCallBack(stringOperation.Result);
        }
        
    }
}
