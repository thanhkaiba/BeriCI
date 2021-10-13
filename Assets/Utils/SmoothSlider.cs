using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmoothSlider : Slider
{
    public float speed = 0.8f;
    float realValue = 0f;
    bool changed = false;

    public void ChangeValue(float val)
    {
        realValue = val;
        changed = true;
    }
    public void SetValue(float val)
    {
        value = val;
        realValue = val;
    }
    void LateUpdate()
    {
        if (!changed) return;
        value = Mathf.MoveTowards(value, realValue, speed * Time.deltaTime);
        if (value == realValue) changed = false;
    }
}
