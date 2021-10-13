using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class GameEffMgr : MonoBehaviour
{
    public static GameEffMgr Instance { get; private set; }
    public void Awake()
    {
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
    public float ShowSkillIconFall(Vector3 p, string skillName)
    {
        GameObject go = new GameObject();
        SpriteRenderer spr = go.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spr.sprite = Resources.Load<Sprite>("IconSkill/" + skillName);
        go.transform.position = p;
        Sequence seq = DOTween.Sequence();
        seq.Append(go.transform.DOMoveY(p.y + 4, 0));
        seq.Append(go.transform.DOMoveY(p.y + 1, 0.6f));
        seq.AppendCallback(() => Destroy(go));
        return 0.4f;
    }
    public float ShowSkillIconActive(Vector3 p, string skillName)
    {
        GameObject go = new GameObject();
        SpriteRenderer spr = go.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        spr.sprite = Resources.Load<Sprite>("IconSkill/" + skillName);
        go.transform.position = p;
        Sequence seq = DOTween.Sequence();
        seq.Append(go.transform.DOMoveY(p.y + 2, 0));
        seq.Append(go.transform.DOScale(1.5f, 0.6f));
        seq.AppendCallback(() => Destroy(go));
        return 0.4f;
    }
    private void Start()
    {
        //GameObject prefab = Resources.Load<GameObject>("sea/waveprefab");
        //for (int i = -15; i < 16; i++)
        //{
        //    for (int j = 0; j < 1; j++) {
        //        GameObject eff = Instantiate(prefab, new Vector3(25*j, -3, i * 2f), new Quaternion(0, 0, 0, 0));
        //        eff.GetComponent<WaveFakeOcean>().isReverse = i % 2 == 0;
        //    }
        //}
        GameEvents.Instance.activeClassBonus.AddListener(ActiveClassBonus);
    }
    private void ActiveClassBonus(Sailor sailor, SailorType type, List<float> _params)
    {
        ShowClassIconActive(sailor.transform.position, type);
        //switch (type)
        //{
        //    case SailorType.BERSERK:

        //        break;
        //}
        //return;
    }
    public float ShowClassIconActive(Vector3 p, SailorType type)
    {
        GameObject go = new GameObject();
        SpriteRenderer spr = go.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Billboard billboard = go.AddComponent(typeof(Billboard)) as Billboard;
        billboard.cam = Camera.main.transform;
        spr.sprite = Resources.Load<Sprite>("Icons/sailor_type/" + type);
        spr.transform.localScale = new Vector3(3, 3, 3);
        go.transform.position = p;
        Sequence seq = DOTween.Sequence();
        seq.Append(go.transform.DOMoveY(p.y + 8, 0));
        seq.Append(go.transform.DOScale(5f, 0.4f));
        seq.AppendCallback(() => Destroy(go));
        return 0.4f;
    }
}
