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
    public int exp { get; set; }
    public List<Item> items { get; set; }
    public Sprite img;

    public void LoadConfig()
    {
        config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/" + name);
        img = Resources.Load("ScriptableObject/Character/" + name) as Sprite;
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
        if (items != null)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];
                if (item.class_buff != SailorClass.NONE) result.Add(item.class_buff);
            }
        }

        return result;
    }
}
