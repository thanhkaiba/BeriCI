using Piratera.GUI;
using Piratera.Cheat;
using System.Collections;
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
    private SailorDescription sailorDes;
    [SerializeField]
    private Image quality,rank;
    [SerializeField]
    private Image [] classImgs;
    [SerializeField]
    private Transform sailorPos;

    private GameObject sailor;
    private List<IconSailor> listIcon;

    private SailorModel curModel;
    [SerializeField]
    private GameObject buttonCheat;
    // Start is called before the first frame update

    private void Start()
    {
#if PIRATERA_DEV
        buttonCheat.SetActive(true);
#else
        buttonCheat.SetActive(false);
#endif
    }

    private void OnEnable()
    {
        RenderListSubSailor();
    }

    void RenderListSubSailor()
    {
        sailors = CrewData.Instance.Sailors;
        listIcon = new List<IconSailor>();
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
        foreach (var item in sailorDes.sheets[0].list)
        {
            if(model.name == item.root_name)
            {
                texts[2].text = item.title;
                texts[8].text = item.skill_description;
            }             
        }
        quality.fillAmount = (float)model.quality / GlobalConfigs.SailorGeneral.MAX_QUALITY;
        texts[3].text = Mathf.Round(model.config_stats.GetPower(model.level, model.quality)).ToString();
        texts[4].text = Mathf.Round(model.config_stats.GetHealth(model.level, model.quality)).ToString();
        texts[5].text = Mathf.Round(model.config_stats.GetSpeed(model.level, model.quality)).ToString();
        texts[6].text = Mathf.Round(model.config_stats.GetArmor()).ToString();
        texts[7].text = Mathf.Round(model.config_stats.GetMagicResist()).ToString();
        if (sailor != null) Destroy(sailor);
        sailor = Instantiate(model.config_stats.model, sailorPos);
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
        qualityText.text = "Qua: " + model.quality + "/" + GlobalConfigs.SailorGeneral.MAX_QUALITY;
    }
    public void BackToLobby()
    {
        SceneManager.LoadScene("SceneLobby");
    }
    public void GoToLineup()
    {
        SceneManager.LoadScene("ScenePickTeam");
    }


    public void ShowCheatSailorInfo()
    {
#if PIRATERA_DEV
       PopupCheatSailorInfo popup = GuiManager.Instance.AddGui<PopupCheatSailorInfo>("Cheat/PopupCheatSailor").GetComponent<PopupCheatSailorInfo>();
       popup.sailorId = curModel.id; 
#endif
    }

}
