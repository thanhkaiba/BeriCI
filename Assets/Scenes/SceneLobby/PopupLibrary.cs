using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupLibrary : MonoBehaviour
{
    public void ClickSailor()
    {
        Application.OpenURL("https://piratera.fandom.com/wiki/Sailor");
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
