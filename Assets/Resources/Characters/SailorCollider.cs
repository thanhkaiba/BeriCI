using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SailorCollider : MonoBehaviour
{
    private void OnMouseDown()
    {
        if (SceneManager.GetActiveScene().name == "ScenePickTeamBattle" && gameObject.transform.parent.name == "SquadA")
            return;
        if (TooltipSailorInfo.Instance != null) TooltipSailorInfo.Instance.ShowTooltip(gameObject);
    }
}
