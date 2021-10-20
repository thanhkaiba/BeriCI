using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColor : MonoBehaviour
{
    public void ShowClassBonus(List<Sailor> squad)
    {
        List<ClassBonusItem> passiveType = GameUtils.CalculateClassBonus(squad);

        IconClassBonus[] icons = gameObject.GetComponentsInChildren<IconClassBonus>();

        for (int i = 0; i < passiveType.Count; i++)
        {
            IconClassBonus s;
            if (i >= icons.Length)
            {
                GameObject icon = Resources.Load<GameObject>("Icons/SailorType/combine");
                s = Instantiate(icon, transform).GetComponent<IconClassBonus>();
            } else
            {
                s = icons[i];
            }
            
            s.SetData(passiveType[i]);
        }
    }
}
