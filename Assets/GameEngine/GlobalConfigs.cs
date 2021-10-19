using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalConfigs : Singleton<GlobalConfigs>
{
    public ContainerClassBonus ClassBonus;
    protected override void OnAwake()
    {
        ClassBonus = Resources.Load<ContainerClassBonus>("ScriptableObject/ClassBonus/ContainerClassBonus");
    }
}
