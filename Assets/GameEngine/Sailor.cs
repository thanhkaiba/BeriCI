using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sailor : MonoBehaviour
{
    public SailorConfig config_stats;
    public List<Item> items;
    public int level;
    public int quality;
    public void SetEquipItems(List<Item> _items)
    {
        items = _items;
    }
    // animation
    public GameObject modelObject;
    void Awake()
    {
        modelObject = transform.Find("model").gameObject;
    }
    public void TriggerAnimation(string trigger)
    {
        modelObject.GetComponent<Animator>().SetTrigger(trigger);
    }
};
