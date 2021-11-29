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

    void Start()
    {
        progressBar.value = 0;
        SendGetData();
        ShowLoading(startingPoint, SendGetData);
    }

    public void SendGetData()
    {
        buttonReload.gameObject.SetActive(false);
        NetworkController.Send(SFSAction.LOAD_LIST_HERO_INFO);
    }

    private void OnDestroy()
    {
        NetworkController.RemoveServerActionListener(OnReceiveServerAction);
    }

    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        if (action == SFSAction.LOAD_LIST_HERO_INFO)
        {
            if (errorCode == SFSErrorCode.SUCCESS)
            {
                ShowLoading(1f, OnLoadSuccess);
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

 

    public void ShowLoading(float value, Action action)
    {
        DOTween.Kill(progressBar);
        percentText.text = value.ToString("P");
        DOTween.Kill(progressBar);
        progressBar.DOValue(value, 0.4f).SetLink(progressBar.gameObject).SetTarget(progressBar);

        Sequence seq = DOTween.Sequence();
        seq.Append(progressBar.DOValue(value, 0.4f));
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
