using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum TransitionType
{
    ARENA,
    FADE,
    BATTLE,
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
    }
    private void Start()
    {
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
                    //SetTransitionTheme(type);
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
            case TransitionType.BATTLE:
                {
                    //SetTransitionTheme(type);
                    transition = transitions[2];
                    transition.gameObject.SetActive(true);
                    transition.SetTrigger("Start");
                    yield return new WaitForSeconds(0.5f);
                    var progress = SceneManager.LoadSceneAsync(sceneName);
                    while (!progress.isDone)
                        yield return null;
                    transition.SetTrigger("End");
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
    private GameObject fog;
    public void ShowWaiting(bool b = true, bool visible = true)
    {
        waiting.SetActive(b);
        if (b)
        {
            if (!fog)
            {
                fog = new GameObject("PanelFog");
                fog.transform.SetParent(GameObject.Find("Canvas").transform);
                fog.transform.localPosition = Vector3.zero;
                fog.AddComponent<CanvasRenderer>();
                RectTransform rectTransform = fog.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(Screen.width * 2, Screen.height * 2);
                Image image = fog.AddComponent<Image>();
                image.color = new Color(0, 0, 0, 0);
                image.raycastTarget = true;
            }
            fog.transform.SetAsLastSibling();
            fog.SetActive(true);
        }
        else
        {
            if (fog) fog.SetActive(false);
        }
    }
    public void SetTransitionTheme(TransitionType type)
    {
        var img = transitions[0].transform.Find("Logo").GetComponent<Image>();
        switch (type)
        {
            case TransitionType.ARENA:
                img.sprite = Resources.Load<Sprite>("UI/Arena/arena_header");
                break;
            case TransitionType.BATTLE:
                img.sprite = Resources.Load<Sprite>("UI/Combat/battle_icon");
                break;
        }
        img.SetNativeSize();
    }
}
