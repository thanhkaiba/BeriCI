using Piratera.GUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupLibrary : MonoBehaviour
{
    [SerializeField]
    private GameObject PopupSailor;
    public void ClickSailor()
    {
        // Application.OpenURL("https://piratera.fandom.com/wiki/Sailor");
        GuiManager.Instance.AddGui(PopupSailor);
    }
    public void ClickClasses()
    {
        Application.OpenURL("https://piratera.fandom.com/wiki/Classes");
    }
    public void ClickClose()
    {
        Destroy(gameObject);
    }
}
