using UnityEngine;
using UnityEngine.UI;

public class CharBarControl : MonoBehaviour
{
    public SmoothSlider healthBar;
    public Text healthText;
    public Slider speedBar;
    public Text speedText;
    public SmoothSlider furyBar;
    public Text furyText;
    public Image iconType;
    public Image iconSkill;
    public Text textName, textLevel, textQuality;
    [SerializeField]
    private Sprite starSpr;
    [SerializeField]
    private Transform nodeStar;
    private void Awake()
    {
#if PIRATERA_DEV
        healthText.gameObject.SetActive(true);
#else
        healthText.gameObject.SetActive(false);
#endif
    }
    public void Init(SailorModel model)
    {
        for (int i = 0; i < model.star; i++)
        {
            GameObject starObj = new GameObject();
            Image starImg = starObj.AddComponent<Image>();
            starImg.sprite = starSpr;
            starObj.GetComponent<RectTransform>().SetParent(nodeStar);
            starObj.SetActive(true);
            starObj.transform.localScale = new Vector3(0.1f, 0.1f);
            starObj.transform.localPosition = new Vector3(i * 10, 0);
        }
        textLevel.text = "" + model.level;
        textQuality.text = "" + model.quality;
    }
    public void SetName(string name)
    {
        textName.text = name;
    }
    public void SetIconType(AttackType type)
    {
        iconType.sprite = Resources.Load<Sprite>("Icons/AttackType/" + type);
    }
    public void SetIconSkill(string skill_name)
    {
        if (skill_name != null)
        {
            //Debug.Log("SetIconSkill skill.name" + skill.name);
            iconSkill.sprite = Resources.Load<Sprite>("IconSkill/" + skill_name);
        }
        else iconSkill.sprite = Resources.Load<Sprite>("IconSkill/None");
    }
    public void SetHealthBar(float max, float min)
    {
        healthBar.ChangeValue(min / max);
#if PIRATERA_DEV
        healthText.text = $"{min}/{max}";
#else
        healthText.text = (System.Math.Ceiling(min)).ToString();
#endif
    }
    public void SetSpeedBar(int max, int min)
    {
        speedBar.value = (float)min / max;
        speedText.text = min + "/" + max;
    }
    public void SetFuryBar(int max, int min)
    {
        furyBar.gameObject.SetActive(max != 0);
        if (max != 0)
        {
            furyBar.ChangeValue((float)min / max);
            furyText.text = min + "/" + max;
        }
    }
}
