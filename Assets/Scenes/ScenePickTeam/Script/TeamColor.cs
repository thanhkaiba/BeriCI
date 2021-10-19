using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColor : MonoBehaviour
{
    public void ShowClassBonus(List<Sailor> squad)
    {
        List<ClassBonusItem> passiveType = GameUtils.CalculateClassBonus(squad);

        for (int i = 0; i < passiveType.Count; i++)
        {
            GameObject icon = Resources.Load<GameObject>("Icons/SailorType/combine");
            IconClassBonus s = Instantiate(icon, transform).GetComponent<IconClassBonus>();
            s.SetData(passiveType[i]);
        }
    }
}
