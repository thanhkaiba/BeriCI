using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TypeCombineInGameCanvas : MonoBehaviour
{
    public Image icon;
    public Text text;
    public PassiveType data;
    private void Start()
    {
        Button b = gameObject.AddComponent<Button>() as Button;
        b.onClick.AddListener(() => OnClickIcon(Input.mousePosition));
    }
    public void ChangeIcon(SailorType type)
    {
        //Debug.Log("ChangeIcon SailorType: " + type + "   |||| " + "Icons/sailor_type/type_" + type);
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
    public void SetData(PassiveType data)
    {
        this.data = data;
        ChangeIcon(data.type);
        ChangeLevel(data.level);
        ChangeText("");
    }
    private void OnClickIcon(Vector2 mousePos)
    {
        if (data == null) return;
        if (TooltipCombineType.Instance != null)
        {
            TooltipCombineType.Instance.ShowTooltipPassiveType(data, mousePos);
        }
    }
}
