using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using UnityEngine.Events;

public class CombatEvents : MonoBehaviour
{
    public static CombatEvents Instance;
    private void Awake()
    {
        Instance = this;
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    public AttackOneTargetEvent attackOneTarget = new AttackOneTargetEvent();
    public TakeDamageEvent takeDamage = new TakeDamageEvent();
    public GainHealthEvent gainHealth = new GainHealthEvent();
    public UIHighlightTarget highlightTarget = new UIHighlightTarget();
    public ActiveClassBonus activeClassBonus = new ActiveClassBonus();
}

public class AttackOneTargetEvent : UnityEvent<CombatSailor, CombatSailor> { }
public class TakeDamageEvent : UnityEvent<CombatSailor, Damage> { }
public class GainHealthEvent : UnityEvent<CombatSailor, float> { }
public class UIHighlightTarget : UnityEvent<CombatSailor> { }
public class ActiveClassBonus : UnityEvent<CombatSailor, SailorClass, List<float>> { }