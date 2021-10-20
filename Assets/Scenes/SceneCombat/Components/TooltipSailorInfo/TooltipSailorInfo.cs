using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipSailorInfo : MonoBehaviour
{
    private Transform follow;
    private CombatSailor sailor;
    public static TooltipSailorInfo Instance;
    private bool justOpen = false;

    public Image avtSailor;
    public List<Image> iconTypes;
    public Image iconAttackType;

    public Slider healthSlider;
    public Slider furySlider;
    public Slider qualityBar;

    public Text textLevel;
    public Text textPower;
    public Text textCrit;
    public Text textSpeed;
    public Text textArmor;
    public Text textMagicResist;

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
            Rect size = GetComponent<RectTransform>().rect;
            Vector2 posInImage = transform.position - Input.mousePosition;
            if (posInImage.x >= -size.width / 2 && posInImage.x < size.width / 2 &&
                 posInImage.y >= -size.height / 2 && posInImage.y < size.height / 2)
            {
                // click inside
            }
            else if (!justOpen) gameObject.SetActive(false);
            else justOpen = false;
        }
        if (gameObject.activeSelf)
        {
            Vector3 pos = Camera.main.GetComponent<Camera>().WorldToScreenPoint(follow.transform.position) + new Vector3(0, 110, 0);

            float width = GetComponent<RectTransform>().rect.width;
            float height = GetComponent<RectTransform>().rect.height;
            if (pos.x < 20 + width / 2) pos.x = 20 + width / 2;
            if (pos.x > Screen.width - 20 - width / 2) pos.x = Screen.width - 20 - width / 2;
            if (pos.y < 20 + height / 2) pos.y = 20 + height / 2;
            if (pos.y > Screen.height - 20 - height / 2) pos.y = Screen.height - 20 - height / 2;
            transform.position = pos;

            textPower.text = ((int) sailor.cs.Power).ToString();
            textCrit.text = (sailor.cs.Crit*100).ToString() + "%";
            textSpeed.text = ((int)sailor.cs.DisplaySpeed).ToString();
            textArmor.text = ((int)sailor.cs.Armor).ToString();
            textMagicResist.text = ((int)sailor.cs.MagicResist).ToString();

            healthSlider.value = sailor.cs.CurHealth / sailor.cs.MaxHealth;
            healthSlider.transform.Find("Text").GetComponent<Text>().text = ((int)sailor.cs.CurHealth).ToString() + "/" + ((int)sailor.cs.MaxHealth).ToString();
            if (sailor.cs.MaxFury == 0) furySlider.value = 1;
            else furySlider.value = (float)sailor.cs.Fury / (float)sailor.cs.MaxFury;
            furySlider.transform.Find("Text").GetComponent<Text>().text = (sailor.cs.Fury).ToString() + "/" + (sailor.cs.MaxFury).ToString();
        }
    }
    public void ShowTooltip(GameObject sailorGO, bool clickFromUI = false)
    {
        //if (gameObject.activeSelf) return;
        sailor = sailorGO.GetComponent<CombatSailor>();
        follow = sailorGO.transform.Find("nodeBar");
        gameObject.SetActive(true);
        justOpen = !clickFromUI;

        avtSailor.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + sailor.config_stats.root_name);
        var listType = sailor.config_stats.classes;
        for (int i = 0; i < 3; i++)
        {
            if (listType.Count > i)
            {
                iconTypes[i].gameObject.SetActive(true);
                iconTypes[i].sprite = Resources.Load<Sprite>("Icons/SailorType/" + listType[i]);
            }
            else iconTypes[i].gameObject.SetActive(false);
        }
        var attackType = sailor.config_stats.attack_type;
        iconAttackType.sprite = Resources.Load<Sprite>("Icons/AttackType/" + attackType);
        textLevel.text = sailor.level.ToString();

        qualityBar.value = (float) sailor.quality / 200;
    }
}
