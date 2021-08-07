using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public enum Team
{
    A,
    B
};

public class CombatMgr : MonoBehaviour
{
    public Camera MainCamera;
    public CombatState combatState;
    public GameObject characterFrefab;
    private void Start()
    {
        combatState = new CombatState();
        combatState.GetAllCombatCharacters().ForEach(delegate (CombatCharacter character)
        {
            character.SetGameObject(CreateCharacter(character));
        });

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
        actionChar.DoCombatAction(combatState);
        combatState.lastTeamAction = actionChar.team;
        StartCoroutine(NextLoop(1f));
    }
    IEnumerator NextLoop(float delay)
    {
        yield return new WaitForSeconds(delay);
        CombatLoop();
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
            Debug.Log(character.current_speed);
            if (character.IsEnoughSpeed()) listAvaiableCharacter.Add(character);
        });

        return RuleGetActionCharacter(listAvaiableCharacter);
    }
    CombatCharacter RuleGetActionCharacter(List<CombatCharacter> listAvaiableCharacter)
    {
        // dang lay phan tu dau tien, sau thay bang rule trong design
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
    // create character on screen
    GameObject CreateCharacter(CombatCharacter character)
    {
        UnityEngine.Vector3 p = GameObject.Find("slot_" + (character.team == Team.A ? "A" : "B") + character.position.x + character.position.y).transform.position;
        GameObject c = Instantiate(characterFrefab, p, Quaternion.identity);
        GameObject child = c.transform.Find("sword_man").gameObject;
        c.GetComponent<Billboard>().cam = MainCamera.transform;
        c.GetComponent<CharacterAnimatorCtrl>().SetHealthBar(character.max_health, character.current_health);
        child.transform.localScale = new Vector3(character.team == Team.A ? -100f : 100f, 100f, 100f);
        return c;
    }
}
