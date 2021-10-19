using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColor : MonoBehaviour
{
    public void ShowClassBonus(List<ClassBonusItem> passiveTypeA, List<ClassBonusItem> passiveTypeB)
    {
      /*  GameUtils.CalculateClassBonus(sailorsTeamA)
        Transform nodeLeft = transform.FindDeepChild("NodeTypeLeft");
        Transform nodeRight = transform.FindDeepChild("NodeTypeRight");

        for (int i = 0; i < passiveTypeA.Count; i++)
        {
            GameObject GO = Resources.Load<GameObject>("Icons/SailorType/combine");
            IconClassBonus s = Instantiate(GO, nodeLeft).GetComponent<IconClassBonus>();
            s.SetData(passiveTypeA[i]);
            s.transform.DOLocalMoveY(-128 * i, 0.5f);
        }

        for (int i = 0; i < passiveTypeB.Count; i++)
        {
            GameObject GO = Resources.Load<GameObject>("Icons/SailorType/combine");
            IconClassBonus s = Instantiate(GO, nodeRight).GetComponent<IconClassBonus>();
            s.SetData(passiveTypeB[i]);
            s.transform.DOLocalMoveY(-128 * i, 0.5f);
        }*/
    }
}
