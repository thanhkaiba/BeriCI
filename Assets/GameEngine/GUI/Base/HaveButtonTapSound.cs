using Piratera.Sound;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Piratera.GUI
{
    class HaveButtonTapSound : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            SoundMgr.PlayTabSound();
        }
    }
}
