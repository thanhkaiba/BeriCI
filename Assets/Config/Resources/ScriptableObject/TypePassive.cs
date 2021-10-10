using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TypePassive", menuName = "TypePassive")]
public class TypePassive : ScriptableObjectPro
{
    public SailorType type = SailorType.MIGHTY;
    public List<int> pop;
    public List<float> param_1;
    public List<float> param_2;
}
