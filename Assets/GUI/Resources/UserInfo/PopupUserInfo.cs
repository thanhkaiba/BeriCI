using DG.Tweening;
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupUserInfo : MonoBehaviour
{
    private string uid;
    [SerializeField]
    private Text username, userid, joinTime, beriEarn, winrateArena;
    [SerializeField]
    private GameObject btnChangeName, btnChangeAvt;
    [SerializeField]
    private Transform background;
    private void Awake()
    {
        NetworkController.Listen(OnReceiveServerAction);
        Appear();
    }
    private void OnDestroy()
    {
        NetworkController.RemoveListener(OnReceiveServerAction);
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
    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        Debug.Log("SFSAction.USER_DETAIL: " + SFSAction.USER_DETAIL);
        switch (action)
        {
            case SFSAction.USER_DETAIL:
                Debug.Log("errorCode: " + errorCode);
                if (errorCode == SFSErrorCode.SUCCESS)
                {
                    //ShowInfo();
                }
                // else Close();
                break;
        }
    }
    private void Start()
    {
        username.text = "---";
        userid.text = "---";
        joinTime.text = "---";
        beriEarn.text = "---";
        winrateArena.text = "---";
        for (short x = 0; x < 3; x++)
        {
            for (short y = 0; y < 3; y++)
            {
                var icon = transform.FindDeepChild("sailor_" + x + "_" + y);
                icon.gameObject.SetActive(false);
            }
        }
        ShowFightingLine(CrewData.Instance.Sailors, CrewData.Instance.FightingTeam);
    }
    public void SetUID(string _uid)
    {
        Debug.Log("PopupUserInfo " + _uid);
        uid = _uid;
        btnChangeName.SetActive(uid == UserData.Instance.UID);
        btnChangeAvt.SetActive(uid == UserData.Instance.UID);
        NetworkController.Send(SFSAction.USER_DETAIL);
    }
    public void ShowInfo(string _username, string _userid, long _joinTime, long _beriEarn, int pvp_count, int pvp_win)
    {
        username.text = _username;
        userid.text = _userid;
        joinTime.text = _userid;
    }
    public void ShowFightingLine(List<SailorModel> sailors, FightingLine fgl)
    {
        for (short x = 0; x < 3; x++)
        {
            for (short y = 0; y < 3; y++)
            {
                string sailorID = fgl.SailorIdAt(x, y);
                Debug.Log("sailorID: " + sailorID);
                var icon = transform.FindDeepChild("sailor_" + x + "_" + y);
                if (sailorID != "")
                {
                    icon.gameObject.SetActive(true);
                    SailorModel sailor = sailors.Find(sailor => sailor.id == sailorID);
                    var iconS = icon.GetComponent<IconSailor>();
                    iconS.PresentData(sailor);
                }
                else
                {
                    icon.gameObject.SetActive(false);
                }
            }
        }
    }
    public void ChangeName()
    {
        GuiManager.Instance.AddGui("UserInfo/PopupChangeName");
    }
    public void Close()
    {
        Destroy(gameObject);
    }
}
