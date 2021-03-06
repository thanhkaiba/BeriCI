using Sfs2X.Entities.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TempCombatData : Singleton<TempCombatData>
{
    public bool waitForAServerGame = false;
    public bool challengeGame = false;
    public string lastScene = "";
    public string lastChallengeUID = "";
    public int trainingGameLevel = -1;

    public ModeID modeID;
    public byte yourTeamIndex;
    public string userName0;
    public string userName1;
    public int avt0;
    public int avt1;
    public int combatId;
    public List<SailorModel> listSailor;
    public FightingLine fgl0;
    public FightingLine fgl1;
    public List<CombatAction> ca;
    public HomefieldAdvantage defense_advantage;

    public bool isReplayMatch = false;

    public void LoadCombatDataFromSfs(ISFSObject packet)
    {
        Debug.Log("Read Combat Data");
        modeID = (ModeID)packet.GetByte("mode_id");
        listSailor = new List<SailorModel>();

        yourTeamIndex = packet.GetByte("your_team_idx");
        userName0 = packet.GetUtfString("username_0");
        userName1 = packet.GetUtfString("username_1");
        avt0 = packet.GetInt("avt_0");
        avt1 = packet.GetInt("avt_1");
        combatId = packet.GetInt("combat_id");
        /* ISFSArray sFSSailors = packet.GetSFSArray("sailors");
         foreach (ISFSObject obj in sFSSailors)
         {
             SailorModel model = new SailorModel(obj.GetUtfString("id"), obj.GetUtfString("name"))
             { quality = obj.GetInt("quality"), level = obj.GetInt("level"), exp = obj.GetInt("exp") };
             listSailor.Add(model);
         }*/

        ISFSArray sFSSailors = packet.GetSFSArray("sailors_1");
        foreach (ISFSObject obj in sFSSailors)
        {
            SailorModel model = new SailorModel(obj);
            listSailor.Add(model);
        }

        sFSSailors = packet.GetSFSArray("sailors_0");
        foreach (ISFSObject obj in sFSSailors)
        {
            SailorModel model = new SailorModel(obj);
            listSailor.Add(model);
        }

        fgl0 = new FightingLine();
        fgl1 = new FightingLine();

        fgl0.NewFromSFSObject(packet.GetSFSArray("fgl_0"));
        fgl1.NewFromSFSObject(packet.GetSFSArray("fgl_1"));

        waitForAServerGame = true;

        ca = new List<CombatAction>();
        ISFSArray _ca = packet.GetSFSArray("ca");
        foreach (ISFSObject obj in _ca) ca.Add(new CombatAction(obj, modeID));

        if (modeID == ModeID.Arena) defense_advantage = (HomefieldAdvantage)packet.GetInt("defense_advantage");
        Debug.Log("defense_advantage " + defense_advantage);
        isReplayMatch = false;
    }
}

public enum CombatAcionType
{
    BaseAttack,
    UseSkill,
    GameResult,
    Immobile,
    Lose
}
public enum RankBonus
{
    None, ABSOLUTE = 3, OUTSTANDING = 2, VERY_GOLD = 1, GOOD = 0
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
    public string actor;
    public string target;
    public List<string> targets;
    public List<float> _params;
    public bool haveCrit;
    public bool haveDodge;

    public List<MapStatusItem> mapStatus;

    public GameEndData gameEndData;
    public CombatAction(ISFSObject packet, ModeID mode)
    {
        type = (CombatAcionType)packet.GetByte("actionType");
        ISFSObject detail = packet.GetSFSObject("detail");
        switch (type)
        {
            case CombatAcionType.BaseAttack:
                actorTeam = detail.GetByte("actor_team");
                actor = detail.GetUtfString("actor");
                target = detail.GetUtfString("target");
                haveCrit = detail.GetBool("have_crit");
                haveDodge = detail.GetBool("have_dodge");
                _params = detail.GetFloatArray("params").OfType<float>().ToList();
                break;
            case CombatAcionType.UseSkill:
                actorTeam = detail.GetByte("actor_team");
                actor = detail.GetUtfString("actor");
                targets = detail.GetUtfStringArray("targets").OfType<string>().ToList();
                _params = detail.GetFloatArray("params").OfType<float>().ToList();
                break;
            case CombatAcionType.GameResult:
                if (mode == ModeID.PvE)
                {
                    gameEndData = new GameEndData(packet);
                }
                else if (mode == ModeID.Arena)
                {
                    gameEndData = new GameEndPvPData(packet);
                }

                break;
            case CombatAcionType.Immobile:
                actorTeam = detail.GetByte("actor_team");
                actor = detail.GetUtfString("actor");
                break;
        }
        mapStatus = new List<MapStatusItem>();
        if (type == CombatAcionType.BaseAttack || type == CombatAcionType.UseSkill)
        {
            ISFSArray _mapStatus = packet.GetSFSArray("mapStatus");
            if (mapStatus != null) foreach (ISFSObject obj in _mapStatus) mapStatus.Add(new MapStatusItem(obj));
        }
    }
}
public class GameEndData
{
    public RankBonus type;
    public byte win_rank;
    public int mode_reward;
    public int win_rank_bonus;
    public int hard_bonus;
    public int team_bonus;
    public byte team_win;
    public Rewards bonus;
    public GameEndData(ISFSObject packet)
    {
        ISFSObject detail = packet.GetSFSObject("detail");
        type = (RankBonus)detail.GetByte("win_type");
        win_rank = detail.GetByte("win_rank");
        mode_reward = detail.GetInt("mode_reward");
        win_rank_bonus = detail.GetInt("win_rank_bonus");
        hard_bonus = detail.GetInt("hard_bonus");
        team_bonus = detail.GetInt("team_bonus");
        team_win = detail.GetByte("team_win");
        bonus = new Rewards(detail.GetSFSObject("bonus"));
    }
}
public class Rewards
{
    public string type;
    public string meta;
    public long amount;
    public Rewards(ISFSObject packet)
    {
        Read(packet);
    }
    public void Read(ISFSObject packet)
    {
        type = packet.GetUtfString("type");
        meta = packet.GetUtfString("meta");
        amount = packet.GetLong("amount");
    }
}
public class GameEndPvPData : GameEndData
{
    public int elo_delta;
    public GameEndPvPData(ISFSObject packet) : base(packet)
    {
        ISFSObject detail = packet.GetSFSObject("detail");
        elo_delta = detail.GetInt("elo_delta");
        team_win = detail.GetByte("team_win");
    }
}

public class MapStatusItem
{
    public MapStatusItem(ISFSObject packet)
    {
        sailor_id = packet.GetUtfString("sid");
        listStatus = new List<SailorStatus>();
        ISFSArray _listStatus = packet.GetSFSArray("status");
        foreach (ISFSObject obj in _listStatus)
        {
            var stack = obj.GetInt("stack");
            var type = obj.GetByte("type");
            listStatus.Add(new SailorStatus((SailorStatusType)type, stack));
        }
    }
    public string sailor_id;
    public List<SailorStatus> listStatus;
}