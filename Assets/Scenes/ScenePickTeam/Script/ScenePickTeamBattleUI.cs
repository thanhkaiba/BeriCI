using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScenePickTeamBattleUI : MonoBehaviour
{
    [SerializeField]
    private Text textCountDown;
    [Header("UserA Info")]
    [SerializeField]
    private Text userNameA;
    [SerializeField]
    private Image avatarA;

    [Header("UserB Info")]
    [SerializeField]
    private Text userNameB;
    [SerializeField]
    private Image avatarB;
    public void OnJoinBattle()
    {
        SceneManager.LoadScene("SceneCombat2D");
    }

    public void OnSurrender()
    {
        SceneManager.LoadScene("SceneLobby");
    }
}
