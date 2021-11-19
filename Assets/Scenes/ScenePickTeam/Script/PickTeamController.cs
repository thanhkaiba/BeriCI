using UnityEngine.SceneManagement;
using UnityEngine;

public class PickTeamController: MonoBehaviour
{
    private void Start()
    {
        SquadContainer.Draging = false;
    }
    public void OnJoinBattle()
    {
        SceneManager.LoadScene("SceneCombat2d");
    }

    public void OnBackToLobby()
    {
        SceneManager.LoadScene("SceneLobby");
    }
  
}
