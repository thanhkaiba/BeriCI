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
    private Transform list;
    [SerializeField]
    private Text[] texts;
    [SerializeField]
    private SailorDescription sailorDes;
    [SerializeField]
    private Image img;
    // Start is called before the first frame update
    void Start()
    {
        RenderListSubSailor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RenderListSubSailor()
    {
        sailors = CrewData.Instance.GetSubstituteSailors();
        Debug.LogError(sailors);

        for (int i = 0; i < sailors.Count; i++)
        {


            
           Instantiate(iconSailorPrefap, list);
                //subSailorIcon = imgObject.AddComponent<SubSailorIcon>();
    

            
 
        }
        SetData(sailors[0]);
    }


    void SetData(SailorModel model) 
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
 
        texts[3].text = model.config_stats.power_base.ToString();
        texts[4].text = model.config_stats.health_base.ToString();
        texts[5].text = model.config_stats.speed_base.ToString();
        texts[6].text = model.config_stats.armor.ToString();
        texts[7].text = model.config_stats.magic_resist.ToString();
        img.sprite = model.img;



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
