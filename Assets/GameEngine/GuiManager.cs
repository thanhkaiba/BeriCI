using UnityEngine;

public class GuiManager : Singleton<GuiManager>
{
    public GameObject AddGui(string prefap)
    {
        GameObject gui = Resources.Load(prefap) as GameObject;
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            gui.transform.SetParent(canvas.transform);
        }
        return gui;
    }

    public GameObject AddGui(GameObject prefap)
    {
        GameObject gui = Instantiate(prefap);
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            gui.transform.SetParent(canvas.transform);
        }
        return gui;
    }
}
