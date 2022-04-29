using Piratera.GUI;
using UnityEngine;

public class SailorCollider : MonoBehaviour
{
    private Vector2 mouseBeginPos;
    private const float MOVE_OFFSET = 50;

    private bool ShowToolTip = true;
    private bool selected = false;
    private bool moved = false;

    private void OnMouseDown()
    {
        if (GameObject.Find("PanelFog")) return;
        mouseBeginPos = Input.mousePosition;
        selected = true;
        moved = false;
    }
    private void OnMouseUp()
    {
        if (selected && !moved)
        {
            if (TooltipSailorInfo.Instance != null && ShowToolTip)
            {
                TooltipSailorInfo.Instance.ShowTooltip(gameObject);
            }
        }
        selected = false;
        moved = false;
    }
    private void OnMouseDrag()
    {
        if (Vector3.Distance(mouseBeginPos, Input.mousePosition) >= MOVE_OFFSET)
        {
            moved = true;
            if (TooltipSailorInfo.Instance != null)
            {
                TooltipSailorInfo.Instance.gameObject.SetActive(false);
            }
        }
    }
}
