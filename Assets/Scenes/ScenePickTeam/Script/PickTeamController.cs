using UnityEngine.SceneManagement;

public class PickTeamController : BaseController
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
