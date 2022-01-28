using Piratera.Cheat;
using Piratera.Config;
using Piratera.GUI;
using System.Collections.Generic;
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
        if (sailors.Count > 0)
        {
            SetData(sailors[0]);
            listIcon[0].ShowFocus(true);
        }
    }

    public void SetData(SailorModel model)
    {
        curModel = model;
        texts[0].text = model.name;
        texts[1].text = model.id;
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
        if (sailor != null) Destroy(sailor);
        sailor = Instantiate(GameUtils.GetSailorModelPrefab(model.config_stats.root_name), sailorPos);
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
        } else
        {
            expText.text = "" + model.exp + "/" + GlobalConfigs.SailorGeneral.GetNextLevelExp(model.level);
            exp.fillAmount = (float)model.exp / GlobalConfigs.SailorGeneral.GetNextLevelExp(model.level);
        }
        UpdateRankIconPosition();
    }
    public void BackToLobby()
    {
        SceneManager.LoadScene("SceneLobby");
    }
    public void GoToLineup()
    {
        SceneManager.LoadScene("ScenePickTeam");
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


    public void ShowCheatSailorInfo()
    {
#if PIRATERA_DEV || PIRATERA_QC
        PopupCheatSailorInfo popup = GuiManager.Instance.AddGui<PopupCheatSailorInfo>("Cheat/PopupCheatSailor").GetComponent<PopupCheatSailorInfo>();
        popup.sailorId = curModel.id;
#endif
    }

}
