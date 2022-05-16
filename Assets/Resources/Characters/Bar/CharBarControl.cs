using UnityEngine;
using UnityEngine.UI;

public class CharBarControl : MonoBehaviour
{
    public SmoothSlider healthBar;
    public Image fakeBloodBar;
    public Text healthText;
    public Slider speedBar;
    public Text speedText;
    public SmoothSlider furyBar;
    public Text furyText;
    public Image iconType;
    public Image iconSkill;
    public Text textName, textLevel, textQuality;
    [SerializeField]
    private Sprite starSpr, starSprRed, starSprPurple;
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
        ShowStar(model.star);
        textLevel.text = "" + model.level;
        textQuality.text = "" + model.quality;
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
                starObj.transform.localPosition = new Vector3(i * 10, 0);
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
                starObj.transform.localPosition = new Vector3((i-5) * 14 + 2, 2);
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
                starObj.transform.localPosition = new Vector3((i-8) * 18 + 3, 3);
            }
        }
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
    public void SetHealthBar(float max, float min, float fakeBlood = 0)
    {
        float maxWidth = healthBar.GetComponent<Image>().rectTransform.sizeDelta.x;
        float realMax = Mathf.Max(max, min + fakeBlood);
        
        if (max < min + fakeBlood) healthBar.SetValue(min / realMax);
        else healthBar.ChangeValue(min / realMax);

        float curY = fakeBloodBar.rectTransform.sizeDelta.y;
        fakeBloodBar.rectTransform.sizeDelta = new Vector2(fakeBlood / realMax * maxWidth, curY);


#if PIRATERA_DEV
        healthText.text = $"{System.Math.Ceiling(min)}/{System.Math.Ceiling(max)}";
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
