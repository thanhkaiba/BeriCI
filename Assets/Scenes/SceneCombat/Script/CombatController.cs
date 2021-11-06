using Sfs2X.Entities.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatController : BaseController
{
    public static CombatController Instance;
    protected override void Awake()
    {
        base.Awake();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SendCombatBotTest()
    {
        SmartFoxConnection.Send(SFSAction.COMBAT_BOT);
        Debug.Log("send COMBAT_BOT");
    }
    public CombatController()
    {
        forceLoginScene = false;
    }
    protected override void OnReceiveServerAction(SFSAction action, SFSErrorCode errorCode, ISFSObject packet)
    {

        Debug.Log("Recieve Combat controller" + action.ToString());
        switch (action)
        {
            case SFSAction.COMBAT_BOT:
                var listSailor = new List<SailorModel>();

                byte yourTeamIndex = packet.GetByte("your_team_idx");
                string userName0 = packet.GetUtfString("username_0");
                string userName1 = packet.GetUtfString("username_1");
                string avt0 = packet.GetUtfString("avt_0");
                string avt1 = packet.GetUtfString("avt_1");
                string combatId = packet.GetUtfString("combat_id");
                ISFSArray sFSSailors = packet.GetSFSArray("sailors");
                foreach (ISFSObject obj in sFSSailors)
                {
                    SailorModel model = new SailorModel(obj.GetUtfString("id"), obj.GetUtfString("name"))
                    { quality = obj.GetInt("quality"), level = obj.GetInt("level"), exp = obj.GetInt("exp") };
                    listSailor.Add(model);
                }

                var fgl_0 = new FightingLine();
                var fgl_1 = new FightingLine();

                fgl_0.NewFromSFSObject(packet.GetSFSArray("fgl_0"));
                fgl_1.NewFromSFSObject(packet.GetSFSArray("fgl_1"));

                SceneManager.LoadScene("SceneCombat2D");
                break;
        }
    }
}
