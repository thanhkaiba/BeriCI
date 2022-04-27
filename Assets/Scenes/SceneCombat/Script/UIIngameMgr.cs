using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIIngameMgr : MonoBehaviour
{
    public static UIIngameMgr Instance;
    public GameObject sailorInQueue;
    public Text actionCount;
    public List<SailorInQueue> listSailorInQueue;
    public Text timescale;
    public UserAvatar avtA;
    public UserAvatar avtB;

    public Text usernameA;
    public Text usernameB;

    public void Awake()
    {
        Instance = this;
    }
    public void OnDestroy()
    {
        Instance = null;
    }
    public void InitListSailorInQueue(List<CombatSailor> sailors)
    {
        Transform node = transform.FindDeepChild("ListSailorInQueue");
        for (int i = 0; i < sailors.Count; i++)
        {
            SailorInQueue s = Instantiate(sailorInQueue, node).GetComponent<SailorInQueue>();
            s.transform.localPosition = new Vector3(-i * 154 + (i == 0 ? 0 : -6), 0, 0);
            s.transform.SetSiblingIndex(sailors.Count - i);
            listSailorInQueue.Add(s);
            s.SetData(sailors[i]);
            s.PresentData();
            if (i >= 5) s.gameObject.SetActive(false);
        }
    }
    public void UpdateListSailorInQueue()
    {
        UpdateListSailorInQueue(CombatState.Instance.GetQueueNextActionSailor());
    }
    public void UpdateListSailorInQueue(List<CombatSailor> sailors)
    {
        for (int i = 0; i < listSailorInQueue.Count; i++)
        {
            var s = listSailorInQueue[i];
            int index = sailors.IndexOf(listSailorInQueue[i].GetData());
            if (index >= 0 && index < 5)
            {
                s.gameObject.SetActive(true);
                s.transform.DOLocalMoveX(-index * 154 + (index == 0 ? 0 : -6), 0.5f).SetEase(Ease.OutSine);
                s.transform.SetSiblingIndex(listSailorInQueue.Count - index - 1);
                s.PresentData();
            }
            else
            {
                s.transform.position = new Vector3(-4 * index + (index == 0 ? 0 : -6), s.transform.position.y, s.transform.position.z);
                s.gameObject.SetActive(false);
            }
        }
    }

    public void ShowActionCount(int count)
    {
        actionCount.text = count.ToString();
    }

    public void ShowClassBonus(List<ClassBonusItem> passiveTypeA, List<ClassBonusItem> passiveTypeB)
    {
        Transform nodeLeft = transform.FindDeepChild("NodeTypeLeft");
        Transform nodeRight = transform.FindDeepChild("NodeTypeRight");

        for (int i = 0; i < passiveTypeA.Count; i++)
        {
            GameObject GO = Resources.Load<GameObject>("Icons/SailorType/combine");
            IconClassBonus s = Instantiate(GO, nodeLeft).GetComponent<IconClassBonus>();
            s.SetData(passiveTypeA[i]);
            s.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            s.transform.DOLocalMoveX(72 * i, 0.5f).SetLink(s.gameObject).SetTarget(s);
        }

        for (int i = 0; i < passiveTypeB.Count; i++)
        {
            GameObject GO = Resources.Load<GameObject>("Icons/SailorType/combine");
            IconClassBonus s = Instantiate(GO, nodeRight).GetComponent<IconClassBonus>();
            Vector3 pos = s.transform.GetChild(1).transform.localPosition;
            s.transform.GetChild(1).transform.localPosition = new Vector3(-pos.x, pos.y, pos.z);
            s.SetData(passiveTypeB[i]);
            s.transform.localScale = new Vector3(0.8f, 0.8f, 1);
            s.transform.DOLocalMoveX(-72 * i, 0.5f).SetLink(s.gameObject).SetTarget(s);
        }
    }

    public void ShowSailorDetail(GameObject sailor)
    {
        Debug.Log("click" + sailor.GetComponent<CombatSailor>().Model.config_stats.root_name);
    }

    CombatSailor currentA;
    CombatSailor currentB;
    private void Start()
    {
        avtA.ShowAvatar(TempCombatData.Instance.yourTeamIndex == 0 ? TempCombatData.Instance.avt0 : TempCombatData.Instance.avt1);
        avtB.ShowAvatar(TempCombatData.Instance.yourTeamIndex == 0 ? TempCombatData.Instance.avt1 : TempCombatData.Instance.avt0);
        usernameA.text = TempCombatData.Instance.yourTeamIndex == 0 ? TempCombatData.Instance.userName0 : TempCombatData.Instance.userName1;
        usernameB.text = TempCombatData.Instance.yourTeamIndex == 0 ? TempCombatData.Instance.userName1 : TempCombatData.Instance.userName0;
        ShowTimeScale();

        //Debug.Log("GameEvents.instance " + GameEvents.instance);
        //Debug.Log("GameEvents.instance.attackOneTarget " + GameEvents.instance.attackOneTarget);
        //GameEvents.Instance.attackOneTarget.AddListener(ShowHighlightConfrontation);
        CombatEvents.Instance.takeDamage.AddListener(UpdateTotalHealth);
        CombatEvents.Instance.gainHealth.AddListener(UpdateTotalHealth);
        //GameEvents.Instance.castSkill.AddListener(ShowHighLightCastSkill);
        //GameEvents.Instance.highlightTarget.AddListener(ShowHighlightInfo);
    }
    public void ShowHighlightConfrontation(CombatSailor a, CombatSailor b)
    {
        ShowHighlightInfo(a);
        ShowHighlightInfo(b);
    }
    public void UpdateTotalHealth(CombatSailor s, Damage damage)
    {
        //if (s.cs.team == Team.A && currentA == s) ShowTakeDamage(s);
        //if (s.cs.team == Team.B && currentB == s) ShowTakeDamage(s);
        UpdateTotalHealth(Team.A);
        UpdateTotalHealth(Team.B);
    }
    public void UpdateTotalHealth(CombatSailor s, float heal)
    {
        //if (s.cs.team == Team.A && currentA == s) ShowTakeDamage(s);
        //if (s.cs.team == Team.B && currentB == s) ShowTakeDamage(s);
        UpdateTotalHealth(Team.A);
        UpdateTotalHealth(Team.B);
    }
    public void HideAllHighlightInfo()
    {
        transform.FindDeepChild("sailorA").gameObject.SetActive(false);
        currentA = null;
        transform.FindDeepChild("sailorB").gameObject.SetActive(false);
        currentB = null;
    }
    public void ShowHighlightInfo(CombatSailor sailor)
    {
        if (sailor == null) return;
        GameObject node;
        Image icon;
        HealthSlider health;
        Slider fury;
        switch (sailor.cs.team)
        {
            case Team.A:
                node = transform.FindDeepChild("sailorA").gameObject;
                icon = transform.FindDeepChild("IconSailorA").GetComponent<Image>();
                health = transform.FindDeepChild("sliderHealthA").GetComponent<HealthSlider>();
                fury = transform.FindDeepChild("sliderFuryA").GetComponent<Slider>();
                currentA = sailor;
                break;
            case Team.B:
                node = transform.FindDeepChild("sailorB").gameObject;
                icon = transform.FindDeepChild("IconSailorB").GetComponent<Image>();
                health = transform.FindDeepChild("sliderHealthB").GetComponent<HealthSlider>();
                fury = transform.FindDeepChild("sliderFuryB").GetComponent<Slider>();
                currentB = sailor;
                break;
            default:
                return;
        }
        node.SetActive(true);
        icon.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + sailor.Model.config_stats.root_name);
        health.SetValue(sailor.cs.CurHealth / sailor.cs.MaxHealth);
#if PIRATERA_DEV
        health.Heath.text = $"{sailor.cs.CurHealth} / {sailor.cs.MaxHealth}";
#endif
        //Debug.Log("huhhh " + sailor.charName + " e >>>>>> " + sailor.cs.current_health + " " + sailor.cs.max_health + " " + health.value);
        if (sailor.cs.MaxFury != 0) fury.value = (float)sailor.cs.Fury / (float)sailor.cs.MaxFury;
        else fury.value = 1;
    }
    public void ShowTakeDamage(CombatSailor sailor)
    {
        if (sailor == null) return;
        HealthSlider health;
        switch (sailor.cs.team)
        {
            case Team.A:
                health = transform.FindDeepChild("sliderHealthA").GetComponent<HealthSlider>();
                currentA = sailor;
                break;
            case Team.B:
                health = transform.FindDeepChild("sliderHealthB").GetComponent<HealthSlider>();
                currentB = sailor;
                break;
            default:
                return;
        }
        health.ChangeValue(sailor.cs.CurHealth / sailor.cs.MaxHealth);
#if PIRATERA_DEV
        health.Heath.text = $"{sailor.cs.CurHealth} / {sailor.cs.MaxHealth}";
#endif
    }
    public void UpdateTotalHealth()
    {
        UpdateTotalHealth(Team.A);
        UpdateTotalHealth(Team.B);
    }
    private void UpdateTotalHealth(Team t)
    {
        HealthSlider health;
        GameObject node;
        switch (t)
        {
            case Team.A:
                node = transform.FindDeepChild("sailorA").gameObject;
                health = transform.FindDeepChild("sliderHealthA").GetComponent<HealthSlider>();
                break;
            case Team.B:
                node = transform.FindDeepChild("sailorB").gameObject;
                health = transform.FindDeepChild("sliderHealthB").GetComponent<HealthSlider>();
                break;
            default:
                return;
        }
        node.SetActive(true);
        health.ChangeValue(CombatState.Instance.GetTeamHealthRatio(t));
#if PIRATERA_DEV
        health.Heath.text = CombatState.Instance.GetTeamHealthRatioString(t);
#endif
    }
    public void ShowTimeScale()
    {
        timescale.text = "X" + (int)Time.timeScale;
    }
}
