using Sfs2X.Entities.Data;
using Piratera.GUI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

public class LoadServerDataUI : MonoBehaviour
{
    [SerializeField]
    private Text percentText;

    [SerializeField]
    private Button buttonReload;

    [SerializeField]
    private Slider progressBar;

    [SerializeField]
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
    private Text sailorRank;


    [SerializeField]
    private GameObject sailorNode;

    [SerializeField]
    private Image BgSlotCapacity;


    void Start()
    {
        RandomTip();
        buttonReload.gameObject.SetActive(false);
        progressBar.value = 0;
        SendGetData();
        ShowLoading(15f, startingPoint, RandomTip);

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
        if (GameUtils.AddSailorImage(param.root_name, sailorNode.transform, out SailorConfig config_stats) != null)
        {

            sailorName.text = param.present_name;
            sailorBio.text = param.title;
            sailorDescripton.text = param.skill_description;
          
            sailorRank.text = config_stats.rank.ToString();
        } else
        {
            RandomTip();
        }

    }

    public void SendGetData()
    {
        buttonReload.gameObject.SetActive(false);
        NetworkController.Send(SFSAction.LOAD_LIST_HERO_INFO);
    }

    private void OnDestroy()
    {
        NetworkController.RemoveServerActionListener(OnReceiveServerAction);
        progressBar.onValueChanged.RemoveListener(UpdateTextPercent);
    }

    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        if (action == SFSAction.LOAD_LIST_HERO_INFO)
        {
            if (errorCode == SFSErrorCode.SUCCESS)
            {
                ShowLoading(0.5f, 1f, OnLoadSuccess);
            } else
            {
                OnLoadError(errorCode);
            }
        }

    }

    private void OnLoadError(SFSErrorCode errorCode)
    {
        GuiManager.Instance.ShowPopupNotification($"Load list hero fail! \n {errorCode}: {(int)errorCode}");
        
        buttonReload.gameObject.SetActive(true);

        /*GuiManager.Instance.ShowPopupNotification($"Load list hero fail! \n {errorCode}: {(int)errorCode}", () => {
            Debug.Log("da load scene moi");
            SceneManager.LoadScene("SceneLogin");
                    
          }
        );*/

    }

 

    public void ShowLoading(float actionTime, float value, Action action)
    {
        DOTween.Kill(progressBar);
        Sequence seq = DOTween.Sequence();
        seq.Append(progressBar.DOValue(value, actionTime));
        seq.AppendCallback(() => action());
        seq.SetLink(progressBar.gameObject);
        seq.SetTarget(progressBar);
    }


    private void OnLoadSuccess()
    {
        SceneManager.LoadScene("SceneLobby");
    }

    private void Awake()
    {
        NetworkController.AddServerActionListener(OnReceiveServerAction);
    }

}
