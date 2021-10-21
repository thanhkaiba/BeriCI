using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailorModel
{
    public SailorModel(string _id, string _name)
    {
        id = _id;
        name = _name;
        LoadConfig();
    }
    public SailorConfig config_stats { get; set; }
    public readonly string id;
    public readonly string name;
    public int quality { get; set; }
    public int level { get; set; }
    public List<Item> items { get; set; }

    public void LoadConfig()
    {
        config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/" + name);
    }
}
