using Sfs2X.Entities.Data;
using System.Collections.Generic;

public class TeamCombatPrepareData : Singleton<TeamCombatPrepareData>
{
	private byte yourTeamIndex;
	private string userName0;
	private string userName1;
	private string avt0;
	private string avt1;
	public int combatId;
	public List<SailorModel> listSailor;
	private FightingLine fgl0 = new FightingLine();
	private FightingLine fgl1 = new FightingLine();
	public byte countdown;


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
    }

	public void NewFromSFSObject(ISFSObject packet)
	{
		Clean();
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

		fgl0.NewFromSFSObject(packet.GetSFSArray("fgl_0"));
		fgl1.NewFromSFSObject(packet.GetSFSArray("fgl_1"));

		countdown = packet.GetByte("countdown");
	}

}