using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
   public void OnLoginSuccess()
    {
        SceneManager.LoadScene("Scenes/SceneLobby/SceneLobby");
    }
}
