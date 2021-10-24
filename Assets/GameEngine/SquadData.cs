using System;
using System.Collections.Generic;
using UnityEngine;

public class SquadData : Singleton<SquadData>
{
    // for sailors aren't in squad
    public List<SailorModel> Sailors;
    private static readonly byte MAX_SQUAD_SLOT = 9;
    private static readonly byte NUM_SQUAD_COL = 3;
    public static readonly byte NUM_SAILOR_IN_SQUAD = 5;

    public Dictionary<short, string> Squad { get; private set; }
    protected override void OnAwake()
    {
        SetUpFakeData();
    }

    private void SetUpFakeData()
    {
        Sailors = new List<SailorModel>
        {
            new SailorModel("1", "Meechic") { quality = 1, level = 1},
            new SailorModel("2", "Meechic") { quality = 1, level = 1},
            new SailorModel("3", "Meechic") { quality = 1, level = 1},
            new SailorModel("4", "Meechic") { quality = 1, level = 1},
            new SailorModel("5", "Meechic") { quality = 1, level = 1},
            new SailorModel("6", "Target") { quality = 1, level = 1},
            new SailorModel("7", "Target") { quality = 1, level = 1},
            new SailorModel("8", "Target") { quality = 1, level = 1},
            new SailorModel("9", "Target") { quality = 1, level = 1},
            new SailorModel("10", "Target") { quality = 1, level = 1},
            new SailorModel("11", "Helti") { quality = 1, level = 1},
            new SailorModel("12", "Helti") { quality = 1, level = 1},
            new SailorModel("12A", "Helti") { quality = 1, level = 1},
            new SailorModel("13", "Helti") { quality = 1, level = 1},
            new SailorModel("14", "Helti") { quality = 1, level = 1},
           
        };
        
        Squad = new Dictionary<short, string>
        {
            {0,  ""},
            {1,  ""},
            {2,  ""},
            {3,  ""},
            {4,  ""},
            {5,  ""},
            {6,  ""},
            {7,  ""},
            {8,  ""},
        };

    }

    public SailorModel GetSailorModel(string id)
    {
        return Sailors.Find(s => s.id == id);
    }

    public bool Swap(string sailorA, string sailorB)
    {
        short slotA = SlotOf(sailorA);
        short slotB = SlotOf(sailorB);

        if (slotA == -1 || slotB == -1)
        {
            return false;
        }

        Squad[slotA] = sailorB;
        Squad[slotB] = sailorA;
        GameEvent.SquadChange.Invoke();

        return true;
    }

    public bool Occupie(string sailorId,short slot)
    {
        if (GetSailorModel(sailorId) == null)
        {
            return false;
        }

        if (Squad[slot] != null)
        {
            return false;
        }
        Squad[slot] = sailorId;
        GameEvent.SquadChange.Invoke();
        return true;
    }

    public bool Replace(string subSailor, short slot)
    {
        if (GetSailorModel(subSailor) == null)
        {
            return false;
        }
        Squad[slot] = subSailor;
        GameEvent.SquadChange.Invoke();
        return true;
    }

    public short SlotOf (string id)
    {

        foreach (KeyValuePair<short, string> slot in Squad)
        {
           if (slot.Value == id)
            {
                return slot.Key;
            }
        }

        return -1;
    }

    public string OwnerOf(short slot)
    {
        if (Squad.ContainsKey(slot))
        {
            return Squad[slot];
        }
        else
        {
            throw new Exception(String.Format("Slot {0} was not found", slot));
        }
    }

    public bool IsSlotEmpty(short slot)
    {
        if (Squad.ContainsKey(slot))
        {
            return Squad[slot] == "";
        } else
        {
            throw new Exception(String.Format("Slot {0} was not found", slot));
        }
    }

    public bool IsInSquad(string id)
    {
        Dictionary<short, string>.ValueCollection values = Squad.Values;
        foreach (string val in values)
        {
            if (id == val)
            {
                return true;
            }
        }
        return false;
    }

    public List<SailorModel> GetSquadModelList()
    {
        List<SailorModel> result = new List<SailorModel>();
        Dictionary<short, string>.ValueCollection values = Squad.Values;
        foreach (string val in values)
        {
           if (val != "")
            {
                result.Add(GetSailorModel(val));
            }
        }
        return result;
    }

    public List<SailorModel> GetSubstituteSailors()
    {
        List<SailorModel> result = new List<SailorModel>();
        foreach (SailorModel model in Sailors)
        {
            if (!IsInSquad(model.id))
            {
                result.Add(model);
            }
        }
        return result;
    }

    /// <summary>
    /// Get a sailor in squad by a combatPosition {x, y}
    /// </summary>
    /// <param name="combatPosition"></param>
    /// <returns>return null if not found</returns>
    public SailorModel SailorAt(CombatPosition combatPosition)
    {
        short slotIndex = (short)(combatPosition.x * NUM_SQUAD_COL + combatPosition.y);
        if (slotIndex < 0 || slotIndex >= MAX_SQUAD_SLOT)
        {
            Debug.LogError("Index is out of range " + combatPosition);
            return null;
        }
        return GetSailorModel(Squad[slotIndex]);
    }

    public bool IsSquadFull()
    {
        int count = 0;
        Dictionary<short, string>.ValueCollection values = Squad.Values;
        foreach (string val in values)
        {
            if (val != "")
            {
                count++;
            }
        }
        return count >= NUM_SAILOR_IN_SQUAD;
    }
}
