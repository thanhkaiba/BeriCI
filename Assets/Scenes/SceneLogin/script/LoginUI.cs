using UnityEngine;
using UnityEngine.UI;
using Piratera.Utils;
using DG.Tweening;

public class LoginUI : MonoBehaviour
{
    [SerializeField]
    private Image loginBox;
    [SerializeField]
    private Image chain;
    [SerializeField]
    private Image logo;
    private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<Canvas>();
		RunAppearAction();
    }

	private void RunAppearAction()
	{
        float scale = canvas.transform.localScale.x;
        float chainHeight = ((RectTransform)chain.transform).sizeDelta.y;
        float loginBoxHeight = ((RectTransform)loginBox.transform).sizeDelta.y;



        DoTweenUtils.FadeAppearY(chain,  chainHeight * scale, 0.6f, 0.2f ,Ease.OutFlash);


        DoTweenUtils.FadeAppearY(loginBox, (chainHeight + loginBoxHeight) * scale, 1f, Ease.OutFlash);

        DoTweenUtils.FadeAppear(logo, 0.5f, 1.2f);
    }
}
