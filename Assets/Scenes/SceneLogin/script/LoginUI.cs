using UnityEngine;
using UnityEngine.UI;
using Piratera.Utils;
using DG.Tweening;

public class LoginUI : MonoBehaviour
{
    [SerializeField]
    private Image loginBox;
    [SerializeField]
    private CanvasGroup chain;
    [SerializeField]
    private Image[] monters;
    [SerializeField]
    private Image buttonLogin;

    [SerializeField]
    private Image buttonSingup;

    [SerializeField]
    private Image[] textboxs;

    [SerializeField]
    private Image textOr;

    [SerializeField]
    private Text[] textboxlbs;

    // Start is called before the first frame update
    void Start()
    {
		RunAppearAction();
    }

	private void RunAppearAction()
	{
     
        Sequence seq = DOTween.Sequence();
        RectTransform chainTrasform = (chain.transform as RectTransform);
        seq.Insert(1, chainTrasform.DOAnchorPosY(chainTrasform.anchoredPosition.y + 500, 0.6f).From().SetEase(Ease.OutQuint));
        seq.Insert(1, chain.DOFade(0, 0.6f).From());
        seq.Insert(1, chain.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.6f).From());


        RectTransform slotCapacityRect = loginBox.transform as RectTransform;
        float width = slotCapacityRect.sizeDelta.x;
        slotCapacityRect.sizeDelta -= new Vector2(width, 0);
        seq.Insert(0, loginBox.DOFade(0, 0.7f).From());
        seq.Insert(0, DOTween.To(x => slotCapacityRect.sizeDelta = new Vector2(x, slotCapacityRect.sizeDelta.y), 0, width, 0.7f).SetTarget(transform).SetLink(gameObject).SetEase(Ease.Unset));
        seq.Insert(0, slotCapacityRect.DOAnchorPosX(slotCapacityRect.anchoredPosition.x - width / 2, 0.5f).From());

        foreach (Image element in textboxs)
        {
            
            Sequence textBoxSeq = DOTween.Sequence();
            element.transform.localScale -= new Vector3(element.transform.localScale.x, 0, 0);
            textBoxSeq.PrependInterval(Random.Range(0.4f, 0.6f));
            textBoxSeq.Append(element.transform.DOScaleX(1, 0.2f));
            seq.Insert(1, textBoxSeq);
        }

        buttonLogin.color -= new Color(0, 0, 0, buttonLogin.color.a);
        Sequence buttonSeq = DOTween.Sequence();
        buttonSeq.Insert(0, buttonLogin.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f, 10, 0));
        buttonSeq.Insert(0, buttonLogin.DOFade(1, 0.4f));
        seq.Insert(2, buttonSeq);

        seq.AppendInterval(0.2f);
        seq.Append(textOr.DOFade(0, 0.2f).From().SetEase(Ease.OutBack));
        Sequence buttonSeq2 = DOTween.Sequence();
        buttonSingup.color -= new Color(0, 0, 0, buttonSingup.color.a);
        buttonSeq2.Insert(0, buttonSingup.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f, 10, 0));
        buttonSeq2.Insert(0, buttonSingup.DOFade(1, 0.4f));
        buttonSeq2.Insert(0, buttonSingup.transform.DOMoveY(-200, 0.4f).From());
        seq.Append(buttonSeq2);
       



        foreach (Image monter in monters)
        {
            seq.Insert(1, monter.DOFade(0, 0.2f).From());
            seq.Insert(1, monter.transform.DOScale(new Vector3(0, 0, 0), 0.4f).From().SetEase(Ease.OutQuad));
        }

        seq.SetTarget(transform).SetLink(gameObject);

    }
}
