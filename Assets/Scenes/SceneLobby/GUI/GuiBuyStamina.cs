using System;
using UnityEngine;
using UnityEngine.UI;

public class GuiBuyStamina : MonoBehaviour
{
    [SerializeField]
    private Text textBeriCost;
    [SerializeField]
    private Text textStaminaValue;
    [SerializeField]
    private Text textCountDownStamina;
    [SerializeField]
    private Text textCurrentStamina;
    [SerializeField]
    private Button buttonBuy;
 

    public void InitPackData()
    {
        UserStaminaConfig staminaConfig = UserData.Instance.StaminaConfig;
        textStaminaValue.text = "" + staminaConfig.statmina_buy_value;
        textBeriCost.text = "" + staminaConfig.costs[UserData.Instance.TimeBuyStaminaToday];
        EnableButtonBuy(UserData.Instance.TimeBuyStaminaToday < staminaConfig.costs.Length - 1);
        textCurrentStamina.text = UserData.Instance.GetCurrentStaminaFormat();
    }

    private void EnableButtonBuy(bool enabled)
    {
        buttonBuy.interactable = enabled;
        int children = buttonBuy.transform.childCount;
        for (int i = 0; i < children; ++i)
        {
            Transform child = buttonBuy.transform.GetChild(i);
            if (child.gameObject.name == "TextLimited")
            {
                child.gameObject.SetActive(!enabled);
            } else
            {
                child.gameObject.SetActive(enabled);
            }
        }

    }

    private void Start()
    {
        InitPackData();
    }

    public void OnClose()
    {
        Destroy(gameObject);
    }

    public void OnBuyStamina()
    {
        NetworkController.Send(SFSAction.BUY_STAMINA);
        OnClose();
    }

    void Update()
    {
        
        if (UserData.Instance.IsRecorveringStamina())
        {
            TimeSpan remaining = TimeSpan.FromMilliseconds(UserData.Instance.TimeToHaveNewStamina());
            textCountDownStamina.text = string.Format("{0:00}:{1:00}:{2:00}", remaining.Hours, remaining.Minutes, remaining.Seconds);
        }
        else
        {
            textCountDownStamina.text = "";

        }
    }
}
