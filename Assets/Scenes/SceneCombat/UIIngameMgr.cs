using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
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
            s.transform.localPosition = new Vector3(-i * 150, 0, 0);
            s.transform.SetSiblingIndex(sailors.Count-i);
            listSailorInQueue.Add(s);
            s.SetData(sailors[i]);
            s.PresentData();
            s.gameObject.SetActive(i < 8);
        }
    }
    public void UpdateListSailorInQueue(List<CombatSailor> sailors)
    {
        for (int i = 0; i < listSailorInQueue.Count; i++)
        {
            var s = listSailorInQueue[i];
            int index = sailors.IndexOf(listSailorInQueue[i].GetData());
            if (index >= 0)
            {
                s.transform.DOLocalMoveX(-index * 150, 0.5f);
                s.transform.SetSiblingIndex(listSailorInQueue.Count-index-1);
                s.PresentData();
                s.gameObject.SetActive(index < 8);
            }
            else
            {
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
            s.transform.DOLocalMoveY(-128*i, 0.5f);
        }

        for (int i = 0; i < passiveTypeB.Count; i++)
        {
            GameObject GO = Resources.Load<GameObject>("Icons/SailorType/combine");
            IconClassBonus s = Instantiate(GO, nodeRight).GetComponent<IconClassBonus>();
            s.SetData(passiveTypeB[i]);
            s.transform.DOLocalMoveY(-128 * i, 0.5f);
        }
    }

    public void ShowSailorDetail(GameObject sailor)
    {
        Debug.Log("click" + sailor.GetComponent<CombatSailor>().config_stats.root_name);
    }

    CombatSailor currentA;
    CombatSailor currentB;
    private void Start()
    {
        //Debug.Log("GameEvents.instance " + GameEvents.instance);
        //Debug.Log("GameEvents.instance.attackOneTarget " + GameEvents.instance.attackOneTarget);
        //GameEvents.Instance.attackOneTarget.AddListener(ShowHighlightConfrontation);
        GameEvents.Instance.takeDamage.AddListener(UpdateTotalHealth);
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
    public void ShowHighLightCastSkill(CombatSailor s, Skill skill)
    {
        ShowHighlightInfo(s);
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
        icon.sprite = Resources.Load<Sprite>("Icons/IconSailor/" + sailor.config_stats.root_name);
        health.SetValue(sailor.cs.CurHealth / sailor.cs.MaxHealth);
        //Debug.Log("huhhh " + sailor.charName + " e >>>>>> " + sailor.cs.current_health + " " + sailor.cs.max_health + " " + health.value);
        if (sailor.cs.MaxFury != 0) fury.value = (float) sailor.cs.Fury / (float) sailor.cs.MaxFury;
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
    }
}
