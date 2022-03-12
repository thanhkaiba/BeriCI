using Piratera.Network;
using Sfs2X.Entities.Data;
using System.Collections.Generic;

public class DefenseCrewData : Singleton<DefenseCrewData>
{
    public FightingLine FightingTeam = new();

    protected override void OnAwake()
    {
        ResetData();
        GameEvent.DefenseSquadChanged.AddListener(OnUpdateSquad);
    }
    private void OnUpdateSquad()
    {
        NetworkController.Send(SFSAction.UPDATE_ARENA_FGL, FightingTeam.ToSFSObject());
    }

    public void OnConfirmSquad()
    {
        NetworkController.Send(SFSAction.UPDATE_ARENA_FGL, FightingTeam.ToSFSObject());
    }
    private void ResetData()
    {
        FightingTeam.Clean();

    }
    public void NewFromSFSObject(ISFSObject packet)
    {
        ResetData();
        
        FightingTeam.NewFromSFSObject(packet.GetSFSArray("defense_fgl"));
    }
    public SailorModel GetSailorModel(string id)
    {
        return CrewData.Instance.Sailors.Find(s => s.id == id);
    }
    public void Swap(string sailorA, string sailorB)
    {
        if (FightingTeam.Swap(sailorA, sailorB))
        {
            GameEvent.DefenseSquadChanged.Invoke();
        }

    }
    public void Occupie(string sailorId, short slot)
    {
        if (GetSailorModel(sailorId) == null)
        {
            return;
        }
        if (FightingTeam.Occupie(sailorId, slot))
        {
            GameEvent.DefenseSquadChanged.Invoke();
        }
    }
    public void Replace(string subSailor, short slot)
    {
        if (GetSailorModel(subSailor) == null)
        {
            return;
        }

        if (FightingTeam.Replace(subSailor, slot))
        {
            GameEvent.DefenseSquadChanged.Invoke();
        }
    }

    public void UnEquip(string sailorId)
    {
        if (GetSailorModel(sailorId) == null)
        {
            return;
        }

        if (FightingTeam.UnEquip(sailorId))
        {
            GameEvent.DefenseSquadChanged.Invoke();
        }
    }
    public List<SailorModel> GetSquadModelList()
    {
        List<SailorModel> result = new();
        List<string> values = FightingTeam.GetValues();
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
        List<SailorModel> result = new();
        foreach (SailorModel model in CrewData.Instance.Sailors)
        {
            if (!FightingTeam.IsInSquad(model.id))
            {
                result.Add(model);
            }
        }
        result.Sort();
        result.Reverse();
        return result;
    }

    /// <summary>
    /// Get a sailor in squad by a combatPosition {x, y}
    /// </summary>
    /// <param name="combatPosition"></param>
    /// <returns>return null if not found</returns>
    public SailorModel SailorAt(CombatPosition combatPosition)
    {
        return GetSailorModel(FightingTeam.SailorIdAt(combatPosition));
    }
}
