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
};
