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
        Canvas canvas = FindObjectOfType<Canvas>(); 

        if (canvas != null)
        {
            GameObject gui = Instantiate(prefap, canvas.transform);
            return gui;

        }
        return null;
    }
}
