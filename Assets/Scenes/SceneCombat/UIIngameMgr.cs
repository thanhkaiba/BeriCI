using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class UIIngameMgr : MonoBehaviour
{
    public GameObject sailorInQueue;
    public List<SailorInQueue> listSailorInQueue;
    public Text actionCount;
    public void InitListSailorInQueue(List<Sailor> sailors)
    {
        for (int i = 0; i < sailors.Count; i++)
        {
            SailorInQueue s = Instantiate(sailorInQueue, transform.FindDeepChild("ListSailorInQueue")).GetComponent<SailorInQueue>();
            s.transform.localPosition = new Vector3(-i * 96, 0, 0);
            s.transform.SetSiblingIndex(sailors.Count-i);
            listSailorInQueue.Add(s);
            s.SetData(sailors[i]);
            s.PresentData();
            //s.transform.DOLocalMoveX(250 - i * 96, 0.5f);
        }
    }
    public void UpdateListSailorInQueue(List<Sailor> sailors)
    {
        for (int i = 0; i < listSailorInQueue.Count; i++)
        {
            var s = listSailorInQueue[i];
            int index = sailors.IndexOf(listSailorInQueue[i].GetData());
            if (index >= 0)
            {
                s.transform.DOLocalMoveX(-index * 96, 0.5f);
                s.transform.SetSiblingIndex(listSailorInQueue.Count-index-1);
                s.PresentData();
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

    Sailor currentA;
    Sailor currentB;
    private void Start()
    {
        Debug.Log("GameEvents.instance " + GameEvents.instance);
        Debug.Log("GameEvents.instance.attackOneTarget " + GameEvents.instance.attackOneTarget);
        GameEvents.instance.attackOneTarget.AddListener(ShowHighlightConfrontation);
        GameEvents.instance.takeDamage.AddListener(ShowHighLightTakeDamage);
        GameEvents.instance.castSkill.AddListener(ShowHighLightCastSkill);
        GameEvents.instance.highlightTarget.AddListener(ShowHighlightInfo);
    }
    public void ShowHighlightConfrontation(Sailor a, Sailor b)
    {
        ShowHighlightInfo(a);
        ShowHighlightInfo(b);
    }
    public void ShowHighLightTakeDamage(Sailor s, float damage)
    {
        if (s.cs.team == Team.A && currentA == s) ShowTakeDamage(s);
        if (s.cs.team == Team.B && currentB == s) ShowTakeDamage(s);
    }
    public void ShowHighLightCastSkill(Sailor s, Skill skill)
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
    public void ShowHighlightInfo(Sailor sailor)
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
        icon.sprite = Resources.Load<Sprite>("IconSailor/" + sailor.charName);
        health.SetValue(sailor.cs.current_health / sailor.cs.max_health);
        //Debug.Log("huhhh " + sailor.charName + " e >>>>>> " + sailor.cs.current_health + " " + sailor.cs.max_health + " " + health.value);
        if (sailor.cs.max_fury != 0) fury.value = (float) sailor.cs.current_fury / (float) sailor.cs.max_fury;
        else fury.value = 1;
    }
    public void ShowTakeDamage(Sailor sailor)
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
        health.ChangeValue(sailor.cs.current_health / sailor.cs.max_health);
    }
}
