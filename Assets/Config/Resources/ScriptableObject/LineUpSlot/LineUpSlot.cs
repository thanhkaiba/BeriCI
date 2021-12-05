using UnityEngine;

[CreateAssetMenu(fileName = "LineUpSlot", menuName = "config/LineUpSlot")]
public class LineUpSlot : ScriptableObjectPro
{
    public int max = 5;
    public int new_user_default = 1;
    public int[] costs = { 10, 100, 200, 500 };
}
