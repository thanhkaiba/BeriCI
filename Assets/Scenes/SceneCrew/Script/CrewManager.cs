using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CrewManager : MonoBehaviour
{
    private List<SailorModel> substituteSailors;
    [SerializeField]
    private GameObject iconSailorPrefap;
    [SerializeField]
    private Transform list;
    [SerializeField]
    private Text[] texts;
    [SerializeField]
    private ScriptableObject sailorDes;
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
        substituteSailors = CrewData.Instance.GetSubstituteSailors();
        Debug.LogError(substituteSailors);

        for (int i = 0; i < substituteSailors.Count; i++)
        {


            
           Instantiate(iconSailorPrefap, list);
                //subSailorIcon = imgObject.AddComponent<SubSailorIcon>();
    

            
 
        }
    }


    void SetData(SailorModel model) 
    {
        texts[0].text = model.name;
        texts[1].text = model.id;
        texts[2].text = model.name;
        texts[3].text = model.name;
        texts[4].text = model.name;
        texts[5].text = model.name;
        texts[6].text = model.name;
        texts[7].text = model.name;
        texts[8].text = model.name;


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
