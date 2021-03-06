using UnityEngine;

public class PvPSquadAContainer : MonoBehaviour
{
    [SerializeField]
    private TeamColor teamColor;
    [SerializeField] SquadSlot[] slots;
    public static bool Draging = false;

    void Start()
    {
        for (short i = 0; i < FightingLine.NUM_SQUAD_COL; i++)
        {
            for (short j = 0; j < FightingLine.NUM_SQUAD_COL; j++)
            {
                short key = FightingLine.Position2SlotIndex(i, j);
                SquadSlot slot = slots[key];
                string sailorId = TeamPvPCombatPrepareData.Instance.YourFightingLine.SailorIdAt(i, j);

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

        OnUpdateSquadA();
        GameEvent.PreparePvPSquadChanged.AddListener(OnUpdateSquadA);

    }

    private void OnDestroy()
    {
        GameEvent.PreparePvPSquadChanged.RemoveListener(OnUpdateSquadA);
    }

    public Sailor AddSubSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(sailorId);
        CreateSubDragableSailorComponent(sailor.gameObject);
        sailor.transform.parent = transform;
        sailor.transform.localScale = Vector3.one;
        sailor.transform.localPosition = Vector3.zero;


        return sailor;
    }

    private void CreateDragableSailorComponent(GameObject gameObject)
    {
        DragableSailor drag = gameObject.AddComponent<DragableSailor>();
        drag.IsSlotEmpty = (short index) => TeamPvPCombatPrepareData.Instance.YourFightingLine.IsSlotEmpty(index);
        drag.UnEquipSailor = (string id) => TeamPvPCombatPrepareData.Instance.UnEquip(id);
        drag.OccupieSailor = (string id, short index) => TeamPvPCombatPrepareData.Instance.Occupie(id, index);
        drag.SwapSailor = (string id, short index) => TeamPvPCombatPrepareData.Instance.Swap(id, TeamPvPCombatPrepareData.Instance.YourFightingLine.OwnerOf(index));
        drag.slots = slots;
    }

    private void CreateSubDragableSailorComponent(GameObject gameObject)
    {
        DragableSubsailor drag = gameObject.AddComponent<DragableSubsailor>();
        drag.ReplaceSailorAction = (string id, short index) => TeamPvPCombatPrepareData.Instance.Replace(id, index);
        drag.IsSquadFull = () => !TeamPvPCombatPrepareData.Instance.YourFightingLine.IsFull();
        drag.slots = slots;
        drag.OnTransfromSailor = (gameObject) => CreateDragableSailorComponent(gameObject);
    }

    public Sailor AddSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(TeamPvPCombatPrepareData.Instance.GetYourSailorModel(sailorId));
        CreateDragableSailorComponent(sailor.gameObject);
        sailor.transform.parent = transform;
        sailor.transform.localScale = Vector3.one;
        sailor.transform.localPosition = Vector3.zero;


        return sailor;
    }
    public bool HaveSailor()
    {
        bool result = false;
        foreach (var s in slots)
        {
            if (s.GetState() == SquadSlot.State.SELECTED)
                result = true;
        }
        return result;
    }
    public void OnUpdateSquadA()
    {
        var synergy = GameUtils.CalculateClassBonus(TeamPvPCombatPrepareData.Instance.GetSquadModelList(true));
        teamColor.ShowClassBonus(synergy);
        GameUtils.lineUpSynergy = synergy;
    }
}
