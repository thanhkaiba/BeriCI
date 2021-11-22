using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : MonoBehaviour
{
    private List<SailorModel> substituteSailors;
    [SerializeField]
    private GameObject iconSailorPrefap;
    void Start()
    {
        RenderListSubSailor();
        GameEvent.SquadChanged.AddListener(RenderListSubSailor);
    }

    private void OnDestroy()
    {
        GameEvent.SquadChanged.RemoveListener(RenderListSubSailor);
    }

    void RenderListSubSailor()
    {
        substituteSailors = CrewData.Instance.GetSubstituteSailors();

        if (substituteSailors.Count < transform.childCount)
        {
            for (int i = substituteSailors.Count; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
       
        for (int i = 0; i < substituteSailors.Count; i++)
        {
            GameObject imgObject;
            SubSailorIcon subSailorIcon;
            if (i < transform.childCount)
            {
                imgObject = transform.GetChild(i).gameObject;
                subSailorIcon = imgObject.GetComponent<SubSailorIcon>();
            } else
            {
                imgObject = Instantiate(iconSailorPrefap, transform);
                subSailorIcon = imgObject.AddComponent<SubSailorIcon>();
            }
            subSailorIcon.model = substituteSailors[i];
            imgObject.GetComponent<IconSailor>().PresentData(substituteSailors[i]);
        }
    }
}
