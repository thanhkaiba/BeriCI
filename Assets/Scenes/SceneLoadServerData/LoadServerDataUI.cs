using DG.Tweening;
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Piratera.Config;

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

    private float startingPoint = 0.3f;

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
    private GameObject sailorClass;

    [SerializeField]
    private Text errorText;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private GameConfigSync ConfigSync;


    void Start()
    {
        VisibleErrorUI(false);
        RandomTip();
        buttonReload.gameObject.SetActive(false);
        progressBar.value = 0;
        ShowLoading(1f, startingPoint, SendGetData);
        progressBar.onValueChanged.AddListener(UpdateTextPercent);
    }

    void UpdateTextPercent(float value)
    {
        percentText.text = value.ToString("P1");
    }

    void RandomTip()
    {

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
        errorText.gameObject.SetActive(visible);
    }

    public void SendGetData()
    {
        progressBar.value = startingPoint;
        VisibleErrorUI(false);
        NetworkController.Send(SFSAction.LOAD_LIST_HERO_INFO);
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
        if (action == SFSAction.LOAD_LIST_HERO_INFO)
        {
            if (errorCode == SFSErrorCode.SUCCESS)
            {
                ShowLoading(1f, 0.4f, OnLoadSuccess);
            }
            else
            {
                OnLoadError(errorCode);
            }
        }

    }

    private void OnLoadError(SFSErrorCode errorCode)
    {
        errorText.text = $"Load list hero fail! \n {errorCode}: {(int)errorCode}";
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


    private void OnLoadSuccess()
    {
        ConfigSync.StartFlowSync(x => progressBar.value += x, () =>
        {
            GlobalConfigs.InitSyncConfig();
            SceneManager.LoadScene("SceneLobby");
        }, () => {
            errorText.text = "Synchronization Failed";
            VisibleErrorUI(true);
        });
        
    }

    private void Awake()
    {
        NetworkController.AddServerActionListener(OnReceiveServerAction);
    }

}
