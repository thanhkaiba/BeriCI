using UnityEngine;

public class SquadContainer : MonoBehaviour
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

        OnUpdateSquad();
        GameEvent.SquadChanged.AddListener(OnUpdateSquad);

    }

    private void OnDestroy()
    {
        GameEvent.SquadChanged.RemoveListener(OnUpdateSquad);
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
        drag.IsSlotEmpty = (short index) => CrewData.Instance.FightingTeam.IsSlotEmpty(index);
        drag.UnEquipSailor = (string id) => CrewData.Instance.UnEquip(id);
        drag.OccupieSailor = (string id, short index) => CrewData.Instance.Occupie(id, index);
        drag.SwapSailor = (string id, short index) => CrewData.Instance.Swap(id, CrewData.Instance.FightingTeam.OwnerOf(index));
        drag.slots = slots;
    }

    private void CreateSubDragableSailorComponent(GameObject gameObject)
    {
        DragableSubsailor drag = gameObject.AddComponent<DragableSubsailor>();
        drag.ReplaceSailorAction = (string id, short index) => CrewData.Instance.Replace(id, index);
        drag.IsSquadFull = () => !CrewData.Instance.FightingTeam.IsFull();
        drag.slots = slots;
        drag.OnTransfromSailor = (gameObject) => CreateDragableSailorComponent(gameObject);
    }

    public Sailor AddSailor(string sailorId)
    {
        Sailor sailor = GameUtils.CreateSailor(sailorId);
        CreateDragableSailorComponent(sailor.gameObject);
        sailor.transform.parent = transform;
        sailor.transform.localScale = Vector3.one;
        sailor.transform.localPosition = Vector3.zero;


        return sailor;
    }

    public void OnUpdateSquad()
    {
        var synergy = GameUtils.CalculateClassBonus(CrewData.Instance.GetSquadModelList(), true);
        teamColor.ShowClassBonus(synergy);
        GameUtils.lineUpSynergy = synergy;
    }
}
