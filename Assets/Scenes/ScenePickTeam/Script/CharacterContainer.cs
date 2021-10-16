using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : MonoBehaviour
{
    private List<string> substituteSailors = new List<string> { "Helti", "Target", "Helti", "Helti", "Target", "Helti" };
    private SquadContainer squadContainer;
    void Start()
    {
       
        foreach (string sailorId in substituteSailors)
        {
            GameObject imgObject = new GameObject(sailorId);
            imgObject.transform.SetParent(transform);
            SubSailorIcon subSailorIcon = imgObject.AddComponent<SubSailorIcon>();
            subSailorIcon.Init(transform).CreateSailorImage(sailorId);
}
    } 
}
