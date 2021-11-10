using UnityEngine;

[CreateAssetMenu(fileName = "Stamina", menuName = "config/Stamina")]
public class StaminaConfig : ScriptableObjectPro
{
    public int max_stamina = 200;
    public int pve_cost = 5;
    public int pvp_cost = 5;
    public int recovering_time = 600;
    public int[] costs = { 10, 100, 200, 500, 1000 };
}
