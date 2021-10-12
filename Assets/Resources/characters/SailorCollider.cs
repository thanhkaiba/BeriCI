using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailorCollider : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (UIIngameMgr.Instance != null) UIIngameMgr.Instance.ShowSailorDetail(gameObject);
    }
}
