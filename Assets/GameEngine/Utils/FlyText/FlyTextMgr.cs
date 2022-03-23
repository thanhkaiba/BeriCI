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
        v3.y += 30;
        if (damage.physics > 0)
        {
            v3.x -= 40;
            GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
            c.GetComponent<TextDamage>().Present(damage.physics, v3, damage.isCrit, new Color(0.93f, 0.18f, 0.24f));
        }
        if (damage.magic > 0)
        {
            v3.x += 40;
            v3.y += 20;
            GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
            c.GetComponent<TextDamage>().Present(damage.magic, v3, damage.isCrit, new Color(0.58f, 0.72f, 0.92f));
        }
        if (damage.pure > 0)
        {
            v3.y -= 20;
            GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
            c.GetComponent<TextDamage>().Present(damage.pure, v3, damage.isCrit, Color.white);
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
        c.GetComponent<TextDamage>().iconCrit.SetActive(false);
    }
}
