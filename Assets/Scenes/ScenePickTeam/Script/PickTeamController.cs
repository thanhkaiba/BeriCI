using UnityEngine.SceneManagement;

public class PickTeamController : BaseController
{
    public void OnJoinBattle()
    {
        SceneManager.LoadScene("SceneGame");
    }

    public void OnBackToLobby()
    {
        SceneManager.LoadScene("Scenes/SceneLobby/SceneLobby");
    }
  
}
