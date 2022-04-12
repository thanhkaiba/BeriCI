using Piratera.Config;
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupConfirmUnlockAdvantage : BaseGui
{
    private HomefieldAdvantage type;
    [SerializeField]
    private Transform background;
    [SerializeField]
    private Text title, yourBeri, price;
    [SerializeField]
    private Image icon;
    protected override void Start()
    {
        base.Start();
        NetworkController.AddServerActionListener(onReceiveServerAction);
    }
    private void onReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        if (action == SFSAction.PVP_OPEN_HOME_ADVANTAGE)
        {
            OnClose();
        }
    }

    public void SetType(HomefieldAdvantage advantage)
    {
        type = advantage;
        title.text = "Unlock " + GameUtils.GetHomeAdvantageStr(advantage);
        yourBeri.text = UserData.Instance.Beri.ToString("N0");
        price.text = "" + GlobalConfigs.PvP.advantage_price;
        icon.sprite = Resources.Load<Sprite>("UI/Arena/advantage/ad_" + advantage.ToString());
    }
    public void OnClose()
    {
        NetworkController.RemoveServerActionListener(onReceiveServerAction);
        DestroySelf();
    }
    public void OnConfirm()
    {
        var cost = GlobalConfigs.PvP.advantage_price;
        if (UserData.Instance.Beri < cost)
        {
            GuiManager.Instance.ShowPopupNotification("You do not have enough BERI");
        }
        else
        {
            UserData.Instance.Beri -= cost;
            PvPData.Instance.OpenedAdvantage.Add(type);
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("advantage_idx", (int)type);
            NetworkController.Send(SFSAction.PVP_OPEN_HOME_ADVANTAGE, sfsObject);
        }
    }
}