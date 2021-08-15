using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShadow : MonoBehaviour
{
    GameObject ch;
    public void SetCharacter(GameObject character)
    {
        ch = character;
    }
    void LateUpdate()
    {
        if (!ch.activeSelf)
        {
            gameObject.SetActive(false);
        }
        Vector3 charP = ch.transform.position;
        this.transform.position = new Vector3(charP.x, 0, charP.z+0.2f);
    }
}
