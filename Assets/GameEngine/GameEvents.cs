using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;
    private void Awake()
    {
        instance = this;
    }
    public AttackOneTargetEvent attackOneTarget;
    public TakeDamageEvent takeDamage;
    public CastSkillEvent castSkill;
    public UIHighlightTarget highlightTarget;
    private void Start()
    {
        if (attackOneTarget == null) attackOneTarget = new AttackOneTargetEvent();
        if (takeDamage == null) takeDamage = new TakeDamageEvent();
        if (castSkill == null) castSkill = new CastSkillEvent();
        if (highlightTarget == null) highlightTarget = new UIHighlightTarget();
    }
}

public class AttackOneTargetEvent : UnityEvent<Sailor, Sailor> { }
public class TakeDamageEvent : UnityEvent<Sailor, float> { }
public class CastSkillEvent : UnityEvent<Sailor, Skill> { }
public class UIHighlightTarget : UnityEvent<Sailor> { }