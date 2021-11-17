using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSailor : MonoBehaviour
{
    [SerializeField]
    private Text textLevel;
    [SerializeField]
    private Sprite[] rankSprites;
    [SerializeField]
    public Image icon;
    [SerializeField]
    public Image background;
    [SerializeField]
    public Image backgroundLevel;

    private void Start()
    {
    }


    public void PresentData(SailorModel sailorModel)
    {
        gameObject.name = sailorModel.id;
        textLevel.text = "" + sailorModel.level;
        icon.sprite = sailorModel.config_stats.avatar;
        background.sprite = rankSprites[(int)sailorModel.config_stats.rank];
    }

    public void SetVisible(bool visible)
    {
        textLevel.enabled = visible;
        icon.enabled = visible;
        background.enabled = visible;
        backgroundLevel.enabled = visible;
    }
}
