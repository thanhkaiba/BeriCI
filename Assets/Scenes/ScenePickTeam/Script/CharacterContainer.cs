using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : MonoBehaviour
{
    private List<SailorModel> substituteSailors;
    void Start()
    {
        substituteSailors = CrewData.Instance.GetSubstituteSailors();
        foreach (SailorModel model in substituteSailors)
        {
            GameObject imgObject = new GameObject(model.id);
            imgObject.transform.SetParent(transform);
            SubSailorIcon subSailorIcon = imgObject.AddComponent<SubSailorIcon>();
            subSailorIcon.Init(transform).CreateSailorImage(model.id, model.name);
        }
    }
}
