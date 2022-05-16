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
    private Sprite starSpr, starSprRed, starSprPurple;
    [SerializeField]
    private Transform nodeStar;
    [SerializeField]
    private Image iconAttackType;
    public void PresentData(SailorModel model)
    {
        level.text = "" + model.level;
        quality.text = "" + model.quality;
        qualitySlider.value = (float)model.quality / GlobalConfigs.SailorGeneral.MAX_QUALITY;

        ShowStar(model.star);
        iconAttackType.sprite = Resources.Load<Sprite>("Icons/AttackType/" + model.config_stats.attack_type);
    }
    private void ShowStar(int star)
    {
        if (star <= 5)
        {
            for (int i = 0; i < star; i++)
            {
                GameObject starObj = new GameObject();
                Image starImg = starObj.AddComponent<Image>();
                starImg.sprite = starSpr;
                starObj.GetComponent<RectTransform>().SetParent(nodeStar);
                starObj.SetActive(true);
                starObj.transform.localScale = new Vector3(0.1f, 0.1f);
                starObj.transform.localPosition = new Vector3((i - star / 2f + 0.5f) * 10, 0);

            }
        }
        else if (star <= 8)
        {
            for (int i = 5; i < star; i++)
            {
                GameObject starObj = new GameObject();
                Image starImg = starObj.AddComponent<Image>();
                starImg.sprite = starSprRed;
                starObj.GetComponent<RectTransform>().SetParent(nodeStar);
                starObj.SetActive(true);
                starObj.transform.localScale = new Vector3(0.15f, 0.15f);
                starObj.transform.localPosition = new Vector3(((i-5) - (star - 5) / 2f + 0.5f) * 14, 3);
            }
        }
        else
        {
            for (int i = 8; i < star; i++)
            {
                GameObject starObj = new GameObject();
                Image starImg = starObj.AddComponent<Image>();
                starImg.sprite = starSprPurple;
                starObj.GetComponent<RectTransform>().SetParent(nodeStar);
                starObj.SetActive(true);
                starObj.transform.localScale = new Vector3(0.2f, 0.2f);
                starObj.transform.localPosition = new Vector3(((i-8) - (star - 8) / 2f + 0.5f) * 18, 6);

            }
        }
    }
}
