using UnityEngine;

public class CharacterAContainer : BaseCharacterContainer
{ 
    [SerializeField]
    private SquadAContainer squadAContainer;
    protected override void Start()
    {
        addSubSailor = squadAContainer.AddSubSailor;
        getSubstituteSailors = TeamCombatPrepareData.Instance.GetSubstituteSailors;
        GameEvent.PrepareSquadChanged.AddListener(RenderListSubSailor);
        base.Start();
    }
    void OnDestroy()
    {
        GameEvent.PrepareSquadChanged.RemoveListener(RenderListSubSailor);
    }
}
