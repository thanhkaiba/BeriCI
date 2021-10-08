using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiMgr : MonoBehaviour
{
    public static GuiMgr Instance { get; private set; }
    public Canvas sceneCanvas;
    private void Awake()
    {
        Instance = this;
    }
    public GameObject ShowPopup (string src)
    {
        var popup = Instantiate(Resources.Load<GameObject>(src), sceneCanvas.transform);
        return popup;
    }
}
