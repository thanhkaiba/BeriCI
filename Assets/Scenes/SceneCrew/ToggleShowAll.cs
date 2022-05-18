using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleShowAll : MonoBehaviour
{
    void Start()
    {
        GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("is_show_fav", 0) == 1;
    }
    public void OnChange()
    {
        PlayerPrefs.SetInt("is_show_fav", GetComponent<Toggle>().isOn ? 1 : 0);
    }
}
