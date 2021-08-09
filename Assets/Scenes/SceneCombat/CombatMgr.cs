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
        CombatCharacter actionChar = AddSpeedAndGetActionCharacter(speedAdd);
        Debug.Log(
            " ----> combat action character: " + actionChar.charName
            + " | team: " + (actionChar.team == Team.A ? "A" : "B")
            + " | position: " + actionChar.position
        );
        float delayTime = actionChar.DoCombatAction(combatState);
        combatState.lastTeamAction = actionChar.team;
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
        combatState.GetAllAliveCombatCharacters().ForEach(delegate (CombatCharacter character)
        {
            int speedNeed = character.GetSpeedNeeded();
            speedAdd = Math.Min(speedNeed, speedAdd);
        });

        return Math.Max(speedAdd, 0);
    }

    CombatCharacter AddSpeedAndGetActionCharacter(int speedAdd)
    {
        List<CombatCharacter> listAvaiableCharacter = new List<CombatCharacter>();

        combatState.GetAllAliveCombatCharacters().ForEach(delegate (CombatCharacter character)
        {
            character.AddSpeed(speedAdd);
            if (character.IsEnoughSpeed()) listAvaiableCharacter.Add(character);
        });

        return RuleGetActionCharacter(listAvaiableCharacter);
    }
    CombatCharacter RuleGetActionCharacter(List<CombatCharacter> listAvaiableCharacter)
    {
        listAvaiableCharacter.Sort(delegate (CombatCharacter c1, CombatCharacter c2)
        {
            if (c1.max_speed < c2.max_speed) return -1;
            else return 1;
        });
        List<CombatCharacter> teamA = new List<CombatCharacter>();
        List<CombatCharacter> teamB = new List<CombatCharacter>();
        listAvaiableCharacter.ForEach(delegate (CombatCharacter character)
        {
            if (character.team == Team.A) teamA.Add(character);
            if (character.team == Team.B) teamB.Add(character);
        });
        List<CombatCharacter> targetList = teamA;
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
