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
        substituteSailors = CrewData.Instance.GetSubstituteSailors();
        foreach (SailorModel model in substituteSailors)
        {
            GameObject imgObject = Instantiate(iconSailorPrefap, transform);
            SubSailorIcon subSailorIcon = imgObject.AddComponent<SubSailorIcon>();
            subSailorIcon.model = model;
            imgObject.GetComponent<IconSailor>().PresentData(model);
        }
    }
}
