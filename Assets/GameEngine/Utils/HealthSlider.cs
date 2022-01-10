using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    public SmoothSlider mainSlider;
    public SmoothSlider smoothSlider;
    public Text Heath;
    private void Awake()
    {
#if !PIRATERA_DEV
        Heath.gameObject.SetActive(false);
#endif
    }
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