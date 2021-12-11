using UnityEngine;
using UnityEngine.UI;
using Piratera.Sound;

namespace Piratera.GUI
{
    class HaveButtonTapSound : MonoBehaviour
    {
        private void Start()
        {
            Button button = GetComponent<Button>();
            button.onClick.AddListener(PlaySound);
        }

        private void PlaySound()
        {
            SoundMgr.PlayTabSound();
        }
    }
}
