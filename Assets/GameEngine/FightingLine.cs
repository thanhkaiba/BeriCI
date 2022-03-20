using Sfs2X.Entities.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FightingLine
{
    public static readonly byte MAX_SQUAD_SLOT = 9;
    public static readonly byte NUM_SQUAD_COL = 3;

    private int numSlot = -1;
    public int NumSlot
    {
        get
        {
            if (numSlot > 0)
            {
                return numSlot;
            }
            return UserData.Instance.NumSlot;
        }

        set
        {
            numSlot = value;
        }
    }
    private Dictionary<short, string> slots = new Dictionary<short, string>
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

    public bool Swap(string sailorA, string sailorB)
    {
        if (sailorA == sailorB)
        {
            return false;
        }
        short slotA = SlotOf(sailorA);
        short slotB = SlotOf(sailorB);

        if (slotA == -1 || slotB == -1)
        {
            return false;
        }

        slots[slotA] = sailorB;
        slots[slotB] = sailorA;

        return true;
    }

    public short SlotOf(string id)
    {

        foreach (KeyValuePair<short, string> slot in slots)
        {
            if (slot.Value == id)
            {
                return slot.Key;
            }
        }

        return -1;
    }

    public bool Occupie(string sailorId, short slotIndex)
    {

        if (slots[slotIndex] != "")
        {
            return false;
        }

        short oldIndex = SlotOf(sailorId);
        slots[slotIndex] = sailorId;
        slots[oldIndex] = "";
        return true;
    }

    public bool Replace(string sailor, short slot)
    {

        slots[slot] = sailor;
        return true;
    }

    public string OwnerOf(short slotIndex)
    {
        if (slots.ContainsKey(slotIndex))
        {
            return slots[slotIndex];
        }
        else
        {
            throw new Exception(String.Format("Slot {0} was not found", slotIndex));
        }
    }


    public bool IsSlotEmpty(short slot)
    {
        if (slots.ContainsKey(slot))
        {
            return slots[slot] == "";
        }
        else
        {
            throw new Exception(String.Format("Slot {0} was not found", slot));
        }
    }

    internal bool UnEquip(string sailorId)
    {
        short slotIndex = SlotOf(sailorId);
        if (slots.ContainsKey(slotIndex))
        {
            slots[slotIndex] = "";
            return true;
        }
        return false;
    }

    public bool IsInSquad(string id)
    {
        Dictionary<short, string>.ValueCollection values = slots.Values;
        foreach (string val in values)
        {
            if (id == val)
            {
                return true;
            }
        }
        return false;
    }


    public static short Position2SlotIndex(short x, short y)
    {
        return (short)(y * NUM_SQUAD_COL + x);
    }

    public static CombatPosition SlotIndex2Position(short index)
    {
        return new CombatPosition(index % NUM_SQUAD_COL, index / NUM_SQUAD_COL);
    }


    public List<string> GetValues()
    {
        return slots.Values.ToList();
    }

    public bool IsFull()
    {
        return GetCount() >= NumSlot;
    }

    public int GetCount()
    {
        int count = 0;
        Dictionary<short, string>.ValueCollection values = slots.Values;
        foreach (string val in values)
        {
            if (val != "")
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// Get a sailor in squad by a combatPosition {x, y}
    /// </summary>
    /// <param name="combatPosition"></param>
    /// <returns>return null if not found</returns>
    public string SailorIdAt(CombatPosition combatPosition)
    {
        return SailorIdAt(combatPosition.x, combatPosition.y);
    }


    public string SailorIdAt(short x, short y)
    {
        short slotIndex = Position2SlotIndex(x, y);
        if (slotIndex < 0 || slotIndex >= MAX_SQUAD_SLOT)
        {
            Debug.LogError($"Index is out of range x = {x} y = {y}");
            return null;
        }
        return slots[slotIndex];
    }


    public ISFSObject ToSFSObject()
    {
        SFSObject data = new SFSObject();
        data.PutSFSArray("fighting_lines", ToSFSArray());

        return data;
    }

    public ISFSArray ToSFSArray()
    {
        ISFSArray fighting_lines = new SFSArray();
        foreach (KeyValuePair<short, string> slot in slots)
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
        return fighting_lines;

    }

    public FightingLine NewFromSFSObject(ISFSArray sFSArray)
    {
        if (sFSArray != null)
        {
            foreach (ISFSObject obj in sFSArray)
            {
                string uid = obj.GetUtfString("sid");
                ISFSObject pos = obj.GetSFSObject("pos");
                byte x = pos.GetByte("x");
                byte y = pos.GetByte("y");

                short slotIndex = Position2SlotIndex(x, y);
                slots[slotIndex] = uid;
            }
        }
       
        return this;
    }

    public void Clean()
    {
        for (short i = 0; i < MAX_SQUAD_SLOT; i++)
        {
            slots[i] = "";
        }
        NumSlot = -1;
    }
}