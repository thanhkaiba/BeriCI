using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sailors : MonoBehaviour
{
    [SerializeField] Slot[] slots;
    List<string> squad = new List<string> { "Helti", "Target", "Helti", "", "", "", "Helti" , "Target", "Helti" };
    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            Slot slot = slots[i];
            string sailorId = squad[i];

            if (sailorId.Length > 0)
            {
              
                Sailor sailor = GameUtils.Instance.CreateSailor(sailorId);
                Drag drag = sailor.gameObject.AddComponent<Drag>();
                sailor.transform.parent = transform;
                drag.slots = slots;

                slot.SetSelectedSailer(sailor);
            } else
            {
                slot.SetSelectedSailer(null);
            }
        }
      
    }
}
