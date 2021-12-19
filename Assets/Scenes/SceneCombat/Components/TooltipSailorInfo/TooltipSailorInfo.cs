using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TooltipSailorInfo : MonoBehaviour
{
    private Transform follow;
    private CombatStats stats;
    private Sailor sailorPrepe;
    private SailorModel model;
    public static TooltipSailorInfo Instance;
    private bool justOpen = false;
    [SerializeField]
    private Color[] rankColorInside;
    public Image avtSailor;
    public List<Image> iconTypes;
    public Image iconAttackType, background;
    [SerializeField]
    private SailorDescription sailorDes;
    public Slider healthSlider;
    public Slider furySlider;
    public Slider qualityBar;

    public Text textLevel;
    public Text textPower;
    public Text textSpeed;
    public Text textArmor;
    public Text textMagicResist;
    public Text textDes;

    public List<Image> iconItems;
    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        Instance = null;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButton(0) && gameObject.activeSelf &&
             !RectTransformUtility.RectangleContainsScreenPoint(
                 gameObject.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 Camera.main))
            {
                if (!justOpen) gameObject.SetActive(false);
                else justOpen = false;

            }
        }
        if (gameObject.activeSelf)
        {
            Vector3 pos = Camera.main.GetComponent<Camera>().WorldToScreenPoint(follow.transform.position) + new Vector3(0, Screen.height/10, 0);
            transform.position = pos;
            if (SceneManager.GetActiveScene().name != "SceneCombat2D")
                return;
            textPower.text = ((int)stats.Power).ToString();
            textSpeed.text = ((int)stats.Speed).ToString();
            textArmor.text = ((int)stats.Armor).ToString();
            textMagicResist.text = ((int)stats.MagicResist).ToString();

            healthSlider.value = stats.CurHealth / stats.MaxHealth;
            healthSlider.transform.Find("Text").GetComponent<Text>().text = ((int)stats.CurHealth).ToString() + "/" + ((int)stats.MaxHealth).ToString();
            if (stats.MaxFury == 0) furySlider.value = 1;
            else furySlider.value = (float)stats.Fury / (float)stats.MaxFury;
            furySlider.transform.Find("Text").GetComponent<Text>().text = (stats.Fury).ToString() + "/" + (stats.MaxFury).ToString();
        }
    }
    public void ShowTooltip(GameObject sailorGO, bool clickFromUI = false)
    {
        if (SceneManager.GetActiveScene().name == "SceneCombat2D")
        {
            var sailor = sailorGO.GetComponent<CombatSailor>();
            stats = sailor.cs;
            model = sailor.Model;
        }
        else
        {
            sailorPrepe = sailorGO.GetComponent<Sailor>();
            model = sailorPrepe.Model;
            furySlider.transform.Find("Text").GetComponent<Text>().text = (model.config_stats.max_fury).ToString() + "/" + (model.config_stats.max_fury).ToString();
            furySlider.value = 1;
            healthSlider.transform.Find("Text").GetComponent<Text>().text = ((int)model.config_stats.health_base).ToString() + "/" + ((int)model.config_stats.health_base).ToString();
            healthSlider.value = 1;
            textPower.text =  Mathf.Round(model.config_stats.GetPower(model.level , model.quality)).ToString();
            textSpeed.text = Mathf.Round(model.config_stats.GetSpeed(model.level, model.quality)).ToString();
            textArmor.text = Mathf.Round(model.config_stats.GetArmor()).ToString();
            textMagicResist.text = Mathf.Round(model.config_stats.GetMagicResist()).ToString();

        }
        follow = sailorGO.transform.Find("nodeBar");
        gameObject.SetActive(true);
        justOpen = !clickFromUI;
        avtSailor.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + model.config_stats.root_name);
        background.color = rankColorInside[(int)model.config_stats.rank];
        var listType = model.config_stats.classes;
        for (int i = 0; i < 3; i++)
        {
            if (listType.Count > i)
            {
                iconTypes[i].gameObject.SetActive(true);
                iconTypes[i].sprite = Resources.Load<Sprite>("Icons/SailorType/" + listType[i]);
            }
            else iconTypes[i].gameObject.SetActive(false);
        }
        var attackType = model.config_stats.attack_type;
        iconAttackType.sprite = Resources.Load<Sprite>("Icons/AttackType/" + attackType);
        textLevel.text = model.level.ToString();
        foreach (var item in sailorDes.sheets[0].list)
        {
            if (model.name == item.root_name)
            {
                textDes.text = item.skill_description;
            }
        }
        qualityBar.value = (float)model.quality / 200;
    }
}
