using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCTutOpenSlot : MonoBehaviour
{
    private int step = 0;
    private bool block = true;
    [SerializeField]
    private Text text;

    [SerializeField]
    private Transform bubble;

    [SerializeField]
    private Transform npc;
    private void Start()
    {
        StartCoroutine(UnlockClick(2));
    }
    IEnumerator UnlockClick(float time)
    {
        yield return new WaitForSeconds(time);
        block = false;
    }
    public void OnClickPanel()
    {
        Debug.Log("click");
        if (!block)
        {
            step += 1;
            switch (step)
            {
                case 2:
                    {
                        Sequence s = DOTween.Sequence();
                        var curX = npc.position.x;
                        npc.DOMoveX(curX + 1000, 0.4f).SetEase(Ease.InSine);
                        s.AppendCallback(() =>
                        {
                            Destroy(gameObject);
                            var lobbyUI = GameObject.Find("PickTeamController").GetComponent<PickTeamUI>();
                            lobbyUI.ShowFocusOpenSlot();
                        });
                        block = true;
                        break;
                    } 
            }
        }
    }
}
