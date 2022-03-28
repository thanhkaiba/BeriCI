using UnityEngine;

public class PvPSquadBContainer : MonoBehaviour
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
                string sailorId = TeamPvPCombatPrepareData.Instance.OpponentFightingLine.SailorIdAt(i, j);

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

        OnUpdateSquadB();

    }

    public Sailor AddSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(TeamPvPCombatPrepareData.Instance.GetOpponentSailorModel(sailorId));
        sailor.transform.parent = transform;
        sailor.transform.localScale = Vector3.one;
        sailor.transform.localPosition = Vector3.zero;
        var model = sailor.transform.Find("model");
        var curScale = model.localScale;
        model.localScale = new Vector3(-curScale.x, curScale.y, curScale.z);
        return sailor;
    }

    public void OnUpdateSquadB()
    {
        var synergy = GameUtils.CalculateClassBonus(TeamPvPCombatPrepareData.Instance.GetSquadModelList(false));
        teamColor.ShowClassBonus(synergy);
        GameUtils.oppLineUpSynergy = synergy;
    }
}
