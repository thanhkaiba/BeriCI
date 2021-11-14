using UnityEngine;

namespace Piratera.GUI
{
    public class GuiManager : Singleton<GuiManager>
    {
        public GameObject AddGui(string prefap, LayerId layer = LayerId.GUI)
        {

            return AddGui(Resources.Load(prefap) as GameObject, layer);
        }

        public GameObject AddGui(GameObject prefap, LayerId layer = LayerId.GUI)
        {
            GuiLayerSystem guiLayerSystem = FindObjectOfType<GuiLayerSystem>();

            if (guiLayerSystem != null)
            {
                return guiLayerSystem.AddGui(prefap, layer);

            }
            return null;
        }

        public GameObject ShowPopupNotification(string text)
        {
            GameObject gameObject = AddGui("GUI/Prefap/PopupNotificaiton", LayerId.POPUP);
            PopupNotification popup = gameObject.GetComponent<PopupNotification>();
            popup.SetData(text);
            return gameObject;
        }

        public GameObject ShowPopupNotification(string text, PopupNotificationOKDelegate oKDelegate)
        {
            GameObject gameObject = AddGui("GUI/Prefap/PopupNotificaiton", LayerId.POPUP);
            PopupNotification popup = gameObject.GetComponent<PopupNotification>();
            popup.SetData(text, oKDelegate);
            return gameObject;
        }
    }
}
