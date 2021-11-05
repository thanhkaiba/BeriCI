using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SquadData : Singleton<SquadData>
{
    // for sailors aren't in squad
    public List<SailorModel> Sailors = new List<SailorModel>();
    private static readonly byte MAX_SQUAD_SLOT = 9;
    private static readonly byte NUM_SQUAD_COL = 3;
    public static readonly byte NUM_SAILOR_IN_SQUAD = 5;

    public Dictionary<short, string> Squad { get; private set; }
    protected override void OnAwake()
    {
        ResetData();
        GameEvent.SquadChange.AddListener(OnUpdateSquad);
    }

    private void OnUpdateSquad()
    {
        SmartFoxConnection.Send(SFSAction.TEAM_COMMIT, toSFSObject());
    }

    private ISFSObject toSFSObject()
    {
        SFSObject data = new SFSObject();
        ISFSArray fighting_lines = new SFSArray();
        foreach (KeyValuePair<short, string> slot in Squad)
        {
            if (slot.Value != "" && slot.Value != null)
            {
                CombatPosition combatPosition = SlotIndex2Position(slot.Key);
                ISFSObject pos = new SFSObject();
                pos.PutByte("x", (byte)combatPosition.x);
                pos.PutByte("y", (byte)combatPosition.y);

                ISFSObject slotData = new SFSObject();
                slotData.PutUtfString("sid", slot.Value);
                slotData.PutSFSObject("pos", pos);
                fighting_lines.AddSFSObject(slotData);
            }
        }

        data.PutSFSArray("fighting_lines", fighting_lines);

        return data;
    }

    private void ResetData()
    {
        Sailors.Clear();
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

        if (Squad[slot] != "")
        {
            return false;
        }

        short oldIndex = SlotOf(sailorId);
        Squad[slot] = sailorId;
        Squad[oldIndex] = "";
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
        short slotIndex = Position2SlotIndex(combatPosition.x, combatPosition.y);
        if (slotIndex < 0 || slotIndex >= MAX_SQUAD_SLOT)
        {
            Debug.LogError("Index is out of range " + combatPosition);
            return null;
        }
        return GetSailorModel(Squad[slotIndex]);
    }

    public static short Position2SlotIndex(short x, short y)
    {
        return (short)(y * NUM_SQUAD_COL + x);
    }

    public static CombatPosition SlotIndex2Position(short index)
    {
        return new CombatPosition(index % NUM_SQUAD_COL, index / NUM_SQUAD_COL);
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
