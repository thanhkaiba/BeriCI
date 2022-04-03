using DG.Tweening;
using Piratera.Config;
using Piratera.Network;
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadServerDataUI : MonoBehaviour
{
    [SerializeField]
    private Text percentText;

    [SerializeField]
    private Button buttonReload;

    [SerializeField]
    private Button buttonLogout;

    [SerializeField]
    private Slider progressBar;

    private float startingPoint = 0f;

    [SerializeField]
    private SailorDescription sailorDescription;

    [Header("tooltip")]
    [SerializeField]
    private Text sailorName;

    [SerializeField]
    private Text sailorBio;

    [SerializeField]
    private Text sailorDescripton;

    [SerializeField]
    private Sprite[] spriteRanks;


    [SerializeField]
    private Image sailorRank;

    [SerializeField]
    private GameObject sailorNode;
    [SerializeField]
    private SpriteRenderer ligh1;
    [SerializeField]
    private SpriteRenderer ligh2;

    [SerializeField]
    private GameObject sailorClass;

    [SerializeField]
    private Text errorText;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameConfigSync ConfigSync;

    private Action ReloadFunc;

    public static string NextScene = "SceneLobby";

    private readonly HashSet<SFSAction> ActionRequires = new HashSet<SFSAction>() { 
        SFSAction.PIRATE_WHEEL_DATA, 
        SFSAction.LOAD_LIST_HERO_INFO,
        SFSAction.TRAIN_SAILORS_REMAIN,
        // SFSAction.PVP_DATA,
    };
    private int TotalActionRequire = 0;
    void Start()
    {
        VisibleErrorUI(false);
        PrepareAppear();

        progressBar.value = 0;
        UpdateTextPercent(0);
        progressBar.onValueChanged.AddListener(UpdateTextPercent);

        if (!GameConfigSync.Synced)
        {
            errorText.text = "Loading Configuration Files";
            LoadConfig();
        }
        else
        {
            SendGetData();
        }
    }
    void PrepareAppear()
    {
        GameObject[] list = { sailorRank.gameObject, sailorName.gameObject, sailorBio.gameObject, sailorDescripton.gameObject, ligh1.gameObject, ligh2.gameObject };

        foreach (GameObject GO in list)
        {
            GO.SetActive(false);
        }
    }
    void AppearTip()
    {
        GameObject[] list = { sailorRank.gameObject, sailorName.gameObject, sailorBio.gameObject, sailorDescripton.gameObject, ligh1.gameObject, ligh2.gameObject };

        foreach (GameObject GO in list)
        {
            GO.SetActive(true);
        }
        sailorRank.DOFade(0, 0.4f).From().SetLink(sailorRank.gameObject);
        sailorName.DOFade(0, 0.4f).From().SetLink(sailorName.gameObject);
        sailorBio.DOFade(0, 0.4f).From().SetLink(sailorBio.gameObject);
        sailorDescripton.DOFade(0, 0.4f).From().SetLink(sailorDescripton.gameObject);
        ligh1.DOFade(0, 0.4f).From().SetLink(ligh1.gameObject);
        ligh2.DOFade(0, 0.4f).From().SetLink(ligh2.gameObject);
    }
    void UpdateTextPercent(float value)
    {
        percentText.text = value.ToString("P1");
    }
    void RandomTip()
    {
        AppearTip();
        SailorDescription.Param param = sailorDescription.sheets[0].list[UnityEngine.Random.Range(0, sailorDescription.sheets[0].list.Count)];
        foreach (Transform child in sailorNode.transform)
        {
            DestroyImmediate(child.gameObject);
        }
        GameObject sailor;
        if ((sailor = GameUtils.AddSailorImage(param.root_name, sailorNode.transform, out SailorConfig config_stats)) != null)
        {
            BoxCollider2D box = sailor.GetComponent<BoxCollider2D>();
            sailorRank.transform.position = Camera.main.WorldToScreenPoint(sailorNode.transform.position - new Vector3(box.size.x / 2 - box.offset.x, 0, 0)) + new Vector3(0, 10, 0);
            sailorName.text = param.present_name;
            sailorBio.text = param.title;
            sailorDescripton.text = param.skill_description;
            sailorRank.sprite = spriteRanks[(int)config_stats.rank];
            RenderClass(config_stats.classes);
        }
        else
        {
            RandomTip();
        }
    }
    public void VisibleErrorUI(bool visible)
    {
        progressBar.gameObject.SetActive(!visible);
        buttonReload.gameObject.SetActive(visible);
        percentText.gameObject.SetActive(!visible);
        buttonLogout.gameObject.SetActive(visible);
    }
    public void SendGetData()
    {
        errorText.text = "Loading Sailors";
        ReloadFunc = SendGetData;
        progressBar.value = startingPoint + 0.3f;
        RandomTip();
        TotalActionRequire = ActionRequires.Count;
        foreach (SFSAction action in ActionRequires)
        {
            NetworkController.Send(action);
        }
    }
    private void OnDestroy()
    {
        NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        progressBar.onValueChanged.RemoveListener(UpdateTextPercent);
    }
    private void RenderClass(List<SailorClass> classes)
    {

        foreach (Transform child in sailorClass.transform)
        {
            Destroy(child.gameObject);
        }

        float size = 125;
        float margin = 15;
        float totalSize = size * classes.Count;
        for (int i = 0; i < classes.Count; i++)
        {
            GameObject GO = new GameObject(classes[i].ToString());
            Image image = GO.AddComponent<Image>();
            image.sprite = Resources.Load<Sprite>("Icons/SailorType/" + classes[i]);
            image.color = new Color32(202, 202, 202, 255);
            RectTransform rectTransform = GO.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(size - margin, size - margin);
            rectTransform.SetParent(sailorClass.transform);

            rectTransform.anchoredPosition = new Vector2(-totalSize / 2 + size * (i + 0.5f), 10);
            rectTransform.localScale = Vector3.one;

        }
    }

    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        if (errorCode == SFSErrorCode.SUCCESS)
        {
            if (ActionRequires.Contains(action))
            {
                ActionRequires.Remove(action);
                if (ActionRequires.Count != 0)
                {
                    ShowLoading(1f, 0.4f / TotalActionRequire, () => { });
                }
                else
                {
                    ShowLoading(1f, 0.4f / TotalActionRequire, OnLoadSuccess);
                }
            }
        }
        else
        {
            OnLoadError(errorCode);
        }
    }
    private void OnLoadSuccess()
    {
        SceneManager.LoadScene(NextScene);
    }
    private void OnLoadError(SFSErrorCode errorCode)
    {
        errorText.text = $"Load sailor list fail! \n {errorCode}: {(int)errorCode}";
        DOTween.Kill(progressBar);
        VisibleErrorUI(true);

    }
    public void OnLogout()
    {
        NetworkController.Logout();
    }
    public void ShowLoading(float actionTime, float value, Action callback)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(progressBar.DOValue(value, actionTime).SetRelative());
        seq.AppendCallback(() => callback());
        seq.SetLink(progressBar.gameObject);
        seq.SetTarget(progressBar);
    }
    private void LoadConfig()
    {
        errorText.text = "Loading Configuration Files";
        ReloadFunc = LoadConfig;
        progressBar.value = startingPoint;
        ConfigSync.StartFlowSync(x => progressBar.value += x, () =>
        {
            SendGetData();

        }, () =>
        {
            errorText.text = "Synchronization Failed";
            VisibleErrorUI(true);
        });
    }

    public void Reload()
    {
        if (ReloadFunc != null)
        {
            ReloadFunc();
        }
    }

    private void Awake()
    {
        NetworkController.AddServerActionListener(OnReceiveServerAction);
    }

}
