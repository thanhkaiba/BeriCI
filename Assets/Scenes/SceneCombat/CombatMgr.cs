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
    public CombatState combatState;
    public UIIngameMgr UIMgr;
    public int actionCount = 0;
    private void Start()
    {
        //return;
        combatState.CreateDemoTeam();
        combatState.CalculateTypePassive();
        combatState.UpdateGameWithPassive();
        UIMgr.HideAllHighlightInfo();
        UIMgr.InitListSailorInQueue(combatState.GetQueueNextActionSailor());
        UIMgr.ShowCombineSailorType(combatState.passiveTypeA, combatState.passiveTypeB);
        UIMgr.ShowActionCount(actionCount);
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
        //Debug.Log(" ----> speedAdd: " + speedAdd);
        Sailor actionChar = AddSpeedAndGetActionCharacter(speedAdd);
        actionCount++;
        UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor());
        UIMgr.HideAllHighlightInfo();
        UIMgr.ShowActionCount(actionCount);
        Debug.Log(
            " ----> combat action character: " + actionChar.charName
            + " | team: " + (actionChar.cs.team == Team.A ? "A" : "B")
            + " | position: " + actionChar.cs.position
        );
        float delayTime = actionChar.DoCombatAction(combatState) + 0.2f;
        combatState.lastTeamAction = actionChar.cs.team;
        StartCoroutine(WaitAndDo(2*delayTime/3, () => UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor()) ));
        StartCoroutine(EndLoop(actionChar, delayTime));
        StartCoroutine(NextLoop(delayTime));
    }

    IEnumerator EndLoop(Sailor actor, float delay)
    {
        yield return new WaitForSeconds(delay);
        combatState.RunEndAction(actor);
    }

    IEnumerator NextLoop(float delay)
    {
        yield return new WaitForSeconds(delay);
        Team winTeam = CheckTeamWin();
        if (winTeam == Team.NONE) CombatLoop();
        else GameOver(winTeam);
    }
    IEnumerator WaitAndDo(float time, Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

    void GameOver(Team winTeam)
    {
        UIMgr.UpdateListSailorInQueue(combatState.GetQueueNextActionSailor());
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
        combatState.GetAllAliveCombatCharacters().ForEach(delegate (Sailor character)
        {
            character.AddSpeed(speedAdd);
        });

        return combatState.GetQueueNextActionSailor().First();
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
