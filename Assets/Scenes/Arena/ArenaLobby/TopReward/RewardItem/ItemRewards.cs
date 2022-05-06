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
    public void ShowBeri(int quanity)
    {
        quantity.text = StringUtils.ShortNumber(quanity);
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
}