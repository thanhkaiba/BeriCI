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

        public static void InitSyncConfig()
        {
            LineUp = JsonConvert.DeserializeObject<LineUpSlot>(GameConfigSync.GetContent("configs/LineUpSlot.json"));
            PvE = JsonConvert.DeserializeObject<PvEConfig>(GameConfigSync.GetContent("configs/PvE.json"));
            SailorGeneral = JsonConvert.DeserializeObject<SailorGeneralConfig>(GameConfigSync.GetContent("configs/SailorGeneralConfig.json"));
            StaminaConfig = JsonConvert.DeserializeObject<UserStaminaConfig>(GameConfigSync.GetContent("configs/Stamina.json"));
            Combat = JsonConvert.DeserializeObject<CombatConfig>(GameConfigSync.GetContent("configs/Combat.json"));
            Synergies = JsonConvert.DeserializeObject<SynergiesConfig>(GameConfigSync.GetContent("configs/ContainerClassBonus.json"));
            SailorStatus = JsonConvert.DeserializeObject<SailorStatusConfig>(GameConfigSync.GetContent("configs/StatusConfig.json"));

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
            return null;
        }
    
    }
}