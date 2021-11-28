using System;
using UnityEngine;

namespace Piratera.GUI
{
    public class GuiManager : Singleton<GuiManager>
    {
        public GameObject AddGui<T>(string prefap, LayerId layer = LayerId.GUI)
        {

            return AddGui<T>(Resources.Load(prefap) as GameObject, layer);
        }

        public GameObject AddGui<T>(GameObject prefap, LayerId layer = LayerId.GUI)
        {
            GuiLayerSystem guiLayerSystem = FindObjectOfType<GuiLayerSystem>();

            if (guiLayerSystem != null)
            {
                return guiLayerSystem.AddGui(prefap, layer, typeof(T).Name);

            }
            return null;
        }


        public GameObject ShowPopupNotification(string text)
        {
            GameObject gameObject = AddGui<PopupNotification>("Prefap/PopupNotificaiton", LayerId.POPUP);
            PopupNotification popup = gameObject.GetComponent<PopupNotification>();
            popup.SetData(text);
            return gameObject;
        }

        public GameObject ShowPopupNotification(string text, Action oKDelegate)
        {
            GameObject gameObject = AddGui<PopupNotification>("Prefap/PopupNotificaiton", LayerId.POPUP);
            PopupNotification popup = gameObject.GetComponent<PopupNotification>();
            popup.SetData(text, oKDelegate);
            return gameObject;
        }

        public void ShowGuiWaiting(bool show)
        {
            if (show)
            {
                AddGui<GuiWaiting>("Prefap/GuiWaiting", LayerId.LOADING);
            } else
            {
                DestroyGui<GuiWaiting>();
            }
        }

        public void DestroyGui<T>()
        {
            DestroyGui(typeof(T).Name);
        }

        public void DestroyGui(string guiName)
        {
            GuiLayerSystem guiLayerSystem = FindObjectOfType<GuiLayerSystem>();

            if (guiLayerSystem != null)
            {
                guiLayerSystem.DestroyGui(guiName);
            }
        }
    }

}
