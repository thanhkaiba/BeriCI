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
    public GameObject bigExplosion;
    public GameObject fireBall;
    public GameObject energyExplosion;
    public GameObject fieldLeft;
    public GameObject fieldRight;

    public float ShowExplosion(Team team)
    {
        var go = team == Team.A ? fieldLeft : fieldRight; 
        GameObject ex = Instantiate(bigExplosion, go.transform.position, go.transform.rotation);
        ex.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(2.0f);
        seq.AppendCallback(() => Destroy(ex));
        return 0.6f;
    }
    public float ShowFireBallFly(Vector3 start, Team team)
    {
        var go = team == Team.A ? fieldLeft : fieldRight;
        GameObject ex = Instantiate(fireBall, start, new Quaternion(0, 0, 0, 0));
        Sequence seq = DOTween.Sequence();
        seq.Append(ex.transform.DOJump(go.transform.position, 8, 1, 1.0f));
        seq.AppendCallback(() => Destroy(ex));
        return 1.0f;
    }
    public float ShowBuffEnergy(Vector3 start, Vector3 end)
    {
        GameObject eff = Instantiate(energyExplosion, new Vector3(start.x, start.y + 2.0f, start.z), new Quaternion(0, 0, 0, 0));
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.6f);
        seq.Append(eff.transform.DOMove(new Vector3(end.x, end.y + 2.0f, end.z), 0.5f));
        seq.AppendInterval(0.6f);
        seq.AppendCallback(() => Destroy(eff));
        return 0.8f;
    }
}
