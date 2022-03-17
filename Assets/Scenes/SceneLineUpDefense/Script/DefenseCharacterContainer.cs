using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseCharacterContainer : BaseCharacterContainer
{
    [SerializeField]
    private DefenseSquadContainer squadContainer;

    protected override void Start()
    {
        addSubSailor = squadContainer.AddSubSailor;
        getSubstituteSailors = PvPData.Instance.DefenseCrew.GetSubstituteSailors;
        base.Start();
        GameEvent.DefenseSquadChanged.AddListener(RenderListSubSailor);

    }
    void OnDestroy() => GameEvent.DefenseSquadChanged.RemoveListener(RenderListSubSailor);
}
