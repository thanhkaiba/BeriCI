using Piratera.Cheat;
using Piratera.Config;
using Piratera.GUI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CrewManager : MonoBehaviour
{
    private List<SailorModel> sailors;
    [SerializeField]
    private GameObject iconSailorPrefap;
    [SerializeField]
    private Transform listSailors;
    [SerializeField]
    private Text[] texts; // 0 name 1 id 2 title 3 power 4 health 5 speed 6 armor 7 magic resist 8 des
    [SerializeField]
    private Text qualityText;
    [SerializeField]
    private Text levelText, expText;
    [SerializeField]
    private SailorDesc sailorDesc;
    [SerializeField]
    private Image quality, rank, exp;
    [SerializeField]
    private Image[] classImgs;
    [SerializeField]
    private Transform sailorPos;

    private GameObject sailor;
    private List<IconSailor> listIcon;

    private SailorModel curModel;
    [SerializeField]
    private GameObject buttonCheat;
    [SerializeField]
    private Slider furySlider;

    [SerializeField]
    private Text fightCount;
    [SerializeField]
    private Text EFRemain;
    [SerializeField]
    private Text TextTeamBonus;
    [SerializeField]
    private Text TextLockTrade;
    [SerializeField]
    private Toggle toggleShowInLineUp;

    private Transform canvas;

    // Start is called before the first frame update

    private void Start()
    {
        canvas = GameObject.Find("Canvas").transform;
        GameEvent.SailorInfoChanged.AddListener(OnSailorInfoChanged);
        RenderListSubSailor();
#if PIRATERA_DEV || PIRATERA_QC
        buttonCheat.SetActive(true);
#else
        buttonCheat.SetActive(false);
#endif
        if (TutorialMgr.Instance.CheckTutStartUp()) ShowCrewTut1();
    }

    private void OnSailorInfoChanged(SailorModel model)
    {
        if (curModel.id == model.id)
        {
            RenderListSubSailor();
            SetData(model);
        }
    }

    void RenderListSubSailor()
    {
        sailors = CrewData.Instance.Sailors;
        sailors.Sort();
        sailors.Reverse();
        listIcon = new List<IconSailor>();

        foreach (Transform child in listSailors)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < sailors.Count; i++)
        {
            GameObject go = Instantiate(iconSailorPrefap, listSailors);
            IconSailor icon = go.GetComponent<IconSailor>();
            icon.OnClick = model =>
            {
                listIcon.ForEach(_icon => _icon.ShowFocus(false));
                icon.ShowFocus(true);
                SetData(model);
            };
            icon.ShowRank = true;
            icon.PresentData(sailors[i]);
            listIcon.Add(icon);
        }
        if (sailors.Count > 0 && curModel == null)
        {
            SetData(sailors[0]);
            listIcon[0].ShowFocus(true);
        }
    }
    private void FocusSailor(SailorModel s)
    {
        listIcon.ForEach(_icon => _icon.ShowFocus(s == _icon.sailorModel));
    }
    public void SetData(SailorModel model)
    {
        curModel = model;
        texts[0].text = model.name;
        texts[1].text = "#" + model.id.PadLeft(6, '0');
        foreach (var item in sailorDesc.list)
        {
            if (model.name == item.root_name)
            {
                texts[2].text = item.title;
                texts[8].text = GameUtils.GetTextDescription(item.skill_description, model);
            }
        }

        quality.fillAmount = (float)model.quality / GlobalConfigs.SailorGeneral.MAX_QUALITY;
        texts[3].text = Mathf.Round(model.config_stats.GetPower(model.level, model.quality, model.star)).ToString();
        texts[4].text = Mathf.Round(model.config_stats.GetHealth(model.level, model.quality, model.star)).ToString();
        texts[5].text = Mathf.Round(model.config_stats.GetSpeed(model.level, model.quality)).ToString();
        texts[6].text = Mathf.Round(model.config_stats.GetArmor()).ToString();
        texts[7].text = Mathf.Round(model.config_stats.GetMagicResist()).ToString();
        fightCount.text = "Fight: <color=#CCCF44>"+ model.pve_count +"</color>";
        bool isTrial = model.id.StartsWith("trial-");
        var maxEF = isTrial ? GlobalConfigs.SailorGeneral.TRIAL_EARNABLE_FIGHT : GlobalConfigs.SailorGeneral.EARNABLE_FIGHT;
        var EF_remain = (maxEF - model.pve_count);
        EFRemain.text = "EF(*) Remain: <color=#F5FF17>"
            + (EF_remain < 0 ? 0 : EF_remain)
            +"</color>";
        var teamBonus = GlobalConfigs.PvE.sailor_rank_bonus[(int)model.config_stats.rank] * Mathf.Pow(2, model.star);
        TextTeamBonus.text = "(*) Your \"Team Bonus\" increase <color=#e6e9ff>" + teamBonus + "</color> and this sailor lose 1 EF when you win.";
        if (!model.IsAvaiable())
        {
            TextLockTrade.gameObject.SetActive(true);
            TextLockTrade.text = "Sailor is locked for " + System.MathF.Ceiling(model.GetRemainingLockTime() / (60f * 60 * 1000)) + " hour(s) because it's been trade recently.";
        }
        else if (isTrial)
        {
            TextLockTrade.gameObject.SetActive(true);
            TextLockTrade.text = "This is trial sailors. He will leave you after <b><color=#f2b5b1>" + (GlobalConfigs.SailorGeneral.TRIAL_FIGHT - model.pve_count) + "</color></b> fight(s).";
        }
        else
        {
            TextLockTrade.gameObject.SetActive(false);
        }
        if (sailor != null) Destroy(sailor);
        sailor = Instantiate(GameUtils.GetSailorModelPrefab(model.config_stats.root_name), sailorPos);
        if (model.config_stats.root_name == "FatBrakes")
        {
            sailor.transform.localPosition = new Vector3(0, 1.4f, 0);
            sailor.transform.localScale = Vector3.one * 0.8f;
        }
        rank.sprite = Resources.Load<Sprite>("Icons/IconRank/" + model.config_stats.rank.ToString());
        int classCount = model.config_stats.classes.Count - classImgs.Length;
        for (int i = 0; i < classImgs.Length; i++)
        {
            if (i > classCount - 1) classImgs[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < model.config_stats.classes.Count; i++)
        {
            classImgs[i].gameObject.SetActive(true);
            Sprite s = Resources.Load<Sprite>("Icons/SailorType/" + model.config_stats.classes[i]);
            classImgs[i].sprite = s;
            //classImgs[i].rectTransform.sizeDelta = new Vector2(s.rect.width, s.rect.height);
        }
        qualityText.text = "Quality: " + model.quality + "/" + GlobalConfigs.SailorGeneral.MAX_QUALITY;
        levelText.text = "" + model.level;
        if (model.level >= GlobalConfigs.SailorGeneral.MAX_LEVEL)
        {
            expText.text = "MAX";
            exp.fillAmount = 1;
        }
        else
        {
            expText.text = "" + model.exp + "/" + GlobalConfigs.SailorGeneral.GetNextLevelExp(model.level);
            exp.fillAmount = (float)model.exp / GlobalConfigs.SailorGeneral.GetNextLevelExp(model.level);
        }
        if (model.config_stats.max_fury > 0)
        {
            furySlider.gameObject.SetActive(true);
            var textFury = "Mana: " + (model.config_stats.max_fury).ToString();
            if (model.config_stats.start_fury > 0) textFury += " (" + (model.config_stats.start_fury).ToString() + ")";
            furySlider.transform.Find("Text").GetComponent<Text>().text = textFury;
            furySlider.value = (float)model.config_stats.start_fury / model.config_stats.max_fury;
        }
        else furySlider.gameObject.SetActive(false);
        UpdateRankIconPosition();
        toggleShowInLineUp.isOn = GameUtils.IsSailorFavorite(model.id);
    }
    public void BackToLobby()
    {
        SceneManager.LoadScene("SceneLobby");
    }
    public void GoToLineup()
    {
        SceneManager.LoadScene("ScenePickTeam");
    }
    public void RunSceneCraft()
    {
        SceneManager.LoadScene("SceneCraft");
    }

    public void UpdateRankIconPosition()
    {
        BoxCollider2D box = sailor.GetComponent<BoxCollider2D>();
        Vector2 pos = Camera.main.WorldToScreenPoint(sailor.transform.position - new Vector3(box.size.x / 4 - box.offset.x, -box.size.y, 0));
        rank.transform.position = new Vector3(pos.x - 10 * canvas.localScale.x, pos.y, rank.transform.position.z);

        if (curModel.config_stats.root_name == "Salvatafo")
        {
            rank.transform.position += new Vector3(0, 65 * canvas.localScale.x);
        }

    }
    private void ShowCrewTut1()
    {
        var go = Resources.Load<GameObject>("Prefap/Tuts/CrewTut1");
        GameObject tut = Instantiate(go, GuiManager.Instance.GetCanvas().transform);
    }
    public void ShowTutOpenLineUp()
    {
        var go = Resources.Load<GameObject>("Prefap/Tuts/hand");
        GameObject hand = Instantiate(go, GuiManager.Instance.GetCanvas().transform);
        var pos = GameObject.Find("ButtonLineUp").transform.position;
        hand.transform.position = new Vector3(pos.x, pos.y, pos.z);
    }

    public void ShowCheatSailorInfo()
    {
#if PIRATERA_DEV || PIRATERA_QC
        PopupCheatSailorInfo popup = GuiManager.Instance.AddGui("Cheat/PopupCheatSailor").GetComponent<PopupCheatSailorInfo>();
        popup.sailorId = curModel.id;
#endif
    }
    public void SortListSailor()
    {
        RenderListSubSailor();
        FocusSailor(curModel);
    }
    public void OnToggleChange()
    {
        toggleShowInLineUp.isOn = !toggleShowInLineUp.isOn;
        GameUtils.ToggleFavoriteSailor(curModel.id, toggleShowInLineUp.isOn);
    }
}

//int sortType = UserData.Instance.ChangeSortType();
//var text = GameObject.Find("textSort").GetComponent<Text>();

//switch (sortType)
//{
//    case 0:
//        text.text = "Rank";
//        break;
//    case 1:
//        text.text = "Star";
//        break;
//    default:
//        text.text = "Level";
//        break;
//}
//PlayerPrefs.SetInt("sort_type", sortType);
//RenderListSubSailor();