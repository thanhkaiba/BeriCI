using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TransitionType
{
    ARENA,
    FADE,
};
public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    [SerializeField]
    private List<Animator> transitions;
    [SerializeField]
    private GameObject waiting;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        waiting.SetActive(false);
        transitions.ForEach(ani => ani.gameObject.SetActive(false));
    }
    public void LoadScene(string sceneName, TransitionType type = TransitionType.ARENA)
    {
        transitions.ForEach(ani => ani.gameObject.SetActive(false));
        StartCoroutine(LoadSceneT(sceneName, type));
    }
    IEnumerator LoadSceneT (string sceneName, TransitionType type)
    {
        Animator transition;
        switch (type)
        {
            case TransitionType.ARENA:
                {
                    transition = transitions[0];
                    transition.gameObject.SetActive(true);
                    transition.SetTrigger("End");
                    yield return new WaitForSeconds(1.1f);
                    var progress = SceneManager.LoadSceneAsync(sceneName);
                    while (!progress.isDone)
                        yield return null;
                    transition.SetTrigger("Start");
                    break;
                }
            case TransitionType.FADE:
                {
                    transition = transitions[1];
                    transition.gameObject.SetActive(true);
                    transition.SetTrigger("Start");
                    yield return new WaitForSeconds(0.5f);
                    var progress = SceneManager.LoadSceneAsync(sceneName);
                    while (!progress.isDone)
                        yield return null;
                    transition.SetTrigger("End");
                    break;
                }
            default:
                SceneManager.LoadScene(sceneName);
                break;
        }
        //SceneManager.LoadScene(sceneName);
    }
    public void ShowWaiting(bool b = true)
    {
        waiting.SetActive(b);
    }
}
