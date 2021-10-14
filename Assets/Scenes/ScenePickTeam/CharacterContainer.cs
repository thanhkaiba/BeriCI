using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterContainer : MonoBehaviour
{
    private List<string> substituteSailors = new List<string> { "Helti", "Target", "Helti", "Helti", "Target", "Helti" };
    private SquadContainer squadContainer;
    void Start()
    {
        squadContainer = FindObjectOfType<SquadContainer>();
        foreach (string sailorId in substituteSailors)
        {
            CreateSailorIcon(sailorId);

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private GameObject CreateSailorIcon(string sailorId)
    {
        GameObject imgObject = new GameObject(sailorId);

        RectTransform trans = imgObject.AddComponent<RectTransform>();
        trans.transform.SetParent(transform);
        trans.sizeDelta = new Vector2(110, 110);
       

        Image image = imgObject.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + sailorId);

        EventTrigger trigger = imgObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(data => { OnSelectSubSailor((PointerEventData)data, sailorId); });

        trigger.triggers.Add(entry);
        return imgObject;
    }

    public void OnSelectSubSailor(PointerEventData data, string sailorId)
    {
        Sailor sailor = squadContainer.AddSailor(sailorId);
        Drag drag = sailor.GetComponent<Drag>();
        Debug.Log(data.position);
        if (drag.ConvertMousePosWorldPos(data.position, out Vector3 worldPos))
        {
            sailor.transform.position = worldPos;
        } 
        else
        {
            Debug.Log("FAIL");
        }
       
    }
}
