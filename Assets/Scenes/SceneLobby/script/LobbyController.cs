using UnityEngine;
using Sfs2X.Entities.Data;

public class LobbyController : MonoBehaviour
{


	public static LobbyController Instance;
	private void Awake() { Instance = this; }
	private void OnDestroy() { Instance = null; }
	private void Start()
    {
		NetworkController.Instance.Send(SFSAction.LOAD_LIST_HERO_INFO);
	}
	

	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------
	public void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
	{
		switch (action)
		{
			case SFSAction.LOAD_LIST_HERO_INFO:
				{
					if (errorCode == SFSErrorCode.SUCCESS)
					{
						Debug.Log("Load List hero success");
						CrewData.Instance.NewFromSFSObject(packet);
					}
					break;
				}
		}
	}

   
}
