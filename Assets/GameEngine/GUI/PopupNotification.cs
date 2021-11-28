using System;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class PopupNotification : BaseGui
    {
        [SerializeField]
        private Text textNotification;
        private Action OKFunc;

        public void OnOK()
        {
            if (OKFunc != null)
            {
                OKFunc();
            }
            RunDestroy();
        }


        public void SetData(string text)
        {
            textNotification.text = text;
        }

        public void SetData(string text, Action okFunc)
        {
            SetData(text);
            OKFunc = okFunc;
        }
        
    }
}


