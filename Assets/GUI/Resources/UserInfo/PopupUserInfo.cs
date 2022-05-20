using DG.Tweening;
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PopupUserInfo : MonoBehaviour
{
    private string uid;
    [SerializeField]
    private Text username, userid, joinTime, beriEarn, pvePlay, pveWinrate, pvpPlay, pvpWinrate;
    [SerializeField]
    private GameObject btnChangeName, btnChangeAvt, btnChallenge;
    [SerializeField]
    private Transform background;
    [SerializeField]
    private UserAvatar avatar;
    [SerializeField]
    private Image ship, sail, helm;
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
                    var unique_id = packet.GetLong("uid").ToString();
                    var username = packet.GetUtfString("username");
                    var avt_id = packet.GetInt("avt_id");
                    var account_date = packet.GetLong("account_date");
                    var pve_count = packet.GetInt("pve_count");
                    var pve_win = packet.GetInt("pve_win");
                    var pvp_count = packet.GetInt("pvp_count");
                    var pvp_win = packet.GetInt("pvp_win");
                    var total_beri_earn = packet.GetLong("total_beri_earn");
                    var sail_level = packet.GetInt("sail_level");
                    var helm_level = packet.GetInt("helm_level");
                    //var sail_level = 5;
                    //var helm_level = 5;

                    var sailors = new List<SailorModel>();
                    ISFSArray sFSSailors = packet.GetSFSArray("sailors");
                    foreach (ISFSObject obj in sFSSailors)
                    {
                        SailorModel model = new SailorModel(obj);
                        sailors.Add(model);
                    }

                    var fighting_lines = new FightingLine();
                    fighting_lines.NewFromSFSObject(packet.GetSFSArray("fighting_lines"));
                    ShowInfo(username, unique_id, account_date, total_beri_earn, pvp_count, pvp_win, avt_id, pve_count, pve_win);
                    ShowSailHelm(sail_level, helm_level);

                    btnChangeName.SetActive(uid == UserData.Instance.UID);
                    btnChangeAvt.SetActive(uid == UserData.Instance.UID);
                    btnChallenge.SetActive(uid != UserData.Instance.UID);
                    ShowFightingLine(sailors, fighting_lines);
                }
                else Close();
                break;
        }
    }
    private void Start()
    {
        username.text = "---";
        userid.text = "---";
        joinTime.text = "---";
        beriEarn.text = "---";
        pvePlay.text = "---";
        pveWinrate.text = "---";
        pvpPlay.text = "---";
        pvpWinrate.text = "---";
        for (short x = 0; x < 3; x++)
        {
            for (short y = 0; y < 3; y++)
            {
                var icon = transform.FindDeepChild("sailor_" + x + "_" + y);
                icon.gameObject.SetActive(false);
            }
        }
        // ShowFightingLine(CrewData.Instance.Sailors, CrewData.Instance.FightingTeam);

        btnChangeName.SetActive(false);
        btnChangeAvt.SetActive(false);
        btnChallenge.SetActive(false);
    }
    public void SetUID(string _uid)
    {
        Debug.Log("PopupUserInfo " + _uid);
        uid = _uid;

        SFSObject sfsObject = new SFSObject();
        sfsObject.PutUtfString("uid", uid);
        NetworkController.Send(SFSAction.USER_DETAIL, sfsObject);
    }
    public void ShowInfo(string _username, string _userid, long _joinTime, long _beriEarn, int pvp_play, int pvp_win, int avt_id, int pve_play, int pve_win)
    {
        username.text = _username;
        userid.text = _userid;

        DateTime date = (new DateTime(1970, 1, 1)).AddMilliseconds(_joinTime);
        joinTime.text = date.ToString("MM/dd/yyyy");

        beriEarn.text = _beriEarn.ToString("N0");
        pvePlay.text = "" + pve_play;
        pveWinrate.text = pve_play != 0 ? (Math.Round((float)pve_win / (float)pve_play * 100, 1) + "%") : "0%";
        pvpPlay.text = "" + pvp_play;
        pvpWinrate.text = pvp_play != 0 ? (Math.Round((float)pvp_win / (float)pvp_play * 100, 1) + "%") : "0%";
        avatar.ShowAvatar(avt_id);

        TempCombatData.Instance.userName1 = _username;
        TempCombatData.Instance.avt1 = avt_id;
    }
    public void ShowFightingLine(List<SailorModel> sailors, FightingLine fgl)
    {
        TempCombatData.Instance.listSailor = sailors;
        TempCombatData.Instance.fgl1 = fgl;
        for (short x = 0; x < 3; x++)
        {
            for (short y = 0; y < 3; y++)
            {
                string sailorID = fgl.SailorIdAt(x, y);
                var icon = transform.FindDeepChild("sailor_" + x + "_" + y);
                if (sailorID != "")
                {
                    icon.gameObject.SetActive(true);
                    SailorModel sailor = sailors.Find(sailor => sailor.id == sailorID);
                    var iconS = icon.GetComponent<IconSailor>();
                    iconS.PresentData(sailor);
                    icon.name = "sailor_" + x + "_" + y;
                }
                else
                {
                    icon.gameObject.SetActive(false);
                }
            }
        }
        if (fgl.IsEmpty() || CrewData.Instance.FightingTeam.IsEmpty()) btnChallenge.SetActive(false);
    }
    public void ShowSailHelm(int sail_level, int helm_level)
    {
        int sailImgIdx = sail_level + 1;
        int bodyImgIdx = sail_level < 4 ? 1 : 2;
        sail.sprite = Resources.Load<Sprite>("UI/UpgradeShip/sail_" + sailImgIdx);
        ship.sprite = Resources.Load<Sprite>("UI/UpgradeShip/ship_" + bodyImgIdx);

        int helmImgIdx = helm_level + 1;
        helm.sprite = Resources.Load<Sprite>("UI/UpgradeShip/helm/helm_" + helmImgIdx);
    }
    public void ChangeName()
    {
        var popup = GuiManager.Instance.AddGui("UserInfo/PopupChangeName");
    }
    public void ChangeAvatar()
    {
        GuiManager.Instance.AddGui("UserInfo/PopupChangeAvatar");
    }
    public void Close()
    {
        Destroy(gameObject);
    }
    public void Challenge()
    {
        TempCombatData.Instance.challengeGame = true;
        TempCombatData.Instance.lastScene = SceneManager.GetActiveScene().name;
        TempCombatData.Instance.userName0 = UserData.Instance.Username;
        TempCombatData.Instance.avt0 = UserData.Instance.AvtId;
        SceneTransition.Instance.LoadScene("SceneCombat2D", TransitionType.BATTLE);
    }
}
