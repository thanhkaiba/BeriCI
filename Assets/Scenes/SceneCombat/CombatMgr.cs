using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public enum Team
{
    A,
    B,
    NONE,
    BOTH,
};

public class CombatMgr : MonoBehaviour
{
    CombatState combatState;
    public GameObject characterFrefab;
    private void Start()
    {
        combatState = GameObject.Find("CombatState").GetComponent<CombatState>();
        combatState.CreateDemoTeam();

        StartCoroutine(StartGame());
    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2);
        combatState.status = CombatStatus.STARTED;
        Debug.Log(">>>>>>>>>>> Start Game <<<<<<<<<<<<");
        CombatLoop();
    }
    void CombatLoop()
    {
        int speedAdd = CalculateSpeedAddThisLoop();
        Debug.Log(" ----> speedAdd: " + speedAdd);
        Sailor actionChar = AddSpeedAndGetActionCharacter(speedAdd);
        Debug.Log(
            " ----> combat action character: " + actionChar.charName
            + " | team: " + (actionChar.cs.team == Team.A ? "A" : "B")
            + " | position: " + actionChar.cs.position
        );
        float delayTime = actionChar.DoCombatAction(combatState);
        combatState.lastTeamAction = actionChar.cs.team;
        StartCoroutine(NextLoop(delayTime));
    }
    IEnumerator NextLoop(float delay)
    {
        yield return new WaitForSeconds(delay);
        Team winTeam = CheckTeamWin();
        if (winTeam == Team.NONE) CombatLoop();
        else GameOver(winTeam);
    }

    void GameOver(Team winTeam)
    {
        Debug.Log(">>>>>>> Game Over <<<<<<<<<");
        Debug.Log("Team " + winTeam + " win");
    }

    int CalculateSpeedAddThisLoop()
    {
        int speedAdd = 9999;
        combatState.GetAllAliveCombatCharacters().ForEach(delegate (Sailor character)
        {
            int speedNeed = character.GetSpeedNeeded();
            speedAdd = Math.Min(speedNeed, speedAdd);
        });

        return Math.Max(speedAdd, 0);
    }

    Sailor AddSpeedAndGetActionCharacter(int speedAdd)
    {
        List<Sailor> listAvaiableCharacter = new List<Sailor>();

        combatState.GetAllAliveCombatCharacters().ForEach(delegate (Sailor character)
        {
            character.AddSpeed(speedAdd);
            if (character.IsEnoughSpeed()) listAvaiableCharacter.Add(character);
        });

        return RuleGetActionCharacter(listAvaiableCharacter);
    }
    Sailor RuleGetActionCharacter(List<Sailor> listAvaiableCharacter)
    {
        listAvaiableCharacter.Sort(delegate (Sailor c1, Sailor c2)
        {
            if (c1.cs.max_speed < c2.cs.max_speed) return -1;
            else return 1;
        });
        List<Sailor> teamA = new List<Sailor>();
        List<Sailor> teamB = new List<Sailor>();
        listAvaiableCharacter.ForEach(delegate (Sailor character)
        {
            if (character.cs.team == Team.A) teamA.Add(character);
            if (character.cs.team == Team.B) teamB.Add(character);
        });
        List<Sailor> targetList = teamA;
        if (
            teamA.Count <= 0
            || (combatState.lastTeamAction == Team.A && teamB.Count > 0)
        ) targetList = teamB;
        Debug.Log("targetList" + targetList.Count);
        return targetList.First();
    }
    public Team CheckTeamWin()
    {
        bool A_alive = combatState.GetAllTeamAliveCharacter(Team.A).Count() > 0;
        bool B_alive = combatState.GetAllTeamAliveCharacter(Team.B).Count() > 0;

        if (A_alive && !B_alive) return Team.A;
        if (!A_alive && B_alive) return Team.B;
        if (!A_alive && !B_alive) return Team.BOTH;
        return Team.NONE;
    }
}
