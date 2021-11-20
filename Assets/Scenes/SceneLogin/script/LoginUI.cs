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

    // Start is called before the first frame update
    void Start()
    {
		
		RunAppearAction();
	}

	private void RunAppearAction()
	{

        DoTweenUtils.FadeAppearY(chain, ((RectTransform)chain.transform).sizeDelta.y, 0.7f, Ease.InExpo);


        DoTweenUtils.FadeAppearY(loginBox, ((RectTransform)chain.transform).sizeDelta.y + ((RectTransform)loginBox.transform).sizeDelta.y, 1f, Ease.OutFlash);

        DoTweenUtils.FadeAppear(logo, 0.5f, 0.8f);
    }
}
