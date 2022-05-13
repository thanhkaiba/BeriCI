using Piratera.Config;
using Piratera.GUI;
using Piratera.Network;
using Piratera.Utils;
using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupHelm : MonoBehaviour
{
    [SerializeField]
    private Image helm;
    [SerializeField]
    private Text helm_level, ticket_cap, arena_ticket, next_level_desc, beri_cost;
    [SerializeField]
    private GameObject btnUpgrade;
    private void Awake()
    {
        NetworkController.Listen(OnReceiveServerAction);
        PresentHelmWithLevel(UserData.Instance.HelmLevel);
        UpdateAllStatus();
    }
    private void OnDestroy()
    {
        NetworkController.RemoveListener(OnReceiveServerAction);
    }
    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        if (action == SFSAction.SAIL_UPGRADE)
        {
            SceneTransition.Instance.ShowWaiting(false);
            if (errorCode == SFSErrorCode.SUCCESS)
            {
                UpgradeHelmSuccess();
            }
            else
            {

            }
        }
    }
    private void UpdateAllStatus()
    {
        var config = GlobalConfigs.UpgradeShipConfig;
        int ticketMax = GlobalConfigs.PvP.max_ticket;
        int helmLevel = UserData.Instance.HelmLevel;
        int maxHelmLevel = config.GetHelmMaxLevel();
        helm_level.text = "Helm level " + (helmLevel + 1);
        arena_ticket.text = PvPData.Instance.Ticket + "/" + PvPData.Instance.GetTicketCap();
        ticket_cap.text = $"Arena ticket Cap: {ticketMax + config.GetTicketCapacity(helmLevel)} ({ticketMax} + {config.GetTicketCapacity(helmLevel)})";
        if (helmLevel >= maxHelmLevel)
        {
            next_level_desc.gameObject.SetActive(false);
            btnUpgrade.SetActive(false);
        }
        else
        {
            next_level_desc.text = $"Next level: Cap + {config.GetTicketCapacity(helmLevel + 1) - config.GetTicketCapacity(helmLevel)}";
            btnUpgrade.SetActive(true);
            beri_cost.text = config.GetHelmNextLevelPrice(helmLevel).ToString("N0");
        }
    }
    public void Close()
    {
        Destroy(gameObject);
    }
    public void OnUpgadeHelm()
    {
        UpgradeHelmSuccess();
        return;
        var config = GlobalConfigs.UpgradeShipConfig;
        if (UserData.Instance.Beri < config.GetHelmNextLevelPrice(UserData.Instance.HelmLevel))
        {
            GuiManager.Instance.ShowPopupNotification("You do not have enough beri");
        }
        else
        {
            SceneTransition.Instance.ShowWaiting(true);
            NetworkController.Send(SFSAction.HELM_UPGRADE);
        }
    }
    private void UpgradeHelmSuccess()
    {
        UserData.Instance.HelmLevel++;
        PresentHelmWithLevel(UserData.Instance.HelmLevel);
        UpdateAllStatus();
    }
    private void PresentHelmWithLevel(int level)
    {
        int helmImgIdx = level + 1;
        helm.sprite = Resources.Load<Sprite>("UI/UpgradeShip/helm/helm_" + helmImgIdx);
    }
}
