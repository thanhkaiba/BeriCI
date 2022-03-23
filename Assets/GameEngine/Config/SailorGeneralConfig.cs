public class SailorGeneralConfig
{
    public int MAX_QUALITY = 200;
    public float MAX_MIN_POWER_RATIO = 1.5f;
    public float MAX_MIN_HEALTH_RATIO = 1.5f;
    public float MAX_MIN_SPEED_RATIO = 1.2f;

    public float POWER_PER_LEVEL_RATIO = 0.1f;
    public float HEALTH_PER_LEVEL_RATIO = 0.1f;

    public int START_LEVEL = 0;
    public int MAX_LEVEL = 20;
    public int EXP_PARAMS = 500;

    public float STAR_STAT_RATE = 0.2f;
    public int MAX_STAR = 5;

    public int EARNABLE_FIGHT = 1200;
    public int TRIAL_EARNABLE_FIGHT = 5;
    public int TRIAL_FIGHT = 10;

    public int GetNextLevelExp(int curLevel)
    {
        int x = curLevel + 1;
        return EXP_PARAMS * x * x - EXP_PARAMS * x;
    }
}
