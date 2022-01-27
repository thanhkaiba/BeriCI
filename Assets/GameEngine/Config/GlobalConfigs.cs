using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Piratera.Config {
    public class GlobalConfigs : Object
    {
        public static SailorGeneralConfig SailorGeneral;
        public static PvEConfig PvE;
        public static LineUpSlot LineUp;
        public static UserStaminaConfig StaminaConfig;
        public static CombatConfig Combat;
        private static Dictionary<string, SailorConfig> SailorDic = new Dictionary<string, SailorConfig>();
        public static SynergiesConfig Synergies;
        public static SailorStatusConfig SailorStatus;
        private static bool HaveLoaded = false;

        public static void InitSyncConfig()
        {
            LineUp = JsonConvert.DeserializeObject<LineUpSlot>(GameConfigSync.GetContent("LineUpSlot.json"));
            PvE = JsonConvert.DeserializeObject<PvEConfig>(GameConfigSync.GetContent("PvE.json"));
            SailorGeneral = JsonConvert.DeserializeObject<SailorGeneralConfig>(GameConfigSync.GetContent("SailorGeneralConfig.json"));
            StaminaConfig = JsonConvert.DeserializeObject<UserStaminaConfig>(GameConfigSync.GetContent("Stamina.json"));
            Combat = JsonConvert.DeserializeObject<CombatConfig>(GameConfigSync.GetContent("Combat.json"));
            Synergies = JsonConvert.DeserializeObject<SynergiesConfig>(GameConfigSync.GetContent("ContainerClassBonus.json"));
            SailorStatus = JsonConvert.DeserializeObject<SailorStatusConfig>(GameConfigSync.GetContent("StatusConfig.json"));

            string[] files = Directory.GetFiles(GameConfigSync.GetSailorFolder());
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
        public static SailorConfig GetSailorConfig(string sailorRootName)
        {
            if (SailorDic.ContainsKey(sailorRootName)) return SailorDic[sailorRootName];
            return null;
        }
        public static void InitDevConfig()
        {
            if (HaveLoaded) return;
            LineUp = JsonConvert.DeserializeObject<LineUpSlot>(File.ReadAllText("DEV_CONFIG/LineUpSlot.json"));
            PvE = JsonConvert.DeserializeObject<PvEConfig>(File.ReadAllText("DEV_CONFIG/PvE.json"));
            SailorGeneral = JsonConvert.DeserializeObject<SailorGeneralConfig>(File.ReadAllText("DEV_CONFIG/SailorGeneralConfig.json"));
            StaminaConfig = JsonConvert.DeserializeObject<UserStaminaConfig>(File.ReadAllText("DEV_CONFIG/Stamina.json"));
            Combat = JsonConvert.DeserializeObject<CombatConfig>(File.ReadAllText("DEV_CONFIG/Combat.json"));
            Synergies = JsonConvert.DeserializeObject<SynergiesConfig>(File.ReadAllText("DEV_CONFIG/ContainerClassBonus.json"));
            SailorStatus = JsonConvert.DeserializeObject<SailorStatusConfig>(File.ReadAllText("DEV_CONFIG/ContainerClassBonus.json"));

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