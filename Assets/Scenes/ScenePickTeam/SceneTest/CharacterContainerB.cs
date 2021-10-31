using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainerB : MonoBehaviour
{
    private List<SailorModel> substituteSailors;
    void Start()
    {
        substituteSailors = SquadBData.Instance.GetSubstituteSailors();
        foreach (SailorModel model in substituteSailors)
        {
            GameObject imgObject = new GameObject(model.id);
            imgObject.transform.SetParent(transform);
            SubSailorIconB subSailorIcon = imgObject.AddComponent<SubSailorIconB>();
            subSailorIcon.Init(transform).CreateSailorImage(model.id, model.name);
        }
    }
}
