using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;
using Sfs2X.Entities.Data;

public class PickTeamController: MonoBehaviour
{
    SFSObject sailorCheatData;
    private void Start()
    {
        SquadContainer.Draging = false;

        sailorCheatData = new SFSObject();
        ISFSArray sailorcheats = new SFSArray();
        sailorCheatData.PutSFSArray("sailorcheats", sailorcheats);
    }
    public void OnJoinBattle()
    {
        NetworkController.Send(SFSAction.COMBAT_BOT, sailorCheatData);
    }

    public void OnBackToLobby()
    {
        SceneManager.LoadScene("SceneLobby");
    }
    public void CheatBot()
    {
        var sailors = CrewData.Instance.Sailors;
        var fgl = CrewData.Instance.FightingTeam;

        sailorCheatData = new SFSObject();
        ISFSArray sailorcheats = new SFSArray();

        for (short x = 0; x < 3; x++)
        {
            for (short y = 0; y < 3; y++)
            {
                string sailorId = fgl.SailorIdAt(x, y);
                SailorModel m = sailors.FirstOrDefault(s => s.id == sailorId);
                if (m != null)
                {
                    ISFSObject pos = new SFSObject();
                    pos.PutByte("x", (byte) x);
                    pos.PutByte("y", (byte) y);

                    ISFSObject sailorcheat = new SFSObject();
                    sailorcheat.PutUtfString("name", m.config_stats.root_name);
                    sailorcheat.PutSFSObject("pos", pos);

                    sailorcheats.AddSFSObject(sailorcheat);
                }
            }
        }
        sailorCheatData.PutSFSArray("sailorcheats", sailorcheats);
    }
}
