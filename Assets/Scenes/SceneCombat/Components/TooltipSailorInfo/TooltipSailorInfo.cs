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
    private SailorDesc sailorDesc;
    public Slider healthSlider;
    public Image shield;
    public Slider furySlider;
    public Slider qualityBar;

    public Text textLevel;
    public Text textPower;
    public Text textSpeed;
    public Text textArmor;
    public Text textMagicResist;
    public Text textName;
    public Text textDes;
    private RectTransform canvas;
    private RectTransform panel;

    public List<Image> iconItems;
    void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
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
                Vector3 pos = Camera.main.GetComponent<Camera>().WorldToScreenPoint(follow.position) + new Vector3(0, Screen.height / 7.5f, 0);
                transform.position = pos;
                UpdateTooltipPos();
            }
            if (SceneManager.GetActiveScene().name != "SceneCombat2D")
                return;
            ShowStatsInfo(stats, model);
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
            var model = sailorGO.GetComponent<Sailor>().Model;
            ShowTooltip(model, sailorGO.transform.Find("nodeBar"));
        }
    }

    public void ShowDynamicTooltip(SailorModel model, CombatStats cs, Transform target)
    {
        stats = cs;
        this.model = model;
        follow = target;
        ShowUnchangeInfo(model);
    }

    public void ShowTooltip(SailorModel model, Transform target)
    {
        follow = target;
        CombatStats cs = new CombatStats(model);
        if (SceneManager.GetActiveScene().name == "ScenePickTeam")
        {
            cs.UpdateStatsWithSynergy(GameUtils.lineUpSynergy);
        }
        else if (SceneManager.GetActiveScene().name == "ScenePickTeamBattle")
        {
            var myKey = TeamCombatPrepareData.Instance.YourFightingLine.SlotOf(model.id);
            if (myKey != -1) cs.UpdateStatsWithSynergy(GameUtils.lineUpSynergy, GameUtils.oppLineUpSynergy);
            else cs.UpdateStatsWithSynergy(GameUtils.oppLineUpSynergy, GameUtils.lineUpSynergy);
        }
        ShowStatsInfo(cs, model);
        ShowUnchangeInfo(model);
    }

    public void ShowStaticTooltip(SailorModel sailorModel, Vector3 position)
    {
        ShowTooltip(sailorModel, null);
        transform.position = position + new Vector3(0, GetComponent<RectTransform>().sizeDelta.y / 2.4f * canvas.transform.localScale.x);
        UpdateTooltipPos();
    }
    public void ShowStatsInfo(CombatStats cs, SailorModel model)
    {
        if (cs.MaxFury > 0)
        {
            furySlider.gameObject.SetActive(true);
            furySlider.transform.Find("Text").GetComponent<Text>().text = (cs.Fury).ToString() + "/" + (cs.MaxFury).ToString();
            furySlider.value = (float)cs.Fury / cs.MaxFury;
        }
        else furySlider.gameObject.SetActive(false);


        float maxWidth = healthSlider.GetComponent<Image>().rectTransform.sizeDelta.x;
        float realMax = Mathf.Max(cs.MaxHealth, cs.CurHealth + cs.Shield);
        healthSlider.value = cs.CurHealth / realMax;
        float curY = shield.rectTransform.sizeDelta.y;
        shield.rectTransform.sizeDelta = new Vector2(cs.Shield / realMax * maxWidth, curY);

        healthSlider.transform.Find("Text").GetComponent<Text>().text =
            Mathf.Round(cs.CurHealth).ToString()
            + "/" + Mathf.Round(cs.MaxHealth).ToString()
            + (cs.Shield > 0.5f ? " | " + Mathf.Round(cs.Shield).ToString() : "");
        
        textPower.text = Mathf.Round(cs.Power).ToString();
        textSpeed.text = Mathf.Round(cs.Speed).ToString();
        textArmor.text = Mathf.Round(cs.Armor).ToString();
        textMagicResist.text = Mathf.Round(cs.MagicResist).ToString();

        foreach (var item in sailorDesc.list)
        {
            if (model.name == item.root_name)
            {
                textDes.text = GameUtils.GetTextDescription(item.skill_description, model, cs);
            }
        }
        qualityBar.value = (float)model.quality / 200;
    }
    public void ShowUnchangeInfo(SailorModel model)
    {
        gameObject.SetActive(true);
        avtSailor.sprite = GameUtils.GetSailorAvt(model.config_stats.root_name);
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
        foreach (var item in sailorDesc.list)
        {
            if (model.name == item.root_name)
            {
                textName.text = item.present_name;
            }
        }
        qualityBar.value = (float)model.quality / 200;
    }
}
