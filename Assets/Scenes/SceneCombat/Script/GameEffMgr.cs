using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
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
    public Transform BulletToTarget(Vector3 startPos, Vector3 targetPos, float delay, float flyTime, float scale = 1f)
    {
        var bulletGO = Instantiate(Resources.Load<GameObject>("Effect2D/Duong_FX/VFX_Piratera/meechik_projectile"), startPos, Quaternion.identity);
        bulletGO.SetActive(false);

        Vector3 oriPos = transform.position;
        float d = Vector3.Distance(oriPos, targetPos);
        Vector3 desPos = Vector3.MoveTowards(oriPos, targetPos, d);
        int isFlip = 1;
        if (bulletGO.transform.position.x > desPos.x) isFlip = -1;
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => bulletGO.SetActive(true));
        seq.Append(bulletGO.transform.DOMove(desPos, flyTime).SetEase(Ease.OutSine));
        seq.AppendCallback(() =>
        {
            //ShowSmallExplosion(bulletGO.transform.position);
            //ShowSmoke4(bulletGO.transform.position, startPos.x > targetPos.x);


            GameObject ex = Instantiate(Resources.Load<GameObject>("Effect2D/bullet_explore/bullet_explore"), bulletGO.transform.position, new Quaternion());
            Destroy(bulletGO);
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(2.0f);
            seq2.AppendCallback(() => Destroy(ex));
            ex.transform.localScale = new Vector3(isFlip * 3 * scale, 3 * scale, 3 * scale);
        });
        bulletGO.transform.localScale = new Vector3(scale, scale, scale);
        return bulletGO.transform;
    }
    public void TrailToTarget(string trail, string explore, Vector3 startPos, Vector3 targetPos, float delay, float flyTime, float scaleTrail, float scaleExplore)
    {
        var bulletGO = Instantiate(Resources.Load<GameObject>(trail), startPos, Quaternion.identity);

        bulletGO.SetActive(false);

        Vector3 oriPos = transform.position;
        Vector3 desPos = targetPos;
        int isFlip = 1;
        if (bulletGO.transform.position.x > desPos.x) isFlip = -1;
        bulletGO.transform.localScale = new Vector3(isFlip * scaleTrail, scaleTrail, scaleTrail);
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.AppendCallback(() => bulletGO.SetActive(true));
        seq.Append(bulletGO.transform.DOMove(desPos, flyTime).SetEase(Ease.InSine));
        seq.AppendCallback(() =>
        {
            Vector3 pos = bulletGO.transform.position;
            GameObject ex = Instantiate(Resources.Load<GameObject>(explore), pos, new Quaternion());
            ex.transform.localScale = new Vector3(isFlip * scaleExplore, scaleExplore, scaleExplore);
            Destroy(bulletGO);
            Sequence seq2 = DOTween.Sequence();
            seq2.AppendInterval(2.0f);
            seq2.AppendCallback(() => Destroy(ex));

        });
    }
    public void ShowSmallExplosion(Vector3 position)
    {
        GameObject ex = Instantiate(Resources.Load<GameObject>("Effect2D/rocket_explore/Rocket_normal"), position, new Quaternion());
        Sequence seq2 = DOTween.Sequence();
        ex.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        ex.transform.localPosition += new Vector3(0, 0, -3f);
        seq2.AppendInterval(2.0f);
        seq2.AppendCallback(() => Destroy(ex));
    }
    public void ShowSmoke4(Vector3 position, bool flip)
    {
        GameObject ex = Instantiate(Resources.Load<GameObject>("Effect2D/118 sprite effects bundle/19 effects/smoke_4"), position, new Quaternion());
        Sequence seq2 = DOTween.Sequence();
        seq2.AppendInterval(2.0f);
        seq2.AppendCallback(() => Destroy(ex));
        ex.transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
    }
    public GameObject ShowSmokeSide(Vector3 position, bool flip)
    {
        GameObject ex = Instantiate(Resources.Load<GameObject>("Effect2D/smoke/smoke_side/smoke_side"), position, new Quaternion());
        Sequence seq2 = DOTween.Sequence();
        seq2.AppendInterval(2.0f);
        seq2.AppendCallback(() => Destroy(ex));
        ex.transform.localScale = new Vector3(flip ? 1 : -1, 1, 1);
        return ex;
    }
    public GameObject ShowPurple(Vector3 position, bool flip)
    {
        GameObject ex = Instantiate(Resources.Load<GameObject>("Effect2D/118 sprite effects bundle/25 sprite effects/ef_22_purple"), position, new Quaternion());
        Sequence seq2 = DOTween.Sequence();
        seq2.AppendInterval(.5f);
        seq2.AppendCallback(() => Destroy(ex));
        ex.transform.localScale = new Vector3(flip ? 1 : -1, 1, 1);
        return ex;
    }
    public void ShowExplosion(Vector3 position)
    {
        GameObject ex = Instantiate(Resources.Load<GameObject>("ParticleEffect/FireExplosionEffects/Prefabs/BigExplosion"), position, new Quaternion());
        ex.transform.localScale = new Vector3(5.0f, 3.0f, 5.0f);
        Sequence seq2 = DOTween.Sequence();
        seq2.AppendInterval(2.0f);
        seq2.AppendCallback(() => Destroy(ex));
    }
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
        //CombatEvents.Instance.activeClassBonus.AddListener(ActiveClassBonus);
    }
    private void ActiveClassBonus(CombatSailor sailor, SailorClass type, List<float> _params)
    {
        ShowClassIconActive(sailor.transform.position, type);
        //switch (type)
        //{
        //    case SailorType.BERSERK:

        //        break;
        //}
        //return;
    }
    public float ShowClassIconActive(Vector3 p, SailorClass type)
    {
        GameObject go = new GameObject();
        SpriteRenderer spr = go.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        Billboard billboard = go.AddComponent(typeof(Billboard)) as Billboard;
        billboard.cam = Camera.main.transform;
        spr.sprite = Resources.Load<Sprite>("Icons/SailorType/" + type);
        spr.transform.localScale = new Vector3(3, 3, 3);
        go.transform.position = p;
        Sequence seq = DOTween.Sequence();
        seq.Append(go.transform.DOMoveY(p.y + 8, 0));
        seq.Append(go.transform.DOScale(5f, 0.4f));
        seq.AppendCallback(() => Destroy(go));
        return 0.4f;
    }
    public void Shake(float dur = 0.1f, float strength = 1)
    {
        var cam = Camera.main.transform;
        var seq = DOTween.Sequence();
        var z = cam.position.z;
        float s = 0.06f;
        float max_delta = 0.05f * strength;
        int times = Mathf.RoundToInt(dur / s);
        for (int i = 0; i < times; i++)
        {
            float delta = max_delta * Mathf.Sqrt(1f / (i + 1));
            float x = Random.Range(-delta, delta);
            float y = Random.Range(-delta, delta) * 0.6f;
            seq.Append(cam.transform.DOMove(new Vector3(x, y, z), 0));
            seq.AppendInterval(s);
        }
        seq.Append(cam.transform.DOMove(new Vector3(0, 0, z), 0));
    }
    public void CreateCanonShoot()
    {
        var prefab = Resources.Load<GameObject>("Effect2D/canon/canon");
        var go = Instantiate(prefab, GameObject.Find("nodeCanon").transform);
        go.transform.Find("spine").GetComponent<Animator>().SetTrigger("Shoot");
    }
    public void CreateCanonExplore()
    {
        var prefab = Resources.Load<GameObject>("Effect2D/canon/canon");
        var go = Instantiate(prefab, GameObject.Find("FieldA").transform);
        go.transform.localScale = Vector3.one * 5;
        go.transform.Find("spine").GetComponent<Animator>().SetTrigger("Explore");
    }
    public GameObject CreateSpeedEffect(Vector3 pos)
    {
        Sequence seq = DOTween.Sequence();
        var eff = Instantiate(Resources.Load<GameObject>("Effect2D/Duong_FX/VFX_Piratera/fx_speed"), pos, Quaternion.identity);
        eff.transform.localScale = Vector3.one * 1.8f;
        seq.AppendInterval(2f);
        seq.AppendCallback(() => Destroy(eff));
        return eff;
    }
    public GameObject CreateAbsorbEffect(Vector3 pos, float offsetY)
    {
        pos.y += offsetY;
        pos.z -= 0.5f;
        Sequence seq = DOTween.Sequence();
        var eff = Instantiate(Resources.Load<GameObject>("Effect2D/absorb/absorb"), pos, Quaternion.identity);
        seq.AppendInterval(5f);
        seq.AppendCallback(() => Destroy(eff));
        return eff;
    }
}
