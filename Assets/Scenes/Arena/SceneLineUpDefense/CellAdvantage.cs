using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellAdvantage : MonoBehaviour
{
    [SerializeField]
    private Image icon, bg;
    [SerializeField]
    private Text title;
    [SerializeField]
    private Text description;
    [SerializeField]
    private GameObject iconLock;
    public void SetType(HomefieldAdvantage type)
    {
        title.text = GameUtils.GetHomeAdvantageStr(type);
        description.text = GameUtils.GetHomeAdvantageDesc(type);
        var data = PvPData.Instance.OpenedAdvantage;
        iconLock.SetActive(!data.Contains(type));
        icon.sprite = Resources.Load<Sprite>("UI/Arena/advantage/ad_" + type.ToString());
        byte opacity = (byte)(data.Contains(type) ? 255 : 150);
        icon.color = new Color32(255, 255, 225, opacity);
        if (!data.Contains(type))
            bg.sprite = Resources.Load<Sprite>("UI/Arena/advantage/cell_lock");
        else if (PvPData.Instance.SelectingAdvantage != type)
            bg.sprite = Resources.Load<Sprite>("UI/Arena/advantage/cell_opened");
        else
            bg.sprite = Resources.Load<Sprite>("UI/Arena/advantage/cell_selecting");
    }
}
