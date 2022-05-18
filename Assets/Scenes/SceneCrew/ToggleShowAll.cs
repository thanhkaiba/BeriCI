using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleShowAll : MonoBehaviour
{
    public Toggle toggle;
    void Awake()
    {
        try
        {
            toggle.isOn = PlayerPrefs.GetInt("is_show_fav" + UserData.Instance.UID, 0) == 1;
        }
        catch
        {

        }
    }
    public void OnChange()
    {
        PlayerPrefs.SetInt("is_show_fav", GetComponent<Toggle>().isOn ? 1 : 0);
    }
}
