using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseCharacterContainer : MonoBehaviour
{
    protected List<SailorModel> substituteSailors;
    [SerializeField]
    protected GameObject iconSailorPrefap;
    [SerializeField]
    protected ScrollRect scrollRect;

    protected Func<string, Sailor> addSubSailor;

    protected virtual void Start()
    {
        RenderListSubSailor();
    }
   
    protected void RenderListSubSailor()
    {
        substituteSailors = CrewData.Instance.GetSubstituteSailors();
        int childCount = transform.childCount;
        if (substituteSailors.Count < childCount)
        {
            for (int i = substituteSailors.Count + 1; i < childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            childCount = substituteSailors.Count;
        }
        for (int i = 0; i < substituteSailors.Count; i++)
        {
            GameObject imgObject;
            SubSailorIcon subSailorIcon;
            if (i < childCount)
            {
                imgObject = transform.GetChild(i).gameObject;
                subSailorIcon = imgObject.GetComponent<SubSailorIcon>();
                subSailorIcon.iconSailor.SetVisible(true);
            }
            else
            {
                imgObject = Instantiate(iconSailorPrefap, transform);
                subSailorIcon = imgObject.AddComponent<SubSailorIcon>();
                subSailorIcon.AddSubSailor = addSubSailor;
            }
            subSailorIcon.scrollRect = scrollRect;
            subSailorIcon.Model = substituteSailors[i];
        }
    }
}
