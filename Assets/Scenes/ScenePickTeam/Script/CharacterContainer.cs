using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : MonoBehaviour
{
    private List<SailorModel> substituteSailors;
    [SerializeField]
    private GameObject iconSailorPrefap;
    [SerializeField]
    private ScrollRect scrollRect;
    [SerializeField]
    private float speed = 600f;
    private int scrollDirection = 0;
    void Start()
    {
        RenderListSubSailor();
        GameEvent.SquadChanged.AddListener(RenderListSubSailor);
    }

    void OnDestroy()
    {
        GameEvent.SquadChanged.RemoveListener(RenderListSubSailor);
    }

    public void OnPointerDownRight()
    {
        scrollDirection = 1;
    }

    public void OnPointerDownLeft()
    {
        scrollDirection = -1;
    }

    public void OnPointerUp()
    {
        scrollDirection = 0;
    }

    private void Update()
    {
    
        if (scrollDirection != 0)
        {
            float contentShift = speed * scrollDirection * Time.deltaTime;
            scrollRect.content.offsetMin += new Vector2(contentShift, 0);
            scrollRect.content.offsetMax += new Vector2(-contentShift, 0);
           
        }
    }

    void RenderListSubSailor()
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
