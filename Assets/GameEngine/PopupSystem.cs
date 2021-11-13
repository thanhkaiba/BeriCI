using UnityEngine;
using UnityEngine.UI;

public class PopupSystem : MonoBehaviour
{
    public GameObject popuUpBox;
    public Animator animator;
    public Text popupText;

    public void PopUp(string text)
    {
        popuUpBox.SetActive(true);
        popupText.text = text;
        animator.SetTrigger("pop");
    }
}
