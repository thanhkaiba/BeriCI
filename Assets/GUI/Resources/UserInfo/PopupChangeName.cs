using DG.Tweening;
using Piratera.GUI;
using Piratera.Network;
using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PopupChangeName : MonoBehaviour
{
    private int cost = 2000;
    private bool waitResponse = false;
    [SerializeField]
    public Text errorText, textCost, textTotalBeri;
    [SerializeField]
    public InputField inputField;
    [SerializeField]
    public Button btnChange;
    [SerializeField]
    public Transform background;
    private void Start()
    {
        inputField.text = UserData.Instance.Username.ToLower();
        textCost.text = cost.ToString("N0");
        textTotalBeri.text = "You have " + UserData.Instance.Beri.ToString("N0") + " beri";
        errorText.text = "";
    }
    private void Awake()
    {
        NetworkController.Listen(OnReceiveServerAction);
        Appear();
        inputField.onValueChanged.AddListener(text =>
        {
            inputField.text = text.ToLower();
        });
    }
    private void Appear()
    {
        Sequence s = DOTween.Sequence();
        var canvasGroup = background.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.DOFade(1, 0.2f);
        s.AppendCallback(() => canvasGroup.interactable = true);

        background.localScale = new Vector3(0.6f, 0.6f, 0.6f);
        background.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).SetLink(background.gameObject).SetTarget(background.transform);

        var fog = GetComponent<HaveFog>();
        if (fog) fog.FadeIn(0.3f);
    }
    private void OnDestroy()
    {
        NetworkController.RemoveListener(OnReceiveServerAction);
    }
    private void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {
        switch (action)
        {
            case SFSAction.USER_CHANGE_NAME:
                waitResponse = false; if (errorCode == SFSErrorCode.SUCCESS)
                {
                    GuiManager.Instance.ShowPopupNotification("You've changed your name successfully");
                    Close();
                }
                else if (errorCode == SFSErrorCode.NICK_NAME_EXIST)
                {
                    errorText.text = "Name existed";
                }
                else 
                {
                    errorText.text = "Unknown Error";
                }
                break;
        }
    }
    private bool ValidateName()
    {
        return new Regex("^[A-Za-z0-9_ .]+$").IsMatch(inputField.text);
    }
    public void ClickBuy()
    {
        var text = inputField.text.Trim().ToLower();
        inputField.text = text;
        if (UserData.Instance.Beri < cost)
            errorText.text = "You do not have enough beri";
        else if (text == "")
            errorText.text = "Crew's name cannot be blank";
        else if (text.Length < 2)
            errorText.text = "Crew's name need atleast 2 characters";
        else if (text.Length > 12)
            errorText.text = "Crew's name cannot exceed 12 characters";
        else if (!ValidateName())
            errorText.text = "Crew's name cannot contain special character";
        else
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutUtfString("new_name", text);
            Debug.Log("Send change name: " + text);
            NetworkController.Send(SFSAction.USER_CHANGE_NAME, sfsObject);
            waitResponse = true;
        }
    }
    public void Close()
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        var curText = inputField.text;
        bool enable = curText.Trim().ToLower() != UserData.Instance.Username.Trim().ToLower();
        btnChange.enabled = !waitResponse && enable;
        btnChange.GetComponent<CanvasGroup>().alpha = enable ? 1 : 0.6f;
    }
}
