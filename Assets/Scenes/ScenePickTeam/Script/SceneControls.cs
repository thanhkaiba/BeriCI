using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControls : MonoBehaviour
{
    public void OnJoinBattle()
    {

        Debug.Log(TextLocalization.Text("HELLO"));
        SceneManager.LoadScene("Scenes/SceneCombat/SceneGame");
    }
}
