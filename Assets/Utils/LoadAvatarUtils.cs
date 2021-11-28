using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Piratera.Utils
{
    public class LoadAvatarUtils: MonoBehaviour
    {
		private IEnumerator StartLoadAvatar(Image userAvatar, string url)
		{
			using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
			{
				yield return uwr.SendWebRequest();

				if (uwr.result != UnityWebRequest.Result.Success)
				{
					Debug.Log(uwr.error);
				}
				else
				{
					// Get downloaded asset bundle
					Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
					userAvatar.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

				}
			}
		}

		public void LoadAvatar(Image userAvatar, string url)
		{
			if (string.IsNullOrEmpty(url))
            {
				return;
            }
			StartCoroutine(StartLoadAvatar(userAvatar, url));
		}
	}


}
