using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
		} else if (string.IsNullOrEmpty(url))
        {
			SetDefaultAvatar();
		} else
        {
			SailorConfig config_stats = Resources.Load<SailorConfig>("ScriptableObject/Sailors/" + url);
			if (config_stats != null)
            {
				userAvatar.sprite = config_stats.avatar;
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
