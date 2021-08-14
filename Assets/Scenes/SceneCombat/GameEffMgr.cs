using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEffMgr : MonoBehaviour
{
    public static GameEffMgr Instance { get; private set; }
    public void Awake()
    {
        Debug.Log("FIRST TIME APPEAR");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public GameObject frozenPrefab;
    public GameObject bigExplosion;
    public GameObject fieldLeft;
    public GameObject fieldRight;

    public float ShowExplosion(Team team)
    {
        var go = team == Team.A ? fieldLeft : fieldRight; 
        GameObject ex = Instantiate(bigExplosion, go.transform.position, go.transform.rotation);
        ex.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(2.0f);
        seq.AppendCallback(() => Destroy(ex));
        return 0.6f;
    }
}
