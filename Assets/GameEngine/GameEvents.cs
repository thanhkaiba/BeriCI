using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Events;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Instance;
    private void Awake()
    {
        Instance = this;
    }
    public AttackOneTargetEvent attackOneTarget = new AttackOneTargetEvent();
    public TakeDamageEvent takeDamage = new TakeDamageEvent();
    public CastSkillEvent castSkill = new CastSkillEvent();
    public UIHighlightTarget highlightTarget = new UIHighlightTarget();
    public ActiveClassBonus activeClassBonus = new ActiveClassBonus();
}

public class AttackOneTargetEvent : UnityEvent<Sailor, Sailor> { }
public class TakeDamageEvent : UnityEvent<Sailor, Damage> { }
public class CastSkillEvent : UnityEvent<Sailor, Skill> { }
public class UIHighlightTarget : UnityEvent<Sailor> { }
public class ActiveClassBonus : UnityEvent<Sailor, SailorType, List<float>> { }