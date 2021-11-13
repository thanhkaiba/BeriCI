using UnityEngine;
using Sfs2X.Entities.Data;

public class LobbyController : MonoBehaviour
{
	private void OnDestroy() {
		NetworkController.RemoveServerActionListener(OnReceiveServerAction);
	}
	private void Awake()
    {
		NetworkController.Send(SFSAction.LOAD_LIST_HERO_INFO);
		NetworkController.AddServerActionListener(OnReceiveServerAction);
	}
	
	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------
	public void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
	{
	}
}
