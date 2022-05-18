using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterContainer : BaseCharacterContainer
{
    [SerializeField]
    private SquadContainer squadContainer;
    [SerializeField]
    private Toggle toggleShowAll;

    protected override void Start()
    {
       
        addSubSailor = squadContainer.AddSubSailor;
        getSubstituteSailors = GetListSailor;
        GameEvent.SquadChanged.AddListener(RenderListSubSailor);
        base.Start();
    }
    List<SailorModel> GetListSailor()
    {
        List<SailorModel> result = new List<SailorModel>();
        bool isShowFav = PlayerPrefs.GetInt("is_show_fav", 0) == 1;
        Debug.Log("isShowFav: " + isShowFav);
        foreach (SailorModel model in CrewData.Instance.Sailors)
        {
            if (!CrewData.Instance.FightingTeam.IsInSquad(model.id))
            {
                if (!isShowFav) result.Add(model);
                else if (GameUtils.IsSailorFavorite(model.id)) result.Add(model);
            }   
        }
        result.Sort();
        result.Reverse();
        return result;
    } 
    void OnDestroy()
    {
        GameEvent.SquadChanged.RemoveListener(RenderListSubSailor);
    }
}
