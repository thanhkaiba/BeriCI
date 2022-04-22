using Piratera.Constance;
using System;
using UnityEngine;

namespace Piratera.GUI
{
    public class GuiManager : Singleton<GuiManager>
    {
        public Transform GetCanvas()
        {
            return GameObject.Find("Canvas").transform;
        }
        public GameObject AddGui(GameObject prefap)
        {
            return Instantiate(prefap, GetCanvas().transform);
        }
        public GameObject AddGui(string prefapSrc)
        {
            return Instantiate(Resources.Load<GameObject>(prefapSrc), GetCanvas().transform);
        }
        public GameObject ShowPopupNotification(string text)
        {
            GameObject gameObject = AddGui("Prefap/PopupNotification");
            PopupNotification popup = gameObject.GetComponent<PopupNotification>();
            popup.SetData(text);
            return gameObject;
        }
        public GameObject ShowPopupNotification(string text, Action okAction)
        {
            GameObject gameObject = AddGui("Prefap/PopupNotification");
            PopupNotification popup = gameObject.GetComponent<PopupNotification>();
            popup.SetData(text, okAction);
            return gameObject;
        }
        public GameObject ShowPopupNotification(string text, string okText, Action okAction)
        {
            GameObject gameObject = AddGui("Prefap/PopupNotification");
            PopupNotification popup = gameObject.GetComponent<PopupNotification>();
            popup.SetData(text, okText, okAction);
            return gameObject;
        }
        public GameObject ShowPopupNotification(string text, string okText, Action okAction, Action cancelAction)
        {
            GameObject gameObject = AddGui("Prefap/PopupNotification");
            PopupNotification popup = gameObject.GetComponent<PopupNotification>();
            popup.SetData(text, okText, okAction, cancelAction);
            return gameObject;
        }
        public void ShowPopupBuySailor()
        {
            ShowPopupNotification("You need a sailor to play", "BUY NOW", () => Application.OpenURL(GameConst.MARKET_URL), null);
        }
    }
}
