using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TooltipSailorInfo : MonoBehaviour
{
    private Transform follow;
    private CombatStats stats;
    private SailorModel model;
    public static TooltipSailorInfo Instance;
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
    private RectTransform canvas;
    private RectTransform panel;

    public List<Image> iconItems;
    void Awake()
    {
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        panel = GetComponent<RectTransform>();
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
                 panel,
                 Input.mousePosition,
                 Camera.main))
            {
                gameObject.SetActive(false);
                follow = null;

            }
        }
        if (gameObject.activeSelf)
        {

            if (follow != null)
            {
                Vector3 pos = Camera.main.GetComponent<Camera>().WorldToScreenPoint(follow.position) + new Vector3(0, Screen.height / 10, 0);
                transform.position = pos;
                UpdateTooltipPos();
            }
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

    private void UpdateTooltipPos()
    {
        var sizeDelta = canvas.sizeDelta - panel.sizeDelta;
        var panelPivot = panel.pivot;
        var position = panel.anchoredPosition;
        position.x = Mathf.Clamp(position.x, -sizeDelta.x * panelPivot.x, sizeDelta.x * (1 - panelPivot.x));
        position.y = Mathf.Clamp(position.y, -sizeDelta.y * panelPivot.y, sizeDelta.y * (1 - panelPivot.y));
        panel.anchoredPosition = position;
    }

    public void ShowTooltip(GameObject sailorGO)
    {
        if (SceneManager.GetActiveScene().name == "SceneCombat2D")
        {
            CombatSailor sailor = sailorGO.GetComponent<CombatSailor>();
            ShowDynamicTooltip(sailor.Model, sailor.cs, sailorGO.transform.Find("nodeBar"));
        }
        else
        {
            model = sailorGO.GetComponent<Sailor>().Model;
            ShowTooltip(model, sailorGO.transform.Find("nodeBar"));
        }
    }

    public void ShowDynamicTooltip(SailorModel sailorModel, CombatStats cs, Transform target)
    {
        stats = cs;
        model = sailorModel;
        follow = target;
        ShowBasicInfo();
    }

    public void ShowTooltip(SailorModel sailorModel, Transform target)
    {
        model = sailorModel;
        follow = target;
        furySlider.transform.Find("Text").GetComponent<Text>().text = (model.config_stats.max_fury).ToString() + "/" + (model.config_stats.max_fury).ToString();
        furySlider.value = 1;
        healthSlider.transform.Find("Text").GetComponent<Text>().text = ((int)model.config_stats.health_base).ToString() + "/" + ((int)model.config_stats.health_base).ToString();
        healthSlider.value = 1;
        textPower.text = Mathf.Round(model.config_stats.GetPower(model.level, model.quality)).ToString();
        textSpeed.text = Mathf.Round(model.config_stats.GetSpeed(model.level, model.quality)).ToString();
        textArmor.text = Mathf.Round(model.config_stats.GetArmor()).ToString();
        textMagicResist.text = Mathf.Round(model.config_stats.GetMagicResist()).ToString();
        ShowBasicInfo();
    }

    public void ShowStaticTooltip(SailorModel sailorModel, Vector3 position)
    {
        ShowTooltip(sailorModel, null);
        transform.position = position + new Vector3(0, GetComponent<RectTransform>().sizeDelta.y / 2 * canvas.transform.localScale.x);
        UpdateTooltipPos();
    }

    public void ShowBasicInfo()
    {

        gameObject.SetActive(true);
        avtSailor.sprite = model.config_stats.avatar;
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
