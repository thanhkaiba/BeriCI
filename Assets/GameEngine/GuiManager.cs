using UnityEngine;

namespace Piratera.GUI
{
    public class GuiManager : Singleton<GuiManager>
    {
        public GameObject AddGui(string prefap)
        {

            return AddGui(Instantiate(Resources.Load(prefap) as GameObject));
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

        public GameObject ShowPopupNotification(string text)
        {
            GameObject gameObject = AddGui("GUI/Prefap/PopupNotificaiton");
            PopupNotification popup = gameObject.GetComponent<PopupNotification>();
            popup.SetData(text);
            return gameObject;
        }

        public GameObject ShowPopupNotification(string text, PopupNotificationOKDelegate oKDelegate)
        {
            GameObject gameObject = AddGui("GUI/Prefap/PopupNotificaiton");
            PopupNotification popup = gameObject.GetComponent<PopupNotification>();
            popup.SetData(text, oKDelegate);
            return gameObject;
        }
    }
}
