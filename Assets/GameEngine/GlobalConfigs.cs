using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfigs : Object
{
    public static ContainerClassBonus ClassBonus = Resources.Load<ContainerClassBonus>("ScriptableObject/ClassBonus/ContainerClassBonus");
    public static CombatConfig Combat = Resources.Load<CombatConfig>("ScriptableObject/Combat");
    public static SailorGeneralConfig SailorGeneral = Resources.Load<SailorGeneralConfig>("ScriptableObject/SailorGeneralConfig");

}
