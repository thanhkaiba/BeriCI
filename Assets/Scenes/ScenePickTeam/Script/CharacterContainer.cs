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

    public float Speed = 0;
    [SerializeField]
    private float Acceleration = 20f;
    [SerializeField]
    private float MaxSpeed = 600f;
    private int direction = 0;

    [SerializeField]
    private SquadContainer squadContainer;
    void Start()
    {
        HandleScreenSize();
        RenderListSubSailor();
        GameEvent.SquadChanged.AddListener(RenderListSubSailor);
    }

    void HandleScreenSize()
    {
        RectTransform canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
        RectTransform rect = (scrollRect.transform as RectTransform);
        rect.sizeDelta = new Vector2(canvas.rect.width - 420, rect.sizeDelta.y); 
    }

    void OnDestroy()
    {
        GameEvent.SquadChanged.RemoveListener(RenderListSubSailor);
    }

    public void OnPointerDownRight()
    {
        direction = 1;
       
    }

    public void OnPointerDownLeft()
    {
        direction = -1;
       
    }

    public void OnPointerUp()
    {
        direction = 0;
    }

    private void Update()
    {
        
      
        if (direction != 0)
        {
            Speed += Acceleration * direction * Time.deltaTime ;

            if (Mathf.Abs(Speed) >  MaxSpeed)
            {
                Speed = direction * MaxSpeed;
            }

            
            float contentWidth = scrollRect.content.sizeDelta.x;
            float curAmout = scrollRect.horizontalNormalizedPosition + Speed / contentWidth;
            scrollRect.horizontalNormalizedPosition = Mathf.Clamp(curAmout, 0, 1);

        } else
        {
            Speed = 0;
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
                subSailorIcon.AddSubSailor = (id) => squadContainer.AddSubSailor(id);              
            }
            subSailorIcon.model = substituteSailors[i];
            imgObject.GetComponent<IconSailor>().PresentData(substituteSailors[i]);
        }
    }
}
