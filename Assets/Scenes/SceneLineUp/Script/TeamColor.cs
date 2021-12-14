using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamColor : MonoBehaviour
{
    public bool teamA;

    public void ShowClassBonus(List<ClassBonusItem> passiveType)
    {
  

        ClearChildren();

        for (int i = 0; i < passiveType.Count; i++)
        {
            GameObject icon = Resources.Load<GameObject>("Icons/SailorType/combine");
            IconClassBonus s = Instantiate(icon, transform).GetComponent<IconClassBonus>();
            Vector3 pos = s.transform.GetChild(0).transform.localPosition;          
            s.transform.GetChild(0).transform.localPosition = new Vector3(teamA ? pos.x : -pos.x, pos.y, pos.z);
            s.SetData(passiveType[i]);
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
