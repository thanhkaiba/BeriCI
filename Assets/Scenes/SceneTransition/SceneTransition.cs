using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType
{
    ARENA,
};
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    public Animator transition;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    public void LoadScene(string sceneName, TransitionType type = TransitionType.ARENA)
    {
        StartCoroutine(LoadSceneT(sceneName, type));
    }
    IEnumerator LoadSceneT (string sceneName, TransitionType type)
    {
        transition.SetTrigger("End");
        yield return new WaitForSeconds(1.0f);
        var progress = SceneManager.LoadSceneAsync(sceneName);
        while (!progress.isDone)
        {
            yield return null;
        }
        transition.SetTrigger("Start");
        //SceneManager.LoadScene(sceneName);
    }
}
