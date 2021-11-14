using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyTextMgr : MonoBehaviour
{
    public GameObject damageText;
    public static FlyTextMgr Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        CombatEvents.Instance.takeDamage.AddListener(ShowTextTakeDamage);
    }
    public void ShowTextTakeDamage(CombatSailor s, Damage damage)
    {
        Vector3 v3 = Camera.main.WorldToScreenPoint(s.transform.position);
        if (damage.physics_damage > 0)
        {
            GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
            c.GetComponent<TextDamage>().Present(damage.physics_damage, v3, damage.isCrit, new Color(0.93f, 0.18f, 0.24f));
        }
        if (damage.magic_damage > 0)
        {
            GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
            c.GetComponent<TextDamage>().Present(damage.magic_damage, v3, damage.isCrit, new Color(0.58f, 0.72f, 0.92f));
        }
        if (damage.true_damage > 0)
        {
            GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
            c.GetComponent<TextDamage>().Present(damage.true_damage, v3, damage.isCrit, Color.white);
        }
    }

    public void CreateFlyTextWith3DPosition(string text, Vector3 p)
    {
        //Debug.Log("=>>>>>>>p " + p);
        Vector3 v3 = Camera.main.GetComponent<Camera>().WorldToScreenPoint(p);
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
