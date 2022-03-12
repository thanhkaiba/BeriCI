using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseCharacterContainer : BaseCharacterContainer
{
    [SerializeField]
    private DefenseSquadContainer squadContainer;

    protected override void Start()
    {
        GameEvent.DefenseSquadChanged.AddListener(RenderListSubSailor);
        addSubSailor = (id) => squadContainer.AddSubSailor(id);
        base.Start();
    }
    void OnDestroy() => GameEvent.DefenseSquadChanged.RemoveListener(RenderListSubSailor);
}
