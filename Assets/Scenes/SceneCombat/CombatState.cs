using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CombatStatus
{
    PREPARING,
    STARTED,
    ENDED
};

public class CombatState
{
    public CombatStatus status;
    public List<CombatCharacter> charactersTeamA = new List<CombatCharacter>();
    public List<CombatCharacter> charactersTeamB = new List<CombatCharacter>();
    public CombatState()
    {
        status = CombatStatus.PREPARING;
        this.createDemoTeam();
    }
    private void createDemoTeam()
    {
        // test tao team
        for (int i = 0; i < 5; i++)
        {
            charactersTeamA.Add(new CombatCharacter("A " + i, 50, 500, 100, 10, CharacterType.SHIPWRIGHT, i, Team.A));
            charactersTeamB.Add(new CombatCharacter("A " + i, 50, 500, 100, 10, CharacterType.SHIPWRIGHT, i, Team.B));
        }
    }

};
