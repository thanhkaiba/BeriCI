using Newtonsoft.Json;
using UnityEngine;

namespace Piratera.Config {
    public class GlobalConfigs : Object
    {
        public static ContainerClassBonus ClassBonus = Resources.Load<ContainerClassBonus>("ScriptableObject/ClassBonus/ContainerClassBonus");
        public static SailorGeneralConfig SailorGeneral;
        public static PvEConfig PvE;
        public static LineUpSlot LineUp;
        public static UserStaminaConfig StaminaConfig;
        public static CombatConfig Combat;

        public static void InitSyncConfig()
        {
            LineUp = JsonConvert.DeserializeObject<LineUpSlot>(GameConfigSync.GetContent("LineUpSlot.json"));
            PvE = JsonConvert.DeserializeObject<PvEConfig>(GameConfigSync.GetContent("PvE.json"));
            SailorGeneral = JsonConvert.DeserializeObject<SailorGeneralConfig>(GameConfigSync.GetContent("SailorGeneralConfig.json"));
            StaminaConfig = JsonConvert.DeserializeObject<UserStaminaConfig>(GameConfigSync.GetContent("Stamina.json"));
            Combat = JsonConvert.DeserializeObject<CombatConfig>(GameConfigSync.GetContent("Combat.json"));
        }
    }
}