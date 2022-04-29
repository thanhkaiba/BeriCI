using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Piratera.Config;
using Piratera.Network;
using Sfs2X.Entities.Data;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Piratera.Utils;
using Piratera.GUI;

public class UpgradeShip : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera, sailCamera;
    private Vector3 camOriPos;
    private float camOriSize;
    [SerializeField]
    private SpriteRenderer sail, body;
    [SerializeField]
    private GameObject nodeMenu, nodeSail, btnUpgradeSail;
    [SerializeField]
    private Animator sailEffect;
    [SerializeField]
    private Text textSailPrice, textSailLevel, textHelmLevel, textSail, descSail;
    [SerializeField]
    private Text userBeri, userStamina;
    private void Awake()
    {
        NetworkController.Listen(OnReceiveServerAction);
        GameEvent.UserStaminaChanged.AddListener(OnStaminaChanged);
        GameEvent.UserBeriChanged.AddListener(OnBeriChanged);
    }
    private void OnDestroy()
    {
        NetworkController.RemoveListener(OnReceiveServerAction);
        GameEvent.UserStaminaChanged.RemoveListener(OnStaminaChanged);
        GameEvent.UserBeriChanged.RemoveListener(OnBeriChanged);
    }
    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        if (action == SFSAction.SAIL_UPGRADE)
        {
            SceneTransition.Instance.ShowWaiting(false);
            if (errorCode == SFSErrorCode.SUCCESS)
            {
                UpgradeSailSuccess();
            } else
            {

            }
        }
    }
    void Start()
    {
        camOriPos = mainCamera.transform.position;
        camOriSize = mainCamera.orthographicSize;
        PresentShipWithLevel(UserData.Instance.SailLevel);
        UpdateAllStatus();
        OnClickMenu();

        userBeri.text = StringUtils.ShortNumber(UserData.Instance.Beri, 6);
        userStamina.text = StaminaData.Instance.GetStaminaFormat(StringUtils.ShortNumber(StaminaData.Instance.Stamina, 6));
    }
    public void OnClickSail()
    {
        mainCamera.transform.DOMove(sailCamera.transform.position, 1.0f).SetEase(Ease.InOutSine);
        mainCamera.DOOrthoSize(sailCamera.orthographicSize, 1.0f).SetEase(Ease.InOutSine);
        nodeMenu.SetActive(false);
        nodeSail.SetActive(true);
    }
    public void OnClickMenu()
    {
        mainCamera.transform.DOMove(camOriPos, 0.8f).SetEase(Ease.InOutSine);
        mainCamera.DOOrthoSize(camOriSize, 0.8f).SetEase(Ease.InOutSine);
        nodeMenu.SetActive(true);
        nodeSail.SetActive(false);
    }
    private void PresentShipWithLevel(int level)
    {
        int sailImgIdx = level + 1;
        int bodyImgIdx = level < 4 ? 1 : 2;
        Debug.Log("level: " + level);
        Debug.Log("sailImgIdx: " + sailImgIdx);
        Debug.Log("bodyImgIdx: " + bodyImgIdx);
        sail.sprite = Resources.Load<Sprite>("UI/UpgradeShip/sail_" + sailImgIdx);
        body.sprite = Resources.Load<Sprite>("UI/UpgradeShip/ship_" + bodyImgIdx);
    }
    private void UpdateAllStatus()
    {
        var config = GlobalConfigs.UpgradeShipConfig;
        int staminaMax = GlobalConfigs.StaminaConfig.max_stamina;
        int sailLevel = UserData.Instance.SailLevel;
        int maxSailLevel = config.GetMaxLevel();
        textSailLevel.text = "level " + (sailLevel + 1);
        textHelmLevel.text = "level " + (sailLevel + 1);
        textSail.text = "Sail level " + (sailLevel + 1);
        descSail.text = $"Stamina Cap: {staminaMax + config.GetStaminaCapacity(sailLevel)} ({staminaMax} + {config.GetStaminaCapacity(sailLevel)})"; 
        if (sailLevel >= maxSailLevel)
        {
            btnUpgradeSail.SetActive(false);
        }
        else
        {
            btnUpgradeSail.SetActive(true);
            textSailPrice.text = config.GetSailNextLevelPrice(sailLevel).ToString("N0");
        }
    }
    public void OnUpgadeSail()
    {
        UpgradeSailSuccess();
        return;
        var config = GlobalConfigs.UpgradeShipConfig;
        if (UserData.Instance.Beri < config.GetSailNextLevelPrice(UserData.Instance.SailLevel))
        {
            GuiManager.Instance.ShowPopupNotification("You do not have enough beri");
        }
        else
        {
            SceneTransition.Instance.ShowWaiting(true);
            NetworkController.Send(SFSAction.SAIL_UPGRADE);
        }
    }
    private void UpgradeSailSuccess()
    {
        if (!StaminaData.Instance.IsRecorveringStamina())
            StaminaData.Instance.LastCountStamina = GameTimeMgr.GetCurrentTime();

        UserData.Instance.SailLevel++;
        userStamina.text = StaminaData.Instance.GetStaminaFormat(StringUtils.ShortNumber(StaminaData.Instance.Stamina, 6));
        PresentShipWithLevel(UserData.Instance.SailLevel);
        UpdateAllStatus();

        sailEffect.transform.Find("sail").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("UI/UpgradeShip/sail_" + (UserData.Instance.SailLevel + 1));
        sailEffect.SetTrigger("Trigger");
    }
    public void OnUpgadeHelm()
    {
        GuiManager.Instance.ShowPopupNotification("Coming soon!");
    }
    public void ClickBack()
    {
        SceneManager.LoadScene("SceneLobby");
    }
    public void OnStaminaChanged(int oldValue, int newValue)
    {
        DoTweenUtils.UpdateNumber(userStamina, oldValue, newValue, x => StaminaData.Instance.GetStaminaFormat(StringUtils.ShortNumber(x, 6)));
    }
    private void OnBeriChanged(long oldValue, long newValue)
    {
        DoTweenUtils.UpdateNumber(userBeri, oldValue, newValue, x => StringUtils.ShortNumber(x, 6));
    }
}
