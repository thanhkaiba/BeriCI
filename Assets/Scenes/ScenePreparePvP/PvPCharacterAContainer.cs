using UnityEngine;

public class PvPCharacterAContainer : BaseCharacterContainer
{ 
    [SerializeField]
    private PvPSquadAContainer squadAContainer;
    protected override void Start()
    {
        addSubSailor = squadAContainer.AddSubSailor;
        getSubstituteSailors = TeamPvPCombatPrepareData.Instance.GetSubstituteSailors;
        GameEvent.PrepareSquadChanged.AddListener(RenderListSubSailor);
        base.Start();
    }
    void OnDestroy()
    {
        GameEvent.PreparePvPSquadChanged.RemoveListener(RenderListSubSailor);
    }
}
