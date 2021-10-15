using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterContainer : MonoBehaviour
{
    private List<string> substituteSailors = new List<string> { "Helti", "Target", "Helti", "Helti", "Target", "Helti" };
    private SquadContainer squadContainer;
    private Canvas canvas;
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
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
        GameObject imgObject = CreateSailorImage(sailorId, transform,out Image image);

        EventTrigger trigger = imgObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener(data => { OnSelectSubSailor((PointerEventData)data, sailorId, image); });

        trigger.triggers.Add(entry);
        return imgObject;
    }

    private GameObject CreateSailorImage(string sailorId, Transform parent ,out Image image)
    {
        GameObject imgObject = new GameObject(sailorId);
        RectTransform trans = imgObject.AddComponent<RectTransform>();
        if (parent != null)
        {
            imgObject.transform.SetParent(parent);
        }
        trans.sizeDelta = new Vector2(110, 110);
        image = imgObject.AddComponent<Image>();
        image.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + sailorId);

        return imgObject;
    }

    public void OnSelectSubSailor(PointerEventData data, string sailorId, Image originImage)
    {
        Sailor sailor = squadContainer.AddSubSailor(sailorId);
        DragableSubsailor drag = sailor.GetComponent<DragableSubsailor>();


        CreateSailorImage(sailorId,  canvas.transform, out Image dragImage);

        originImage.enabled = false;
        dragImage.transform.position = originImage.transform.position;
        drag.SetStartPosition(data.position, dragImage, originImage);
    }
}
