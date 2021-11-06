using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadContainer : MonoBehaviour
{
    private TeamColor teamColor;
    [SerializeField] SquadSlot[] slots;

    void Start()
    {
        for (short i = 0; i < FightingLine.NUM_SQUAD_COL; i++)
        {
            for (short j = 0; j < FightingLine.NUM_SQUAD_COL; j++)
            {
                short key = FightingLine.Position2SlotIndex(i, j);
                SquadSlot slot = slots[key];
                string sailorId = CrewData.Instance.FightingTeam.SailorIdAt(i, j);

                if (sailorId.Length > 0)
                {

                    Sailor sailor = AddSailor(sailorId);
                    slot.SetSelectedSailer(sailor);
                }
                else
                {
                    slot.SetSelectedSailer(null);
                }
            }
        }
        
        teamColor = FindObjectOfType<TeamColor>();


        OnUpdateSquad();
        GameEvent.SquadChange.AddListener(OnUpdateSquad);

    }

    private void OnDestroy()
    {
        GameEvent.SquadChange.RemoveListener(OnUpdateSquad);
    }

    public Sailor AddSubSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(sailorId);
        DragableSubsailor drag = sailor.gameObject.AddComponent<DragableSubsailor>();
        sailor.transform.parent = transform;
        sailor.transform.localPosition = Vector3.zero;
        drag.slots = slots;

        return sailor;
    }

    public Sailor AddSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(sailorId);
        DragableSailor drag = sailor.gameObject.AddComponent<DragableSailor>();
        sailor.transform.parent = transform;
        sailor.transform.localPosition = Vector3.zero;
        drag.slots = slots;

        return sailor;
    }

    public void OnUpdateSquad()
    {
        teamColor.ShowClassBonus();
    }
}
