using Sfs2X.Entities.Data;
using System.Collections.Generic;
using UnityEngine;

public class TeamCombatPrepareData : Singleton<TeamCombatPrepareData>
{
	private ModeID modeID;
	private byte yourTeamIndex;
	private string userName0;
	private string userName1;
	private string avt0;
	private string avt1;
	public int combatId;
	public List<SailorModel> sailors_0 = new List<SailorModel>();
	public List<SailorModel> sailors_1 = new List<SailorModel>();
	private FightingLine fgl0 = new FightingLine();
	private FightingLine fgl1 = new FightingLine();
	public byte countdown;


	protected override void OnAwake()
	{
		GameEvent.PrepareSquadChanged.AddListener(OnUpdateSquadA);
	}
	private void OnUpdateSquadA()
	{
		NetworkController.Send(SFSAction.TEAM_COMMIT, fgl0.ToSFSObject());
	}


	public string YourName { get
		{
			if (yourTeamIndex == 0)
            {
				return userName0;
            }

			return userName1;
		}
	}

	public string OpponentName
	{
		get
		{
			if (yourTeamIndex == 0)
			{
				return userName1;
			}

			return userName0;
		}
	}

	public string YourAvatar
	{
		get
		{
			if (yourTeamIndex == 0)
			{
				return avt0;
			}

			return avt1;
		}
	}

	public string OpponentAvatar
	{
		get
		{
			if (yourTeamIndex == 0)
			{
				return avt1;
			}

			return avt0;
		}
	}

	public FightingLine YourFightingLine
	{
		get
		{
			if (yourTeamIndex == 0)
			{
				return fgl0;
			}

			return fgl1;
		}
	}

	public FightingLine OpponentFightingLine
	{
		get
		{
			if (yourTeamIndex == 0)
			{
				return fgl1;
			}

			return fgl0;
		}
	}


	private List<SailorModel> YourSailors
	{
		get
		{
			if (yourTeamIndex == 0)
			{
				return sailors_0;
			}

			return sailors_1;
		}
	}

	private List<SailorModel> OpponentSailors
	{
		get
		{
			if (yourTeamIndex == 0)
			{
				return sailors_1;
			}

			return sailors_0;
		}
	}

	private void Clean()
    {
		yourTeamIndex = 0;
		userName0 = "";
		userName1 = "";
		avt0 = "";
		avt1 = "";
		fgl0.Clean();
		fgl1.Clean();
		countdown = 0;
		sailors_0.Clear();
		sailors_1.Clear();
		countdown = 0;
    }

	public void NewFromSFSObject(ISFSObject packet)
	{
		Clean();
		modeID = (ModeID)packet.GetByte("mode_id");
		yourTeamIndex = packet.GetByte("your_team_idx");
		userName0 = packet.GetUtfString("username_0");
		userName1 = packet.GetUtfString("username_1");
		avt0 = packet.GetUtfString("avt_0");
		avt1 = packet.GetUtfString("avt_1");
		combatId = packet.GetInt("combat_id");
		ISFSArray sFSSailors = packet.GetSFSArray("sailors_0");
		foreach (ISFSObject obj in sFSSailors)
		{
			SailorModel model = new SailorModel(obj.GetUtfString("id"), obj.GetUtfString("name"))
			{ quality = obj.GetInt("quality"), level = obj.GetInt("level"), exp = obj.GetInt("exp") };
			sailors_0.Add(model);
		}
		sFSSailors = packet.GetSFSArray("sailors_1");
		foreach (ISFSObject obj in sFSSailors)
		{
			SailorModel model = new SailorModel(obj.GetUtfString("id"), obj.GetUtfString("name"))
			{ quality = obj.GetInt("quality"), level = obj.GetInt("level"), exp = obj.GetInt("exp") };
			sailors_1.Add(model);
		}

		fgl0.NewFromSFSObject(packet.GetSFSArray("fgl_0"));
		fgl1.NewFromSFSObject(packet.GetSFSArray("fgl_1"));

		countdown = packet.GetByte("countdown");
	}

	public SailorModel GetYourSailorModel(string id)
    {
		return YourSailors.Find(sailor => sailor.id == id);
    }

	public SailorModel GetOpponentSailorModel(string id)
	{
		return OpponentSailors.Find(sailor => sailor.id == id);
	}

	public void Swap(string sailorA, string sailorB)
	{
		if (YourFightingLine.Swap(sailorA, sailorB))
		{
			GameEvent.PrepareSquadChanged.Invoke();
		}

	}

	public void Occupie(string sailorId, short slot)
	{
		if (GetYourSailorModel(sailorId) == null)
		{
			return;
		}

		if (YourFightingLine.Occupie(sailorId, slot))
		{
			GameEvent.PrepareSquadChanged.Invoke();
		}

	}

	public void Replace(string subSailor, short slot)
	{
		if (GetYourSailorModel(subSailor) == null)
		{
			return;
		}

		if (YourFightingLine.Replace(subSailor, slot))
		{
			GameEvent.PrepareSquadChanged.Invoke();
		}
	}

	public void UnEquip(string sailorId)
	{
		if (GetYourSailorModel(sailorId) == null)
		{
			return;
		}

		if (YourFightingLine.UnEquip(sailorId))
		{
			GameEvent.PrepareSquadChanged.Invoke();
		}
	}
	public List<SailorModel> GetSquadModelList(bool yourTeam)
	{
		List<SailorModel> result = new List<SailorModel>();
		List<string> values = yourTeam?  fgl0.GetValues() : fgl1.GetValues();
		foreach (string val in values)
		{
			if (val != "")
			{
				result.Add(yourTeam ? GetYourSailorModel(val) : GetOpponentSailorModel(val));
			}
		}
		return result;
	}

	public List<SailorModel> GetSubstituteSailors()
	{
		List<SailorModel> result = new List<SailorModel>();
		foreach (SailorModel model in YourSailors)
		{
			if (!YourFightingLine.IsInSquad(model.id))
			{
				result.Add(model);
			}
		}
		return result;
	}
}