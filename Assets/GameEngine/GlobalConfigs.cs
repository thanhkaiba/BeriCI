using UnityEngine;

public class GlobalConfigs : Object
{
    public static ContainerClassBonus ClassBonus = Resources.Load<ContainerClassBonus>("ScriptableObject/ClassBonus/ContainerClassBonus");
    public static CombatConfig Combat = Resources.Load<CombatConfig>("ScriptableObject/Combat");
    public static SailorGeneralConfig SailorGeneral = Resources.Load<SailorGeneralConfig>("ScriptableObject/SailorGeneralConfig");
    public static PvEConfig PvE = Resources.Load<PvEConfig>("ScriptableObject/PvE/PvE");

}
