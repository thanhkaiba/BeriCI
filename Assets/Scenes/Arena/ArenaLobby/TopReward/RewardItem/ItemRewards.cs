using Piratera.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemRewards : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text title, quantity;
    [SerializeField]
    private GameObject focusing;
    public void ShowBeri(int _quantity, string _title = "")
    {
        if (_quantity >= 100000) quantity.text = StringUtils.ShortNumber(_quantity);
        else quantity.text = _quantity.ToString("N0");
        title.text = _title;
    }
    public void ShowAvatar(int id)
    {
        icon.sprite = Resources.Load<Sprite>("Icons/Avatar/" + id);
    }
    public void ShowSailorPiece(string root_name, int _quantity)
    {
        icon.sprite = GameUtils.GetSailorAvt(root_name);
        quantity.text = "x" + _quantity;
    }
    public void ShowPosterPiece(string posterType, int _quantity)
    {
        icon.sprite = Resources.Load<Sprite>("Icons/Poster/" + posterType);
        quantity.text = "x" + _quantity;
    }
    // craft
    public void ShowSailorPiece(string root_name, string text, Color32 color)
    {
        icon.sprite = GameUtils.GetSailorAvt(root_name);
        quantity.text = text;
        quantity.color = color;
    }
    public void ShowPosterPiece(string posterType, string text, Color32 color)
    {
        icon.sprite = Resources.Load<Sprite>("Icons/Poster/" + posterType);
        quantity.text = text;
        quantity.color = color;
    }
    public void ShowInfocus(bool a = true)
    {
        focusing.SetActive(a);
    }
    public void SetQuantity(string t)
    {
        quantity.text = t;
    }
}
