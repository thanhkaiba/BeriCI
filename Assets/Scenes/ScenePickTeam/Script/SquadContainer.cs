using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadContainer : MonoBehaviour
{
    [SerializeField] SquadSlot[] slots;
    List<string> squad = new List<string> { "Helti", "Target", "Helti", "", "", "", "Helti" , "Target", "Helti" };
    void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            SquadSlot slot = slots[i];
            string sailorId = squad[i];

            if (sailorId.Length > 0)
            {

                Sailor sailor = AddSailor(sailorId);
                slot.SetSelectedSailer(sailor);
            } else
            {
                slot.SetSelectedSailer(null);
            }
        }
      
    }

    public Sailor AddSubSailor(string sailorId)
    {
        Sailor sailor = GameUtils.Instance.CreateSailor(sailorId);
        DragableSubsailor drag = sailor.gameObject.AddComponent<DragableSubsailor>();
        sailor.transform.parent = transform;
        drag.slots = slots;

        return sailor;
    }

    public Sailor AddSailor(string sailorId)
    {
        Sailor sailor = GameUtils.Instance.CreateSailor(sailorId);
        DragableSailor drag = sailor.gameObject.AddComponent<DragableSailor>();
        sailor.transform.parent = transform;
        drag.slots = slots;

        return sailor;
    }
}
