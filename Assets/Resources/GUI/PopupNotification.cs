using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public delegate void PopupNotificationOKDelegate();
    public class PopupNotification : MonoBehaviour
    {
        [SerializeField]
        private Text textNotification;
        private PopupNotificationOKDelegate OKFunc;

        public void OnOK()
        {
            if (OKFunc != null)
            {
                OKFunc();
            }
            Destroy(gameObject);
        }


        public void SetData(string text)
        {
            textNotification.text = text;
        }

        public void SetData(string text, PopupNotificationOKDelegate okFunc)
        {
            SetData(text);
            OKFunc = okFunc;
        }
        
    }
}


