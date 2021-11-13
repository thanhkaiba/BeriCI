using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserAvatar : MonoBehaviour
{
	[SerializeField]
	private Image userAvatar;

	[SerializeField]
	private Sprite defaultAvt;
	public void LoadAvatar(string url)
	{
		StartCoroutine(DoLoadAvatar(url));
	}
	private IEnumerator DoLoadAvatar(string url)
	{
		if (url == "") SetDefaultAvatar();
		else using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
		{
			yield return uwr.SendWebRequest();

			if (uwr.result != UnityWebRequest.Result.Success)
			{
				Debug.Log(uwr.error);
				SetDefaultAvatar();
			}
			else
			{
				// Get downloaded asset bundle
				Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
				userAvatar.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

			}
		}
	}
	private void SetDefaultAvatar()
	{
		userAvatar.sprite = defaultAvt;
	}
}
