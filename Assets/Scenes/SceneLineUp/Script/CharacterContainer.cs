using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : BaseCharacterContainer
{
    [SerializeField]
    private SquadContainer squadContainer;

    protected override void Start()
    {
        GameEvent.SquadChanged.AddListener(RenderListSubSailor);
        addSubSailor = (id) => squadContainer.AddSubSailor(id);
        base.Start();
    }
    void OnDestroy()
    {
        GameEvent.SquadChanged.RemoveListener(RenderListSubSailor);
    }
 
}
