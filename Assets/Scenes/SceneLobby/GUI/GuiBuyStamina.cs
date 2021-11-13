using UnityEngine;
using UnityEngine.UI;

public class GuiBuyStamina : MonoBehaviour
{
    [SerializeField]
    private Text textBeriCost;
    [SerializeField]
    private Text textStaminaValue;
    
   public void InitPackData()
    {
        UserStaminaConfig staminaConfig = UserData.Instance.StaminaConfig;
        textStaminaValue.text = "" + staminaConfig.statmina_buy_value;

        if (UserData.Instance.TimeBuyStaminaToday >= staminaConfig.costs.Length)
        {
            textStaminaValue.text = "OUT OF STOCK";
        } else
        {
            textBeriCost.text = "" + staminaConfig.costs[UserData.Instance.TimeBuyStaminaToday];
        }
    }

    private void Start()
    {
        InitPackData();
    }
}
