using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamColor : MonoBehaviour
{
    public bool teamA;

    public void ShowClassBonus(List<ClassBonusItem> passiveType)
    {
        ClearChildren();
        int count1 = 0;
        int count2 = 0;
        for (int i = 0; i < passiveType.Count; i++)
        {
            var item = passiveType[i];
            GameObject icon = Resources.Load<GameObject>("Icons/SailorType/combine");
            IconClassBonus s = Instantiate(icon, transform).GetComponent<IconClassBonus>();
            Vector3 pos = s.transform.GetChild(0).transform.localPosition;
            s.transform.GetChild(0).transform.localPosition = new Vector3(teamA ? pos.x : -pos.x, pos.y, pos.z);
            s.SetData(passiveType[i]);
            if (item.level >= 0) s.transform.localPosition = new Vector3(0, -120 * count1++, 0);
            else
            {
                var offsetX = count2 >= 5 ? 84 : 0;
                s.transform.localPosition = new Vector3(offsetX + (count1 == 0 ? 0 : 86), 10 - 92 * (count2%5), 0);
                s.transform.localScale = Vector3.one / 1.1f;
                count2++;
            }
        }
    }

    public void ClearChildren()
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }

    }
}
