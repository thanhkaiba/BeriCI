using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControls : MonoBehaviour
{
    public void OnJoinBattle()
    {
        SceneManager.LoadScene("SceneGame");
    }
    // todo test save
    public void SaveSquadB()
    {
        SquadData.Instance.SaveSquadB();
    }
}
