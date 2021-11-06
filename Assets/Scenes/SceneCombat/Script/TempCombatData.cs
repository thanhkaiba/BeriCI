using Sfs2X.Entities.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempCombatData : Singleton<TempCombatData>
{
	public bool waitForAServerGame = false;
    public byte yourTeamIndex;
    public string userName0;
    public string userName1;
    public string avt0;
    public string avt1;
    public int combatId;
	public List<SailorModel> listSailor;
	public FightingLine fgl0;
    public FightingLine fgl1;

    public void LoadCombatDataFromSfs(ISFSObject packet)
    {
		Debug.Log("Read Combat Data");
		listSailor = new List<SailorModel>();

		yourTeamIndex = packet.GetByte("your_team_idx");
		userName0 = packet.GetUtfString("username_0");
		userName1 = packet.GetUtfString("username_1");
		avt0 = packet.GetUtfString("avt_0");
		avt1 = packet.GetUtfString("avt_1");
		combatId = packet.GetInt("combat_id");
		ISFSArray sFSSailors = packet.GetSFSArray("sailors");
		foreach (ISFSObject obj in sFSSailors)
		{
			SailorModel model = new SailorModel(obj.GetUtfString("id"), obj.GetUtfString("name"))
			{ quality = obj.GetInt("quality"), level = obj.GetInt("level"), exp = obj.GetInt("exp") };
			listSailor.Add(model);
		}

		fgl0 = new FightingLine();
		fgl1 = new FightingLine();

		fgl0.NewFromSFSObject(packet.GetSFSArray("fgl_0"));
		fgl1.NewFromSFSObject(packet.GetSFSArray("fgl_1"));

		waitForAServerGame = true;
	}
}
