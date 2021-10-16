using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SailorInQueue : MonoBehaviour
{
    private Sailor sailorData;
    public Image background;
    public SmoothSlider speedSlider;
    public Image icon;
    public Text level;
    public void SetData(Sailor s)
    {
        sailorData = s;
        icon.sprite = s.config_stats.avatar;
        speedSlider.speed = 2.4f;
    }
    public Sailor GetData()
    {
        return sailorData;
    }
    public void PresentData()
    {
        float newValue = (float)sailorData.cs.CurrentSpeed / sailorData.cs.MaxSpeed;
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
        level.text = sailorData.level.ToString();
    }
    public void OnClick()
    {
        TooltipSailorInfo.Instance.ShowTooltip(sailorData.gameObject, true);
    }
}
