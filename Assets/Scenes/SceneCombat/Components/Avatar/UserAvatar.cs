using Piratera.Config;
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
            SailorConfig2 config_stats = GlobalConfigs.GetSailorConfig(url);
            if (config_stats != null)
            {
                userAvatar.sprite = GameUtils.GetSailorAvt(config_stats.root_name);
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
