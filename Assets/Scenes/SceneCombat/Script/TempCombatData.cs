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
	public List<CombatAction> ca;

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

		ca = new List<CombatAction>();
		ISFSArray _ca = packet.GetSFSArray("ca");
		foreach (ISFSObject obj in _ca)
		{
			ca.Add(new CombatAction(obj));
		}
	}
}

public enum CombatAcionType
{
	BaseAttack,
	UseSkill,
	GameResult,
	GameState,
}

public class FGL
{
	public CombatPosition pos;
	public string id;
	public FGL(ISFSObject packet)
	{
		ISFSObject combatPos = packet.GetSFSObject("pos");
		pos = new CombatPosition(combatPos.GetByte("x"), combatPos.GetByte("y"));
		id = packet.GetUtfString("sid");
	}
}

public class CombatAction
{
	public CombatAcionType type;
	public byte actorTeam;
	public FGL actor;
	public FGL target;
	public List<FGL> targets;
	public bool haveCrit;
	public bool haveBlock;
	public byte teamWin;
	public CombatAction(ISFSObject packet)
	{
		type = (CombatAcionType) packet.GetByte("actionType");
		ISFSObject detail = packet.GetSFSObject("detail");
		switch (type)
		{
			case CombatAcionType.BaseAttack:
				actorTeam = detail.GetByte("actor_team");
				actor = new FGL(detail.GetSFSObject("actor"));
				target = new FGL(detail.GetSFSObject("target"));
				haveCrit = detail.GetBool("have_crit");
				haveBlock = detail.GetBool("have_block");
				break;
			case CombatAcionType.UseSkill:
				actorTeam = detail.GetByte("actor_team");
				actor = new FGL(detail.GetSFSObject("actor"));
				targets = new List<FGL>();
				foreach (ISFSObject obj in detail.GetSFSArray("targets"))
				{
					targets.Add(new FGL(obj));
				}
				haveCrit = detail.GetBool("have_crit");
				haveBlock = detail.GetBool("have_block");
				break;
			case CombatAcionType.GameResult:
				teamWin = detail.GetByte("team_win");
				break;
			case CombatAcionType.GameState:
				break;
		}
	}
}