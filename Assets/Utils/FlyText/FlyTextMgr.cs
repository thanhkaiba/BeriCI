using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyTextMgr : MonoBehaviour
{
    public GameObject damageText;
    public Camera cam;
    public static FlyTextMgr Instance { get; private set; }
    public void Awake()
    {
        Debug.Log("FIRST TIME APPEAR");
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void CreateFlyTextWith3DPosition(string text, Vector3 p)
    {
        Debug.Log("=>>>>>>>p " + p);
        Vector3 v3 = cam.GetComponent<Camera>().WorldToScreenPoint(p);
        CreateFlyText(text, v3);
    }
    public void CreateFlyText(string text, Vector3 p)
    {
        Debug.Log("text " + text);
        Debug.Log("p " + p);
        GameObject c = Instantiate(damageText, Vector3.zero, Quaternion.identity, transform);
        c.GetComponent<Text>().text = text;
        c.transform.position = p;
    }
}
