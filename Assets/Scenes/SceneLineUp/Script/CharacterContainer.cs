using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : BaseCharacterContainer
{
    [SerializeField]
    private SquadContainer squadContainer;

    protected override void Start()
    {
       
        addSubSailor = squadContainer.AddSubSailor;
        getSubstituteSailors = CrewData.Instance.GetSubstituteSailors;
        GameEvent.SquadChanged.AddListener(RenderListSubSailor);
        base.Start();
    }
    void OnDestroy()
    {
        GameEvent.SquadChanged.RemoveListener(RenderListSubSailor);
    }
 
}
