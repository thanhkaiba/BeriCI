using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyTextMgr : MonoBehaviour
{
    public GameObject damageText;
    public Camera cam;
    public static FlyTextMgr Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameEvents.Instance.takeDamage.AddListener(ShowTextTakeDamage);
    }
    public void ShowTextTakeDamage(CombatSailor s, Damage damage)
    {
        Vector3 v3 = cam.GetComponent<Camera>().WorldToScreenPoint(s.transform.position);
        if (damage.physics_damage > 0)
        {
            GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
            c.GetComponent<TextDamage>().Present(damage.physics_damage, v3, damage.isCrit, Color.red);
        }
        if (damage.magic_damage > 0)
        {
            GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
            c.GetComponent<TextDamage>().Present(damage.physics_damage, v3, damage.isCrit, Color.blue);
        }
        if (damage.true_damage > 0)
        {
            GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
            c.GetComponent<TextDamage>().Present(damage.physics_damage, v3, damage.isCrit, Color.white);
        }
    }

    public void CreateFlyTextWith3DPosition(string text, Vector3 p)
    {
        //Debug.Log("=>>>>>>>p " + p);
        Vector3 v3 = cam.GetComponent<Camera>().WorldToScreenPoint(p);
        CreateFlyText(text, v3);
    }
    public void CreateFlyText(string text, Vector3 p)
    {
        //Debug.Log("text " + text);
        //Debug.Log("p " + p);
        GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
        c.GetComponent<Text>().text = text;
        //c.GetComponent<Text>().fontSize = 50;
        c.transform.position = p;
    }
}
