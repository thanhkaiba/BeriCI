using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Piratera.Config
{
    public class GlobalConfigs : Object
    {
        public static SailorGeneralConfig SailorGeneral;
        public static PvEConfig PvE;
        public static LineUpSlot LineUp;
        public static UserStaminaConfig StaminaConfig;
        public static PirateWheelConfig PirateWheelConfig;
        public static CombatConfig Combat;
        private static Dictionary<string, SailorConfig> SailorDic = new();
        public static SynergiesConfig Synergies;
        public static SailorStatusConfig SailorStatus;
        public static UpgradeShipConf UpgradeShipConfig;
        public static TrainingConfig Training;
        public static PvPConfig PvP;
        public static HomefieldAdvantageConfig HomefieldAdvantage;
        public static ArenaRewardsConfig ArenaRewards;
        public static bool HaveLoaded;

        public static void InitSyncConfig()
        {
            LineUp = JsonConvert.DeserializeObject<LineUpSlot>(GameConfigSync.GetContent("configs/LineUpSlot.json"));
            PvE = JsonConvert.DeserializeObject<PvEConfig>(GameConfigSync.GetContent("configs/PvE.json"));
            SailorGeneral = JsonConvert.DeserializeObject<SailorGeneralConfig>(GameConfigSync.GetContent("configs/SailorGeneralConfig.json"));
            StaminaConfig = JsonConvert.DeserializeObject<UserStaminaConfig>(GameConfigSync.GetContent("configs/Stamina.json"));
            Combat = JsonConvert.DeserializeObject<CombatConfig>(GameConfigSync.GetContent("configs/Combat.json"));
            Synergies = JsonConvert.DeserializeObject<SynergiesConfig>(GameConfigSync.GetContent("configs/ContainerClassBonus.json"));
            SailorStatus = JsonConvert.DeserializeObject<SailorStatusConfig>(GameConfigSync.GetContent("configs/StatusConfig.json"));
            PirateWheelConfig = JsonConvert.DeserializeObject<PirateWheelConfig>(GameConfigSync.GetContent("configs/PirateWheel.json"));
            UpgradeShipConfig = JsonConvert.DeserializeObject<UpgradeShipConf>(GameConfigSync.GetContent("configs/UpgradeShip.json"));
            Training = JsonConvert.DeserializeObject<TrainingConfig>(GameConfigSync.GetContent("configs/Training.json"));
            PvP = JsonConvert.DeserializeObject<PvPConfig>(GameConfigSync.GetContent("configs/PvP.json"));
            HomefieldAdvantage = JsonConvert.DeserializeObject<HomefieldAdvantageConfig>(GameConfigSync.GetContent("configs/HomefieldAdvantage.json"));
            ArenaRewards = JsonConvert.DeserializeObject<ArenaRewardsConfig>(GameConfigSync.GetContent("configs/ArenaRewards.json"));
            string[] files = GameConfigSync.GetSailorFolder();
            foreach (string file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);

                SailorConfig config = JsonConvert.DeserializeObject<SailorConfig>(GameConfigSync.GetContent(file));
                if (SailorDic.ContainsKey(name))
                {
                    SailorDic[name] = config;
                }
                else
                {
                    SailorDic.Add(name, config);
                }
            }

        }
        public static SailorConfig GetSailorConfig(string sailorRootName)
        {
            if (SailorDic.ContainsKey(sailorRootName)) return SailorDic[sailorRootName];
            HaveLoaded = true;
            return null;
        }

        public static void InitDevConfig()
        {
            if (HaveLoaded) return;
            LineUp = JsonConvert.DeserializeObject<LineUpSlot>(File.ReadAllText("DEV_CONFIG/LineUpSlot.json"));
            PvE = JsonConvert.DeserializeObject<PvEConfig>(File.ReadAllText("DEV_CONFIG/PvE.json"));
            SailorGeneral = JsonConvert.DeserializeObject<SailorGeneralConfig>(File.ReadAllText("DEV_CONFIG/SailorGeneralConfig.json"));
            StaminaConfig = JsonConvert.DeserializeObject<UserStaminaConfig>(File.ReadAllText("DEV_CONFIG/Stamina.json"));
            PirateWheelConfig = JsonConvert.DeserializeObject<PirateWheelConfig>(File.ReadAllText("DEV_CONFIG/PirateWheel.json"));
            Combat = JsonConvert.DeserializeObject<CombatConfig>(File.ReadAllText("DEV_CONFIG/Combat.json"));
            Synergies = JsonConvert.DeserializeObject<SynergiesConfig>(File.ReadAllText("DEV_CONFIG/ContainerClassBonus.json"));
            SailorStatus = JsonConvert.DeserializeObject<SailorStatusConfig>(File.ReadAllText("DEV_CONFIG/StatusConfig.json"));
            UpgradeShipConfig = JsonConvert.DeserializeObject<UpgradeShipConf>(GameConfigSync.GetContent("DEV_CONFIG/UpgradeShip.json"));

            string[] files = Directory.GetFiles("DEV_CONFIG/Sailors");
            foreach (string file in files)
            {
                if (Path.GetExtension(file).ToLower() == ".json")
                {
                    string name = Path.GetFileNameWithoutExtension(file);

                    SailorConfig config = JsonConvert.DeserializeObject<SailorConfig>(File.ReadAllText(file));
                    if (SailorDic.ContainsKey(name))
                    {
                        SailorDic[name] = config;
                    }
                    else
                    {
                        SailorDic.Add(name, config);
                    }
                }
            }
            HaveLoaded = true;
        }

    }
}