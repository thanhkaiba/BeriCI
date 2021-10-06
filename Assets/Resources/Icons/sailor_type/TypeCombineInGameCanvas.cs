using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeCombineInGameCanvas : MonoBehaviour
{
    public Image icon;
    public Text text;

    public void ChangeIcon(SailorType type)
    {
        Debug.Log("ChangeIcon SailorType: " + type + "   |||| " + "Icons/sailor_type/type_" + type);
        icon.sprite = Resources.Load<Sprite>("Icons/sailor_type/" + type);
    }
    public void ChangeText(string _text)
    {
        text.text = _text;
    }
    public void ChangeLevel(int level)
    {
        // sau phai check theo tung loai
        if (level > 1) GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/sailor_type/border_0");
        else GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/sailor_type/border_1");
    }
}
