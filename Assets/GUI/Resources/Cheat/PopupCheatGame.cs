using Piratera.GUI;
using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupCheatGame : MonoBehaviour
{
    [SerializeField]
    private Text textCheatSailor_name; 
    [SerializeField]
    private Text textCheatSailor_level;
    [SerializeField]
    private Text textCheatSailor_quality;


    public void SendCheatSailor()
    {
        SFSObject data = new SFSObject();
        data.PutUtfString("cheat_text",
            textCheatSailor_name.text + "|"
            + textCheatSailor_level.text + "|"
            + textCheatSailor_quality.text);
        NetworkController.Send(SFSAction.CHEAT_SAILOR, data);
        NetworkController.Send(SFSAction.LOAD_LIST_HERO_INFO);
    }
    public void DestroySelf()
    {
        GuiManager.Instance.DestroyGui<PopupCheatGame>();
    }
}
