using UnityEngine;
using UnityEngine.UI;

public class IconClassInAvt : MonoBehaviour
{
    [SerializeField]
    public Image iconClass;
    public void SetClass(SailorClass s)
    {
        iconClass.sprite = Resources.Load<Sprite>("Icons/SailorType/" + s);
    }
}
