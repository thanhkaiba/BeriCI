namespace Piratera.Network
{
    public enum SFSAction
    {
        PING = 1,
        MESSAGE_ERROR = 2,
        JOIN_ZONE_SUCCESS = 3,
        LOAD_USER_INFO = 4,
        LOAD_LIST_HERO_INFO = 5,
        TEAM_COMMIT = 6,
        COMBAT_BOT = 7,
        LOAD_TEAM = 8,
        BUY_STAMINA = 9,
        COMBAT_PREPARE = 10,
        COMBAT_DATA = 11,
        PVE_SURRENDER = 12,
        PVE_CONFIRM = 13,
        PVE_PLAY = 14,
        BUY_SLOT = 20,
        GET_STAMINA_PACK = 21,
        GET_LINEUP_SLOT_PACK = 22,
        GET_SERVER_TIME = 23,
        GET_CONFIG_MANIFEST = 24,
        CONFIRM_LINEUP = 25,
        GET_CONFIG = 26,
        PIRATE_WHEEL = 27,
        PIRATE_WHEEL_DATA = 28,


        UPDATE_ARENA_FGL = 53,

#if PIRATERA_DEV || PIRATERA_QC
        // cheat
        CHEAT_SAILOR = 101,
        CHEAT_RESOURCE = 102,
        CHEAT_RANK = 103,
        CHEAT_SAILOR_QUANTITY = 104,
#endif
    }
}
