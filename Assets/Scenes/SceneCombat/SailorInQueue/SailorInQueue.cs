using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SailorInQueue : MonoBehaviour
{
    private Sailor sailorData;
    public Slider speedSlider;
    public Image icon;
    public void SetData(Sailor s)
    {
        sailorData = s;
        icon.sprite = Resources.Load<Sprite>("IconSailor/" + s.charName);
    }
    public Sailor GetData()
    {
        return sailorData;
    }
    public void PresentData()
    {
        speedSlider.value = (float)sailorData.cs.current_speed / sailorData.cs.max_speed;
        if (sailorData.cs.team == Team.B) icon.transform.localScale = new Vector3(-1, 1, 1);
    }
}
