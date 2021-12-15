using Sfs2X.Entities.Data;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Piratera.GUI
{
    public class GuiConfirmPVE : BaseGui
    {

        [SerializeField]
        private Text textCurrentStamina;

  
        private void OnEnable()
        {
            base.Start();
            UpdateCurrentStamina();
            GameEvent.UserStaminaChanged.AddListener(UpdateCurrentStamina);
        }

        private void OnDisable()
        {
            GameEvent.UserStaminaChanged.RemoveListener(UpdateCurrentStamina);
        }

        private void UpdateCurrentStamina(int arg0, int arg1)
        {
            UpdateCurrentStamina();
        }


        private void UpdateCurrentStamina()
        {
            textCurrentStamina.text = UserData.Instance.GetStamina().ToString();          
        }

    }
}
