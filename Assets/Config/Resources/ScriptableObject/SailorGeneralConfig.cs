using UnityEngine;

[CreateAssetMenu(fileName = "SailorGeneralConfig", menuName = "config/SailorGeneral")]
public class SailorGeneralConfig : ScriptableObjectPro
{
    public int MAX_QUALITY = 200;
    public float MAX_MIN_POWER_RATIO = 1.5f;
    public float MAX_MIN_HEALTH_RATIO = 1.5f;
    public float MAX_MIN_SPEED_RATIO = 1.2f;

    public float POWER_PER_LEVEL_RATIO = 0.1f;
    public float HEALTH_PER_LEVEL_RATIO = 0.1f;
}
