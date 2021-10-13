using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SailorInQueue : MonoBehaviour
{
    private Sailor sailorData;
    public SmoothSlider speedSlider;
    public Image icon;
    public Text level;
    public void SetData(Sailor s)
    {
        sailorData = s;
        icon.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + s.charName);
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
        if (sailorData.cs.team == Team.B) icon.transform.localScale = new Vector3(-1, 1, 1);
        level.text = sailorData.level.ToString();
    }
}
