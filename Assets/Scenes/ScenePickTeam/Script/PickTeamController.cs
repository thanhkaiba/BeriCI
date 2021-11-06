using UnityEngine.SceneManagement;
using UnityEngine;

public class PickTeamController: MonoBehaviour
{
    public void OnJoinBattle()
    {
        SceneManager.LoadScene("SceneCombat2d");
    }

    public void OnBackToLobby()
    {
        SceneManager.LoadScene("SceneLobby");
    }
  
}
