using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.Utils
{
  
    public class UnicodeTextField: MonoBehaviour
    {
        [SerializeField]
        private InputField inputField;
        private string ime;
    

        private void Start()
        {
            inputField.onEndEdit.AddListener(arg =>
            {
                if (!string.IsNullOrEmpty(ime))
                {
                    inputField.text = ime;
                }
                ime = "";
            });
        }

        private void Update()
        {
            if (Input.compositionString != "")
            {
                ime = inputField.text;
                ime = ime.Insert(inputField.caretPosition - Input.compositionString.Length, Input.compositionString);
            }
        }
    }
}
