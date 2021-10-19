using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    public SmoothSlider mainSlider;
    public SmoothSlider smoothSlider;
    public void SetValue(float val)
    {
        mainSlider.value = val;
        smoothSlider.value = val;
    }
    public void ChangeValue(float val)
    {
        if (mainSlider.value < val) mainSlider.ChangeValue(val);
        else mainSlider.SetValue(val);
        smoothSlider.ChangeValue(val);
    }
}