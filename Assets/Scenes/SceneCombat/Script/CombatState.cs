using DG.Tweening;
using Piratera.Config;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CombatStatus
{
    PREPARING,
    STARTED,
    ENDED
};

public class CombatState : MonoBehaviour
{
    public static CombatState Instance;

    public CombatStatus status;
    public List<CombatSailor> sailorsTeamA = new List<CombatSailor>();
    public List<CombatSailor> sailorsTeamB = new List<CombatSailor>();
    public Team lastTeamAction;

    public List<ClassBonusItem> classBonusA = new List<ClassBonusItem>();
    public List<ClassBonusItem> classBonusB = new List<ClassBonusItem>();
    private void Awake()
    {
        Instance = this;
    }
    public CombatState()
    {
        status = CombatStatus.PREPARING;
        lastTeamAction = Team.B;
    }
    public void CreateTeamFromServer()
    {
        var listSailor = TempCombatData.Instance.listSailor;
        var fgl0 = TempCombatData.Instance.fgl0;
        var fgl1 = TempCombatData.Instance.fgl1;

        for (short x = 0; x < 3; x++)
        {
            for (short y = 0; y < 3; y++)
            {
                {
                    string sailorID = fgl0.SailorIdAt(x, y);
                    if (sailorID != "")
                    {
                        SailorModel sailor = listSailor.Find(sailor => sailor.id == sailorID);
                        CreateCombatSailor(sailor, new CombatPosition(x, y), Team.A);
                    }
                }
                {
                    string sailorID = fgl1.SailorIdAt(x, y);
                    if (sailorID != "")
                    {
                        SailorModel sailor = listSailor.Find(sailor => sailor.id == sailorID);
                        CreateCombatSailor(sailor, new CombatPosition(x, y), Team.B);
                    }
                }
            }
        }
    }
    public void CreateDemoTeam()
    {
        CreateTeamA();
        CreateTeamB();
    }
    void CreateTeamA()
    {
        CreateCombatSailor("Nael", new CombatPosition(0, 0), Team.A, 3, 20, 180);
        CreateCombatSailor("Nael", new CombatPosition(0, 2), Team.A, 3, 20, 180);
        CreateCombatSailor("Nael", new CombatPosition(2, 0), Team.A, 3, 20, 180);
        CreateCombatSailor("Nael", new CombatPosition(2, 1), Team.A, 3, 20, 180);
        CreateCombatSailor("Nael", new CombatPosition(2, 2), Team.A, 3, 20, 180);
    }
    void CreateTeamB()
    {
        CreateCombatSailor("Macay", new CombatPosition(0, 0), Team.B, 3, 20, 180);
        CreateCombatSailor("Macay", new CombatPosition(1, 1), Team.B, 3, 20, 180);
        CreateCombatSailor("Macay", new CombatPosition(2, 0), Team.B, 3, 20, 180);
        CreateCombatSailor("Macay", new CombatPosition(2, 2), Team.B, 3, 20, 180);
        CreateCombatSailor("Macay", new CombatPosition(0, 2), Team.B, 3, 20, 180);
    }

    CombatSailor CreateCombatSailor(string sailorString, CombatPosition pos, Team team, int star = 1, int level = 1, int quality = 160)
    {
        string[] split = sailorString.Split(char.Parse("|"));
        string name = split[0];

        //Debug.Log("CreateCombatSailor>>> " + name);

        List<Item> listItem = new List<Item>();
        for (int i = 1; i < split.Length; i++)
        {
            string itemName = split[i].Split(char.Parse(":"))[0];
            string itemQuality = split[i].Split(char.Parse(":"))[1];
            listItem.Add(GameUtils.CreateItem(itemName, Int32.Parse(itemQuality)));
        }
        CombatSailor sailor = GameUtils.CreateCombatSailor(name);
        // sailor.Model.quality = UnityEngine.Random.Range(1, 100 + 1);
        // sailor.Model.level = UnityEngine.Random.Range(1, 10 + 1);
        sailor.Model.quality = quality;
        sailor.Model.level = level;
        sailor.Model.star = (byte)star;

        sailor.SetEquipItems(listItem);
        sailor.InitCombatData(pos, team);
        sailor.CreateStatusBar();
        sailor.InitDisplayStatus();
        if (team == Team.A) sailorsTeamA.Add(sailor);
        else sailorsTeamB.Add(sailor);

        return sailor;
    }

