using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadContainer : MonoBehaviour
{
    private TeamColor teamColor;
    [SerializeField] SquadSlot[] slots;
    List<string> squad = new List<string> { "Helti", "Target", "Helti", "", "", "", "Helti" , "Target", "Helti" };

    void Start()
    {
        teamColor = FindObjectOfType<TeamColor>();
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
        OnUpdateSquad();
      
    }

    public Sailor AddSubSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(sailorId);
        DragableSubsailor drag = sailor.gameObject.AddComponent<DragableSubsailor>();
        drag.setListener(() => OnUpdateSquad());
        sailor.transform.parent = transform;
        drag.slots = slots;

        return sailor;
    }

    public Sailor AddSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(sailorId);
        DragableSailor drag = sailor.gameObject.AddComponent<DragableSailor>();
        drag.setListener(() => OnUpdateSquad());
        sailor.transform.parent = transform;
        drag.slots = slots;

        return sailor;
    }

    public void OnUpdateSquad()
    {
        List<Sailor> squad = new List<Sailor>();
        foreach (SquadSlot slot in slots)
        {
            if (slot.GetOwner() != null)
            {
                squad.Add(slot.GetOwner());
            }
        }

        teamColor.ShowClassBonus(squad);
    }
}
