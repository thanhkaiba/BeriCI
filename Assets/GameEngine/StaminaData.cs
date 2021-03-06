using Piratera.Config;
using Piratera.Utils;
using Sfs2X.Entities;

public class StaminaData : Singleton<StaminaData>
{
    public int Stamina = -1;
    public long LastCountStamina;

    public string GetCurrentStaminaFormat()
    {
        return GetStaminaFormat(Stamina);
    }

    public string GetStaminaFormat(int stamina)
    {
        return $"{StringUtils.ShortNumber(stamina)}/{GlobalConfigs.StaminaConfig.GetStaminaCapacity()}";
    }

    public string GetStaminaFormat(string stamina)
    {
        return $"{stamina}/{GlobalConfigs.StaminaConfig.GetStaminaCapacity()}";
    }
    public string _GetStaminaFormat(int stamina)
    {
        return stamina.ToString();
    }
    public int GetStamina()
    {
        return Stamina;
    }

    public bool IsRecorveringStamina()
    {
        return Stamina < GlobalConfigs.StaminaConfig.GetStaminaCapacity();
    }

    public long TimeToHaveNewStamina()
    {
        if (Stamina > GlobalConfigs.StaminaConfig.GetStaminaCapacity() || Stamina < 0)
        {
            return -1;
        }

        long now = GameTimeMgr.GetCurrentTime();

        long delta = now - LastCountStamina;
        int recoveringTime = GlobalConfigs.StaminaConfig.recovering_time * 1000;
        long remain = recoveringTime - delta % recoveringTime;


        return remain;
    }

    public void OnUserVariablesUpdate(User user)
    {
        LastCountStamina = (long)user.GetVariable(UserInfoPropertiesKey.LAST_COUNT).GetDoubleValue();
        int oldStamina = Stamina;
        Stamina = user.GetVariable(UserInfoPropertiesKey.STAMINA).GetIntValue();

        if (oldStamina != Stamina)
        {
            GameEvent.UserStaminaChanged.Invoke(oldStamina, Stamina);
        }
    }
    public void AddStamina(int quantity)
    {
        int oldStamina = Stamina;
        Stamina += quantity;
        GameEvent.UserStaminaChanged.Invoke(oldStamina, Stamina);
    }
    public int GetPvECost()
    {
        return GlobalConfigs.StaminaConfig.pve_cost;
    }

}