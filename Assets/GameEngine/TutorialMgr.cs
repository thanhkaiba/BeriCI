using Piratera.Network;
using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMgr : Singleton<TutorialMgr>
{
    bool haveStartUp = false; // da hoan thanh tut start up
    bool haveGreeting = false; // da chao o lobby

    public bool CheckTutStartUp()
    {
        return true && !haveStartUp;
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
        haveStartUp = true;
    }
}
