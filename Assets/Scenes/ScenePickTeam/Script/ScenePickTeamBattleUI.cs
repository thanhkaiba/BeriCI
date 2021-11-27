using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePickTeamBattleUI : MonoBehaviour
{
    public void OnJoinBattle()
    {
        SceneManager.LoadScene("SceneCombat2D");
    }
}
