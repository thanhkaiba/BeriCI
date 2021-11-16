using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class GuiWaiting : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        icon.transform.Rotate(new Vector3(0, 0, 70 * Time.deltaTime));
    }
}
