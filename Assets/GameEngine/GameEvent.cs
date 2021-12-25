using System.Collections.Generic;
using UnityEngine.Events;

public class GameEvent
{
    public static readonly UnityEvent SquadChanged = new UnityEvent();
    public static readonly UnityEvent<SailorModel> SailorInfoChanged = new UnityEvent<SailorModel>();
    public static readonly UnityEvent PrepareSquadChanged = new UnityEvent();
    public static readonly UnityEvent<List<string>> UserDataChanged = new UnityEvent<List<string>>();
    public static readonly UnityEvent<int, int> UserStaminaChanged = new UnityEvent<int, int>();
    public static readonly UnityEvent<long, long> UserBeriChanged = new UnityEvent<long, long>();


}
