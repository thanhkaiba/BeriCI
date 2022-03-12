using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterAContainer : BaseCharacterContainer
{ 
    [SerializeField]
    private SquadAContainer squadAContainer;
    protected override void Start()
    {
        GameEvent.SquadChanged.AddListener(RenderListSubSailor);
        addSubSailor = (id) => squadAContainer.AddSubSailor(id);
        base.Start();
    }
    void OnDestroy()
    {
        GameEvent.PrepareSquadChanged.RemoveListener(RenderListSubSailor);
    }
}
