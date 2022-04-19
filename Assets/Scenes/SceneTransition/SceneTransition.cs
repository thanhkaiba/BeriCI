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
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(sceneName);
    }
}
