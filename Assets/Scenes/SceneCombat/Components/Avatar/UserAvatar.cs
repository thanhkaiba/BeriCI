using System;
using UnityEngine;
using UnityEngine.UI;

public class UserAvatar : MonoBehaviour
{
    private Image userAvatar;

    [SerializeField]
    private Sprite defaultAvt;

    private void Awake()
    {
        userAvatar = GetComponent<Image>();
    }
    public void ShowAvatar(int id = 0)
    {
        userAvatar.sprite = Resources.Load<Sprite>("Icons/Avatar/" + id);
    }
    public void LoadAvatar(string url)
    {

        if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
        {
            DoLoadAvatar(url);
        }
        else if (string.IsNullOrEmpty(url))
        {
            SetDefaultAvatar();
        }
        else
        {
            Sprite s = GameUtils.GetSailorAvt(url);
            if (s != null)
            {
                userAvatar.sprite = s;
            }
            else
            {
                SetDefaultAvatar();
            }
        }
    }
    private void DoLoadAvatar(string url)
    {
        Davinci.get()
         .load(url)
         .into(userAvatar)
         .withErrorAction(OnLoadImageError)
         .setFadeTime(0.2f)
         .setCached(true)
         .start();
    }
    private void OnLoadImageError(string err)
    {
        SetDefaultAvatar();
    }

    private void SetDefaultAvatar()
    {
        userAvatar.sprite = defaultAvt;
    }
}
