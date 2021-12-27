using UnityEngine;
using UnityEngine.UI;

public class SailorInQueue : MonoBehaviour
{
    private CombatSailor sailorData;
    public Image background;
    public SmoothSlider speedSlider;
    public Image icon;
    public Text level;
    public Image colorTeam;
    public void SetData(CombatSailor s)
    {
        sailorData = s;
        icon.sprite = s.Model.config_stats.avatar;
        speedSlider.speed = 2.4f;
    }
    public CombatSailor GetData()
    {
        return sailorData;
    }
    public void PresentData()
    {
        float newValue = (float)sailorData.cs.CurrentSpeed / sailorData.cs.SpeedNeed;
        if (speedSlider.value < newValue) speedSlider.ChangeValue(newValue);
        else speedSlider.SetValue(newValue);
        if (sailorData.cs.team == Team.A)
        {
            icon.transform.localScale = new Vector3(-1, 1, 1);
            colorTeam.color = new Color(103f/255f, 169f/255f, 255f/255f);
        }
        else
        {
            icon.transform.localScale = new Vector3(1, 1, 1);
            colorTeam.color = new Color(217f/255f, 88f/255f, 97f/255f);
        }
        level.text = sailorData.Model.level.ToString();
    }
    public void OnClick()
    {
        TooltipSailorInfo.Instance.ShowTooltip(sailorData.gameObject);
    }
}
