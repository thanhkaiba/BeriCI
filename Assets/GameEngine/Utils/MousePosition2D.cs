using UnityEngine;

public class MousePosition2D : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    void Update()
    {
        Debug.Log(mainCamera.ScreenToWorldPoint(Input.mousePosition));
    }
}
