using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControls : MonoBehaviour
{
    public void OnJoinBattle()
    {

        TextLocalization.Text("HELLO", text => Debug.Log(text), this);

        SceneManager.LoadScene("Scenes/SceneLobby/SceneLobby");
    }
}
