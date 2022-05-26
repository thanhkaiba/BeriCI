using DG.Tweening;
using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupChangeAvatar : MonoBehaviour
{
    private int COL_NUM = 5;
    [SerializeField]
    private Transform tableContent;
    [SerializeField]
    private GameObject tableRow;
    [SerializeField]
    private UserAvatar avt;
    [SerializeField]
    private Transform background;
    private void Awake()
    {
        NetworkController.Listen(OnReceiveServerAction);
        NetworkController.Send(Action.USER_LIST_AVT);
        Appear();
    }
    private void Appear()
    {
        Sequence s = DOTween.Sequence();
        var canvasGroup = background.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.DOFade(1, 0.2f);
        s.AppendCallback(() => canvasGroup.interactable = true);

        background.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        background.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetLink(background.gameObject).SetTarget(background.transform);

        var fog = GetComponent<HaveFog>();
        if (fog) fog.FadeIn(0.3f);
    }
    private void OnDestroy()
    {
        NetworkController.RemoveListener(OnReceiveServerAction);
    }
    private void OnReceiveServerAction(Action action, SFSErrorCode errorCode, ISFSObject packet)
    {
        Debug.Log("SFSAction.USER_DETAIL: " + Action.USER_DETAIL);
        avt.ShowAvatar(UserData.Instance.AvtId);
        switch (action)
        {
            case Action.USER_LIST_AVT:
                Debug.Log("errorCode: " + errorCode);
                if (errorCode == SFSErrorCode.SUCCESS)
                {
                    var listAvatar = packet.GetIntArray("list_avt");
                    System.Array.Sort(listAvatar);
                    ShowListAvt(listAvatar);
                }
                break;
        }
    }
    private void ShowListAvt(int[] listAvt)
    {
        int avtCount = listAvt.Length;
        int rowNum = (int) Mathf.Ceil(avtCount / (float) COL_NUM);
        for (int i = 0; i < rowNum; i++)
        {
            var go = Instantiate(tableRow, tableContent);
            for (int j = 0; j < COL_NUM; j++)
            {
                int idx = i * COL_NUM + j;
                var img = go.transform.Find("avt_" + j);
                if (idx >= avtCount) img.gameObject.SetActive(false);
                else
                {
                    img.GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/Avatar/" + listAvt[idx]);
                    var btn = img.gameObject.AddComponent<Button>();
                    btn.onClick.AddListener(() => OnSelectAvatar(listAvt[idx]));
                }
            }
        }
    }
    private void OnSelectAvatar(int avt_id)
    {
        Debug.Log("avt_id " + avt_id);
        avt.ShowAvatar(avt_id);
        UserData.Instance.AvtId = avt_id;
        SFSObject sfsObject = new SFSObject();
        sfsObject.PutInt("avt_id", avt_id);
        NetworkController.Send(Action.USER_CHANGE_AVT, sfsObject);
        GameEvent.UserAvtChange.Invoke();
    }
    public void Close()
    {
        FindObjectOfType<PopupUserInfo>().SetUID(UserData.Instance.UID);
        Destroy(gameObject);
    }
}
