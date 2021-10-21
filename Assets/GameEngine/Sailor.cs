using System.Collections.Generic;
using UnityEngine;

public class Sailor : MonoBehaviour
{
    public SailorModel Model;
    public void SetEquipItems(List<Item> _items)
    {
        Model.items = _items;
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
        List<SailorClass> result = new List<SailorClass>(Model.config_stats.classes);
        if (Model.items != null)
        {
            for (int i = 0; i < Model.items.Count; i++)
            {
                Item item = Model.items[i];
                if (item.class_buff != SailorClass.NONE) result.Add(item.class_buff);
            }
        }

        return result;
    }
};
