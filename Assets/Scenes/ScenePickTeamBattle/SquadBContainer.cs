using UnityEngine;

public class SquadBContainer : MonoBehaviour
{
    [SerializeField] SquadSlot[] slots;
    [SerializeField]
    private TeamColor teamColor;


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

        teamColor.ShowClassBonus(GameUtils.CalculateClassBonus(CrewData.Instance.GetSquadModelList()));

    }

    public Sailor AddSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(sailorId);
        DragableSailor drag = sailor.gameObject.AddComponent<DragableSailor>();
        sailor.transform.parent = transform;
        sailor.transform.localScale = Vector3.one;
        sailor.transform.localPosition = Vector3.zero;
        drag.slots = slots;

        return sailor;
    }
}
