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
        GetAllCombatCharacters().ForEach(delegate (CombatCharacter character)
        {
            character.SetGameObject(CreateCharacter(character));
        });

        StartCoroutine(StartGame());

    }
    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);
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
            " ----> combat action character: " + actionChar.name
            + " | team: " + (actionChar.team == Team.A ? "A" : "B")
            + " | position: " + actionChar.position
        );
        actionChar.DoCombatAction(combatState);
    }

    int CalculateSpeedAddThisLoop()
    {
        int speedAdd = 9999;
        GetAllCombatCharacters().ForEach(delegate (CombatCharacter character)
        {
            if (!character.IsDeath())
            {
                int speedNeed = character.GetSpeedNeeded();
                speedAdd = Math.Min(speedNeed, speedAdd);
            }
        });

        return Math.Max(speedAdd, 0);
    }

    CombatCharacter AddSpeedAndGetActionCharacter(int speedAdd)
    {
        List<CombatCharacter> listAvaiableCharacter = new List<CombatCharacter>();

        GetAllCombatCharacters().ForEach(delegate (CombatCharacter character)
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
        return listAvaiableCharacter.First();
    }
    List<CombatCharacter> GetAllCombatCharacters()
    {
        List<CombatCharacter> result = new List<CombatCharacter>();
        combatState.charactersTeamA.ForEach(delegate (CombatCharacter character)
        {
            result.Add(character);
        });
        combatState.charactersTeamB.ForEach(delegate (CombatCharacter character)
        {
            result.Add(character);
        });
        return result;
    }
    // create character on screen
    GameObject CreateCharacter(CombatCharacter character)
    {
        UnityEngine.Vector3 p = GameObject.Find("slot_" + (character.team == Team.A ? "l" : "r") + "_" + character.position.ToString()).transform.position;
        GameObject c = Instantiate(characterFrefab, p, Quaternion.identity);
        GameObject child = c.transform.Find("sword_man").gameObject;
        child.GetComponent<Billboard>().cam = MainCamera.transform;
        child.transform.localScale = new Vector3(character.team == Team.A ? -100f : 100f, 100f, 100f);
        return c;
    }
}
