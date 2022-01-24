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

        public static void InitSyncConfig()
        {
            LineUp = JsonConvert.DeserializeObject<LineUpSlot>(GameConfigSync.GetContent("LineUpSlot.json"));
            PvE = JsonConvert.DeserializeObject<PvEConfig>(GameConfigSync.GetContent("PvE.json"));
            SailorGeneral = JsonConvert.DeserializeObject<SailorGeneralConfig>(GameConfigSync.GetContent("SailorGeneralConfig.json"));
            StaminaConfig = JsonConvert.DeserializeObject<UserStaminaConfig>(GameConfigSync.GetContent("Stamina.json"));
            Combat = JsonConvert.DeserializeObject<CombatConfig>(GameConfigSync.GetContent("Combat.json"));
            Synergies = JsonConvert.DeserializeObject<SynergiesConfig>(GameConfigSync.GetContent("ContainerClassBonus.json"));

            string[] files = System.IO.Directory.GetFiles(GameConfigSync.GetSailorFolder());
            foreach (string file in files)
            {
                if (file.Contains(".json"))
                {
                    string nameAsset = Path.GetFileName(file);
                    string name = nameAsset.Split('.')[0];
                    SailorConfig config = JsonConvert.DeserializeObject<SailorConfig>(File.ReadAllText(file));
                    if (SailorDic.ContainsKey(name)) SailorDic[name] = config;
                    else SailorDic.Add(name, config);
                }
            }
        }
        public static SailorConfig GetSailorConfig(string sailorRootName)
        {
            return SailorDic[sailorRootName];
        }
    }
}