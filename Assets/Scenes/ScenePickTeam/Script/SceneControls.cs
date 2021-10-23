using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControls : MonoBehaviour
{
    public void OnJoinBattle()
    {
        SceneManager.LoadScene("Scenes/SceneCombat/SceneGame");
    }
}
