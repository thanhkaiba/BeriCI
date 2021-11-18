using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallSalorClass : MonoBehaviour
{
    [SerializeField]
    private Image icon;


    public void SetClass(SailorClass sailorClass)
    {
        Sprite sprite = Resources.Load<Sprite>($"Icons/SailorType/small/{sailorClass}");
        icon.sprite = sprite;
        icon.SetNativeSize();
    }

}
