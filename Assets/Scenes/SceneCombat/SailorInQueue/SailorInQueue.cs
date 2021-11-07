using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SailorInQueue : MonoBehaviour
{
    private CombatSailor sailorData;
    public Image background;
    public SmoothSlider speedSlider;
    public Image icon;
    public Text level;
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
            icon.transform.localScale = new Vector3(1, 1, 1);
            background.color = Color.green;
        }
        else
        {
            icon.transform.localScale = new Vector3(-1, 1, 1);
            background.color = Color.red;
        }
        level.text = sailorData.Model.level.ToString();
    }
    public void OnClick()
    {
        TooltipSailorInfo.Instance.ShowTooltip(sailorData.gameObject, true);
    }
}
