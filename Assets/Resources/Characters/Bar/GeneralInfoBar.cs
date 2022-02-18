using Piratera.Config;
using UnityEngine;
using UnityEngine.UI;

public class GeneralInfoBar : MonoBehaviour
{
    [SerializeField]
    private Text level;
    [SerializeField]
    private Text quality;
    [SerializeField]
    private Slider qualitySlider;
    [SerializeField]
    private Sprite starSpr;
    [SerializeField]
    private Transform nodeStar;
    [SerializeField]
    private Image iconAttackType;
    public void PresentData(SailorModel model)
    {
        level.text = "" + model.level;
        quality.text = "" + model.quality;
        qualitySlider.value = (float)model.quality / GlobalConfigs.SailorGeneral.MAX_QUALITY;

        for (int i = 0; i < model.star; i++)
        {
            GameObject starObj = new GameObject();
            Image starImg = starObj.AddComponent<Image>();
            starImg.sprite = starSpr;
            starObj.GetComponent<RectTransform>().SetParent(nodeStar);
            starObj.SetActive(true);
            starObj.transform.localScale = new Vector3(0.1f, 0.1f);
            starObj.transform.localPosition = new Vector3((i - (float)model.star / 2 + 0.5f) * 10, 0);
        }
        iconAttackType.sprite = Resources.Load<Sprite>("Icons/AttackType/" + model.config_stats.attack_type);
    }
}
