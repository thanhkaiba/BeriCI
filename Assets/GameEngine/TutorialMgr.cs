using Piratera.Config;
using Piratera.Network;
using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMgr : Singleton<TutorialMgr>
{
    bool haveGreeting = false; // da chao o lobby
    public bool CheckTutStartUp()
    {
        if (UserData.Instance.NumSlot > 1) PlayerPrefs.SetInt("ShowTutStartUp", 1);
        else if (UserData.Instance.PVECount > 0) PlayerPrefs.SetInt("ShowTutStartUp", 1);
        else if (GameTimeMgr.GetCurrentUTCTime() - UserData.Instance.CreateAt > 2 * 1000*60*60*24) PlayerPrefs.SetInt("ShowTutStartUp", 1);
        int haveShown = PlayerPrefs.GetInt("ShowTutStartUp", 0);
        if (haveShown == 1) return false;
        return CrewData.Instance.Sailors.Count > 0;
    }
    public bool CheckTutStartUp_Greeting()
    {
        if (CheckTutStartUp() && !haveGreeting)
        {
            haveGreeting = true;
            return true;
        }
        return false;
    }
    public void CheckRunTutStartUp_StepLineUpBack2Lobby()
    {
        if (CheckTutStartUp() && SceneManager.GetActiveScene().name == "ScenePickTeam")
        {
            var LineUpUI = GameObject.Find("PickTeamController").GetComponent<PickTeamUI>();
            LineUpUI.ShowTutBackToLobby();
        }
    }
    public void CompleteStartUp()
    {
        PlayerPrefs.SetInt("ShowTutStartUp", 1);
    }
    public bool CheckTutOpenSlot()
    {
        if (CheckTutStartUp()) return false;
        if (UserData.Instance.NumSlot > 1) PlayerPrefs.SetInt("ShowTutOpenSlot", 1);
        int haveShown = PlayerPrefs.GetInt("ShowTutOpenSlot", 0);
        if (haveShown == 1) return false;
        return  UserData.Instance.Beri >= GlobalConfigs.LineUp.costs[0] && CrewData.Instance.Sailors.Count >= 2;
    }
    public void CompleteOpenSlot()
    {
        PlayerPrefs.SetInt("ShowTutOpenSlot", 1);
    }
}
