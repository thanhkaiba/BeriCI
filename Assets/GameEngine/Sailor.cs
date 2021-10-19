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
    public bool HaveType(SailorClass type)
    {
        bool found = false;
        GetListClasses().ForEach(_t =>
        {
            if (_t == type) found = true;
        });
        return found;
    }
    public List<SailorClass> GetListClasses()
    {
        List<SailorClass> result = new List<SailorClass>(config_stats.classes);
        for (int i = 0; i < items.Count; i++)
        {
            Item item = items[i];
            if (item.class_buff != SailorClass.NONE) result.Add(item.class_buff);
        }
        return result;
    }
};
