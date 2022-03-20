using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCTut : MonoBehaviour
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
                case 1:
                    {
                        Sequence s = DOTween.Sequence();
                        var canvasGroup = bubble.GetComponent<CanvasGroup>();
                        s.Append(canvasGroup.DOFade(0, 0.4f));
                        s.AppendCallback(() => text.text = "Sail with me and fight against other crews...");
                        s.Append(canvasGroup.DOFade(1, 0.2f));
                        block = true;
                        StartCoroutine(UnlockClick(2));
                        break;
                    } 
                case 2:
                    {
                        Sequence s = DOTween.Sequence();
                        var curX = npc.position.x;
                        npc.DOMoveX(curX + 1000, 0.4f).SetEase(Ease.InSine);
                        s.AppendCallback(() =>
                        {
                            Destroy(gameObject);
                            var lobbyUI = GameObject.Find("LobbyManager").GetComponent<LobbyUI>();
                            lobbyUI.ShowTutOpenCrew();
                        });
                        block = true;
                        break;
                    } 
            }
        }
    }
}
