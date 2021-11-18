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
    private Image background;
    [SerializeField]
    private Image backgroundLevel;
    [SerializeField]
    private GameObject sailorTypePrefap;
    [SerializeField]
    private GameObject nodeSailorClass;

  
    public void PresentData(SailorModel sailorModel)
    {
        gameObject.name = sailorModel.id;
        textLevel.text = "" + sailorModel.level;
        icon.sprite = sailorModel.config_stats.avatar;
        background.sprite = rankSprites[(int)sailorModel.config_stats.rank];

        foreach (Transform child in nodeSailorClass.transform)
        {
            Destroy(child.gameObject);
        }
        sailorModel.GetListClasses().ForEach((s => RenderClassIcon(s)));
        
    }

    public void SetVisible(bool visible)
    {
        textLevel.enabled = visible;
        icon.enabled = visible;
        background.enabled = visible;
        backgroundLevel.enabled = visible;
        nodeSailorClass.SetActive(visible);
    }

    private void RenderClassIcon(SailorClass sailorClass)
    {
        GameObject iconClass = Instantiate(sailorTypePrefap, nodeSailorClass.transform);
        iconClass.GetComponent<SmallSalorClass>().SetClass(sailorClass);
    }
}
