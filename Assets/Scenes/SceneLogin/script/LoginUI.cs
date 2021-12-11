using UnityEngine;
using UnityEngine.UI;
using Piratera.Utils;
using DG.Tweening;
using Piratera.Sound;

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
    private Image buttonSignup;

    [SerializeField]
    private Image[] textboxs;

    [SerializeField]
    private Image textOr;

    [SerializeField]
    private Text[] textboxlbs;

    // Start is called before the first frame update
    void Start()
    {
        SoundMgr.PlayBGMusic(PirateraMusic.LOGIN);
		RunAppearAction();
    }

	private void RunAppearAction2()
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
        buttonSignup.color -= new Color(0, 0, 0, buttonSignup.color.a);
        buttonSeq2.Insert(0, buttonSignup.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f, 10, 0));
        buttonSeq2.Insert(0, buttonSignup.DOFade(1, 0.4f));
        buttonSeq2.Insert(0, buttonSignup.transform.DOMoveY(-200, 0.4f).From());
        seq.Append(buttonSeq2);
       



        foreach (Image monter in monters)
        {
            seq.Insert(1, monter.DOFade(0, 0.2f).From());
            seq.Insert(1, monter.transform.DOScale(new Vector3(0, 0, 0), 0.4f).From().SetEase(Ease.OutQuad));
        }

        seq.SetTarget(transform).SetLink(gameObject);

    }
    private void RunAppearAction()
    {
        {
            Sequence seq = DOTween.Sequence();
            RectTransform chainTrasform = (chain.transform as RectTransform);
            seq.Insert(1.0f, chainTrasform.DOAnchorPosY(chainTrasform.anchoredPosition.y + 500, 0.6f).From().SetEase(Ease.OutQuint));
            seq.Insert(1.0f, chain.DOFade(0, 0.6f).From());
            seq.Insert(1.0f, chain.transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.6f).From());
        }
        {
            Sequence seq = DOTween.Sequence();
            var image = loginBox.GetComponent<CanvasGroup>();
            seq.Append(image.DOFade(0, 0));
            seq.AppendInterval(0.0f);
            seq.Append(image.DOFade(1, 0.4f));
        }
        foreach (Image element in textboxs)
        {
            Sequence seq = DOTween.Sequence();
            element.transform.localScale -= new Vector3(element.transform.localScale.x, 0, 0);
            seq.AppendInterval(0.2f);
            seq.Append(element.transform.DOScaleX(1, 0.4f));
        }
        {
            buttonLogin.color -= new Color(0, 0, 0, buttonLogin.color.a);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.2f);
            seq.AppendCallback(() => buttonLogin.DOFade(1, 0.4f));
            seq.Append(buttonLogin.transform.DOScale(0.4f, 0));
            seq.Append(buttonLogin.transform.DOScale(1, 0.4f));
        }
        {
            buttonSignup.color -= new Color(0, 0, 0, buttonSignup.color.a);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.2f);
            seq.AppendCallback(() => buttonSignup.DOFade(1, 0.4f));
            seq.Append(buttonSignup.transform.DOScale(0.4f, 0));
            seq.Append(buttonSignup.transform.DOScale(1, 0.4f));
        }
        foreach (Image monter in monters)
        {
            Sequence seq = DOTween.Sequence();
            seq.Insert(0.2f, monter.DOFade(0, 0.2f).From());
            seq.Insert(0.2f, monter.transform.DOScale(new Vector3(0, 0, 0), 0.4f).From().SetEase(Ease.OutQuad));
        }
    }
}
