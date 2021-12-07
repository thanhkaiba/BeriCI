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
    public Transform sailorPos;
    private GameObject sailor;
    // Start is called before the first frame update

    private void OnEnable()
    {
        RenderListSubSailor();
    }

    void RenderListSubSailor()
    {
        sailors = CrewData.Instance.GetSubstituteSailors();
 
        for (int i = 0; i < sailors.Count; i++)
        {
            GameObject go = Instantiate(iconSailorPrefap, listSailors);
            go.GetComponent<IconSailor>().PresentData(sailors[i]);
            go.GetComponent<IconSailor>().crew = this;
        }
        SetData(sailors[0]);
    }

    public void SetData(SailorModel model) 
    {
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
        texts[3].text = model.config_stats.power_base.ToString();
        texts[4].text = model.config_stats.health_base.ToString();
        texts[5].text = model.config_stats.speed_base.ToString();
        texts[6].text = model.config_stats.armor.ToString();
        texts[7].text = model.config_stats.magic_resist.ToString();
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
        qualityText.text = "" + model.quality + "/" + GlobalConfigs.SailorGeneral.MAX_QUALITY;
    }
    public void BackToLobby()
    {
        SceneManager.LoadScene("SceneLobby");
    }
    public void GoToLineup()
    {
        SceneManager.LoadScene("ScenePickTeam");
    }
}
