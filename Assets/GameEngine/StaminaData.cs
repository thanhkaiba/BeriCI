using Piratera.Utils;
using Sfs2X.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StaminaData : Singleton<StaminaData>
{
    public int Stamina;
    private long LastCountStamina;
    private int TimeBuyStaminaToday;
    UserStaminaConfig StaminaConfig;
    protected override void OnAwake()
    {
        LoadExpConfig();
    }

    private void LoadExpConfig()
    {
        StaminaConfig = Resources.Load<UserStaminaConfig>("ScriptableObject/Stamina/Stamina");
    }

    public string GetCurrentStaminaFormat()
    {
        return GetStaminaFormat(Stamina);
    }

    public string GetStaminaFormat(int stamina)
    {
        return $"{StringUtils.ShortNumber(stamina)}/{StaminaConfig.max_stamina}";
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
        return Stamina < StaminaConfig.max_stamina;
    }

    public long TimeToHaveNewStamina()
    {
        if (Stamina > StaminaConfig.max_stamina)
        {
            return -1;
        }

        long now = GameTimeMgr.GetCurrentTime();
        long delta = now - LastCountStamina;
        int recoveringTime = StaminaConfig.recovering_time * 1000;
        delta = recoveringTime - delta % recoveringTime;

        return delta;
    }

    public void OnUserVariablesUpdate(User user)
    {
        LastCountStamina = (long)user.GetVariable(UserInfoPropertiesKey.LAST_COUNT).GetDoubleValue();
        TimeBuyStaminaToday = user.GetVariable(UserInfoPropertiesKey.TIME_BUY_STAMINA_TODAY).GetIntValue();
        int oldStamina = Stamina;
        Stamina = user.GetVariable(UserInfoPropertiesKey.STAMINA).GetIntValue();

        if (oldStamina != Stamina)
        {
            GameEvent.UserStaminaChanged.Invoke(oldStamina, Stamina);
        }
    }
    public int GetPvECost()
    {
        return StaminaConfig.pve_cost;
    }
}