    public CombatSailor CreateCombatSailor(SailorModel s, CombatPosition pos, Team team)
    {
        string name = s.config_stats.root_name;
        List<Item> listItem = s.items;

        CombatSailor sailor = GameUtils.CreateCombatSailor(s);

        sailor.SetEquipItems(listItem);
        sailor.InitCombatData(pos, team);
        sailor.CreateStatusBar();
        sailor.InitDisplayStatus();
        if (team == Team.A) sailorsTeamA.Add(sailor);
        else sailorsTeamB.Add(sailor);

        return sailor;
    }

    public List<CombatSailor> GetAllSailors()
    {
        List<CombatSailor> result = new List<CombatSailor>();
        sailorsTeamA.ForEach(delegate (CombatSailor character)
        {
            result.Add(character);
        });
        sailorsTeamB.ForEach(delegate (CombatSailor character)
        {
            result.Add(character);
        });
        return result;
    }
    public List<CombatSailor> GetAllAliveCombatSailors()
    {
        List<CombatSailor> result = new List<CombatSailor>();
        sailorsTeamA.ForEach(character =>
        {
            if (!character.IsDeath()) result.Add(character);
        });
        sailorsTeamB.ForEach(character =>
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<CombatSailor> GetAllTeamAliveSailors(Team t)
    {
        List<CombatSailor> result = new List<CombatSailor>();
        List<CombatSailor> CTeam = t == Team.A ? sailorsTeamA : sailorsTeamB;
        CTeam.ForEach(delegate (CombatSailor character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<CombatSailor> GetAllTeamAliveExceptSelfSailors(Team t, CombatSailor m)
    {
        List<CombatSailor> result = new List<CombatSailor>();
        List<CombatSailor> CTeam = t == Team.A ? sailorsTeamA : sailorsTeamB;
        CTeam.ForEach(delegate (CombatSailor character)
        {
            if (!character.IsDeath() && character != m) result.Add(character);
        });
        return result;
    }
    public List<CombatSailor> GetAliveCharacterEnemy(Team t)
    {
        List<CombatSailor> result = new List<CombatSailor>();
        List<CombatSailor> CTeam = t == Team.A ? sailorsTeamB : sailorsTeamA;
        CTeam.ForEach(delegate (CombatSailor character)
        {
            if (!character.IsDeath()) result.Add(character);
        });
        return result;
    }
    public List<CombatSailor> GetQueueNextActionSailor()
    {
        List<CombatSailor> result = GetAllAliveCombatSailors();
        result.Sort((sailor1, sailor2) =>
        {
            int speedNeed_1 = sailor1.GetSpeedNeeded();
            int speedNeed_2 = sailor2.GetSpeedNeeded();
            if (speedNeed_1 < speedNeed_2) return -1;
            if (speedNeed_1 > speedNeed_2) return 1;
            short x_1 = sailor1.cs.position.x;
            short x_2 = sailor2.cs.position.x;
            if (x_1 < x_2) return -1;
            if (x_1 > x_2) return 1;
            short y_1 = sailor1.cs.position.y;
            short y_2 = sailor2.cs.position.y;
            if (y_1 < y_2) return -1;
            if (y_1 > y_2) return 1;
            if (sailor1.cs.team == Team.A) return -1;
            else return 1;
        });
        return result;
    }

    private List<ClassBonusItem> CalculateClassBonus(List<CombatSailor> t)
    {
        List<ClassBonusItem> result = new List<ClassBonusItem>();
        List<int> typeCount = new List<int>();
        int assassinCount = 0;
        for (int i = 0; i < Enum.GetNames(typeof(SailorClass)).Length; i++)
        {
            typeCount.Add(0);
        }

        foreach (SailorClass type in (SailorClass[])Enum.GetValues(typeof(SailorClass)))
        {
            List<string> countedSailor = new List<string>();
            t.ForEach(sailor =>
            {
                string sailorName = sailor.Model.config_stats.root_name;
                // moi loai sailor chi tinh 1 lan
                if (sailor.cs.HaveType(type) && !countedSailor.Contains(sailorName))
                {
                    typeCount[(int)type] += 1;
                    countedSailor.Add(sailorName);
                }
            });
        }

        t.ForEach(sailor =>
        {
            // Dem so assassin, neu >= 2 thi khong kich hoat
            if (sailor.cs.HaveType(SailorClass.ASSASSIN)) assassinCount++;
        });

        foreach (SailorClass type in Enum.GetValues(typeof(SailorClass)))
        {
            // nhieu hon 1 assassin thi skip tinh assassin
            if (type == SailorClass.ASSASSIN && assassinCount > 1) continue;

            SynergiesConfig config = GlobalConfigs.Synergies;
            if (!config.HaveBonus(type)) continue;
            List<int> milestones = config.GetMilestones(type);
            for (int level = milestones.Count - 1; level >= 0; level--)
            {
                int popNeed = milestones[level];
                if (typeCount[(int)type] >= popNeed)
                {
                    result.Add(new ClassBonusItem() { type = type, level = level, current = typeCount[(int)type] });
                    break;
                }
            }
        }
        return result;
    }
    public void CalculateClassBonus()
    {
        classBonusA = CalculateClassBonus(sailorsTeamA);
        classBonusB = CalculateClassBonus(sailorsTeamB);
        //passiveTypeA.ForEach(p => Debug.Log("passiveTypeA: " + p.type + " " + p.level));
    }

    public void UpdateGameWithClassBonus()
    {
        sailorsTeamA.ForEach(sailor => sailor.UpdateCombatData(classBonusA, classBonusB));
        sailorsTeamB.ForEach(sailor => sailor.UpdateCombatData(classBonusB, classBonusA));
    }
    public ClassBonusItem GetTeamClassBonus(Team team, SailorClass type)
    {
        List<ClassBonusItem> t = team == Team.A ? classBonusA : classBonusB;
        return t.FirstOrDefault(e => e.type == type);
    }
    public void SyncServerSailorStatus(List<MapStatusItem> mapStatus)
    {
        mapStatus.ForEach(item =>
        {
            List<SailorStatus> statuses = item.listStatus;
            CombatSailor s = GetSailor(item.sailor_id);
            s.SyncStatus(statuses);
        });
    }
    public void RunEndAction(CombatSailor actor)
    {
        actor.CountdownStatusRemain();
        var listSailor = GetAllSailors();
        listSailor.ForEach(sailor =>
        {
            sailor.CheckDeath();
        });
    }
    public float GetTeamHealthRatio(Team t)
    {
        List<CombatSailor> list = t == Team.A ? sailorsTeamA : sailorsTeamB;
        float health = 0;
        float total_health = 0;
        list.ForEach(sailor =>
        {
            health += sailor.cs.CurHealth;
            total_health += sailor.cs.MaxHealth;
        });
        return health / total_health;
    }

#if PIRATERA_DEV
    public string GetTeamHealthRatioString(Team t)
    {
        List<CombatSailor> list = t == Team.A ? sailorsTeamA : sailorsTeamB;
        float health = 0;
        float total_health = 0;
        list.ForEach(sailor =>
        {
            health += sailor.cs.CurHealth;
            total_health += sailor.cs.MaxHealth;
        });
        return $"{health}/{total_health}";
    }
#endif
    public CombatSailor GetSailor(Team t, string id)
    {
        var l = GetAllTeamAliveSailors(t);
        return l.Find(sailor =>
        {
            return sailor.Model.id == id;
        });
    }
    public CombatSailor GetSailor(Team t, CombatPosition p)
    {
        var l = GetAllTeamAliveSailors(t);
        var s = l.Find(sailor =>
        {
            //Debug.Log("sailor.cs.position.x " + sailor.cs.position.x);
            //Debug.Log("sailor.cs.position.y " + sailor.cs.position.y);
            //Debug.Log("p.x " + p.x);
            //Debug.Log("p.y " + p.y);
            //Debug.Log("sailor.cs.position.x == p.x " + (sailor.cs.position.x == p.x));
            //Debug.Log("sailor.cs.position.y == p.y " + (sailor.cs.position.y == p.y));
            return (sailor.cs.position.x == p.x && sailor.cs.position.y == p.y);
        });
        Debug.Log("S: " + s);
        return s;
    }
    public CombatSailor GetSailor(string id)
    {
        var l = GetAllSailors();
        return l.Find(sailor =>
        {
            return sailor.Model.id == id;
        });
    }
    public List<CombatSailor> GetSailors(List<string> list_id)
    {
        var result = new List<CombatSailor>();
        list_id.ForEach(id => result.Add(GetSailor(id)));
        return result;
    }
    public void HighlightListSailor(List<CombatSailor> sailors, float time)
    {
        var l = GetAllSailors();
        l.ForEach(sailor =>
        {
            if (!sailors.Contains(sailor))
            {
                Spine.Skeleton skeleton = sailor.modelObject.GetComponent<SkeletonMecanim>().skeleton;
                Sequence seq = DOTween.Sequence();
                seq.AppendCallback(() => sailor.DoModelColor(new Color(0.4f, 0.4f, 0.4f)));
                seq.AppendInterval(time - 0.2f);
                seq.AppendCallback(() => sailor.DoModelColor(Color.white));
            }
        });

        SpriteRenderer bg = GameObject.Find("battlefield").GetComponent<SpriteRenderer>();

        Sequence seq = DOTween.Sequence();
        seq.Append(bg.DOColor(new Color(0.4f, 0.4f, 0.4f), 0.4f));
        seq.AppendInterval(time - 0.4f);
        seq.Append(bg.DOColor(Color.white, 0.2f));
    }
    public void HighlightSailor2Step(CombatSailor main, List<CombatSailor> sailors, float time1, float time2)
    {
        var l = GetAllSailors();
        l.ForEach(sailor =>
        {
            if (sailor == main) { }
            else if (!sailors.Contains(sailor))
            {
                Sequence seq = DOTween.Sequence();
                seq.AppendCallback(() => sailor.DoModelColor(new Color(0.4f, 0.4f, 0.4f)));
                seq.AppendInterval(time1 + time2 - 0.2f);
                seq.AppendCallback(() => sailor.DoModelColor(Color.white));
            }
            else
            {
                Spine.Skeleton skeleton = sailor.modelObject.GetComponent<SkeletonMecanim>().skeleton;
                Sequence seq = DOTween.Sequence();
                seq.AppendCallback(() => sailor.DoModelColor(new Color(0.4f, 0.4f, 0.4f)));
                seq.AppendInterval(time1 - 0.2f);
                seq.AppendCallback(() => sailor.DoModelColor(Color.white));
            }
        });

        SpriteRenderer bg = GameObject.Find("battlefield").GetComponent<SpriteRenderer>();

        Sequence seq = DOTween.Sequence();
        seq.Append(bg.DOColor(new Color(0.4f, 0.4f, 0.4f), 0.2f));
        seq.AppendInterval(time1 - 0.3f);
        seq.Append(bg.DOColor(new Color(0.4f, 0.4f, 0.4f), 0.2f));
        seq.AppendInterval(time2 - 0.3f);
        seq.Append(bg.DOColor(Color.white, 0.2f));
    }
    public void CreateTrainGame(int train_level)
    {
        var listSailor = CrewData.Instance.Sailors;
        var fgl0 = CrewData.Instance.FightingTeam;

        float totalStar = 0;
        float totalLevel = 0;
        int sailorNumber = 0;

        for (short x = 0; x < 3; x++)
        {
            for (short y = 0; y < 3; y++)
            {
                {
                    string sailorID = fgl0.SailorIdAt(x, y);
                    if (sailorID != "")
                    {
                        SailorModel sailor = listSailor.Find(sailor => sailor.id == sailorID);
                        CreateCombatSailor(sailor, new CombatPosition(x, y), Team.A);
                        sailorNumber++;
                        totalLevel += sailor.level;
                        totalStar += sailor.star;
                    }
                }
            }
        }
        int level, star, quality;
        switch (train_level)
        {
            case 0:
                level = (int)Math.Floor(totalLevel / sailorNumber);
                star = (int)Math.Floor(totalStar / sailorNumber);
                quality = 100;
                break;
            case 1:
                level = (int) Math.Round(totalLevel / sailorNumber) + 2;
                star = (int) Math.Round(totalStar / sailorNumber);
                quality = 140;
                break;
            default:
                level = (int) Math.Ceiling(totalLevel / sailorNumber) + 1;
                star = (int) Math.Ceiling(totalStar / sailorNumber) + 1;
                quality = 200;
                break;
        }
        if (level > 30) level = 30;
        CreateTrainTeamB(sailorNumber, level, star, quality);
    }
    private void CreateTrainTeamB(int number, int level, int star, int quality)
    {
        for (int i = 0; i < number; i++)
        {
            var pos = new CombatPosition(2, 0);
            var sailor = "Wuone";
            var random = new System.Random();
            switch (i)
            {
                case 0:
                    {
                        pos.x = 1;
                        pos.y = 1;
                        var list = new List<string> { "Anglersei" };
                        sailor = list[random.Next(list.Count)];
                        break;
                    }
                case 1:
                    {
                        pos.x = 2;
                        pos.y = 0;
                        var list = new List<string> { "Anglersei" };
                        sailor = list[random.Next(list.Count)];
                        break;
                    }
                case 2:
                    {
                        pos.x = 0;
                        pos.y = 2;
                        var list = new List<string> { "Anglersei" };
                        sailor = list[random.Next(list.Count)];
                        break;
                    }
                case 3:
                    {
                        pos.x = 0;
                        pos.y = 0;
                        var list = new List<string> { "Anglersei" };
                        sailor = list[random.Next(list.Count)];
                        break;
                    }
                case 4:
                    {
                        pos.x = 2;
                        pos.y = 2;
                        var list = new List<string> { "Anglersei" };
                        sailor = list[random.Next(list.Count)];
                        break;
                    }
            }
            CreateCombatSailor(sailor, pos, Team.B, star, level, quality);
        }
    }
    public void ShowTrainComplete(int trainLevel)
    {
        var exp = GlobalConfigs.Training.exp_receive[trainLevel];
        sailorsTeamA.ForEach(character => {
            FlyTextMgr.Instance.ShowTextAddExp(character, exp);
        });
    }
    public void CreateChallengeGame()
    {
        var listSailor = TempCombatData.Instance.listSailor;
        var fgl1 = TempCombatData.Instance.fgl1;

        var listYourSailor = CrewData.Instance.Sailors;
        var fgl0 = CrewData.Instance.FightingTeam;
        for (short x = 0; x < 3; x++)
        {
            for (short y = 0; y < 3; y++)
            {
                {
                    string sailorID = fgl0.SailorIdAt(x, y);
                    if (sailorID != "")
                    {
                        SailorModel sailor = listYourSailor.Find(sailor => sailor.id == sailorID);
                        CreateCombatSailor(sailor, new CombatPosition(x, y), Team.A);
                    }
                }
                {
                    string sailorID = fgl1.SailorIdAt(x, y);
                    if (sailorID != "")
                    {
                        SailorModel sailor = listSailor.Find(sailor => sailor.id == sailorID);
                        CreateCombatSailor(sailor, new CombatPosition(x, y), Team.B);
                    }
                }
            }
        }
    }
};
