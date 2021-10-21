using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
  
    public void RunScenePickTeam()
    {
    
        SceneManager.LoadScene("Scenes/ScenePickTeam/ScenePickTeam");
    }
}
