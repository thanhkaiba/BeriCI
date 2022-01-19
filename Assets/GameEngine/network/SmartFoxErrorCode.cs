using System.ComponentModel;

namespace Piratera.Network
{
    public enum SFSErrorCode
    {
        [Description("Success")]
        SUCCESS = 800,
        [Description("Unknown error")]
        UNKNOWN_ERROR = 801,
        [Description("Invalid data")]
        INVALID_DATA = 802,
        [Description("Fighting line list empty")]
        FIGHTING_LINE_LIST_EMPTY = 803,
        [Description("Sailor list empty")]
        SAILOR_LIST_EMPTY = 804,
        [Description("Pick team fighting line list invalid")]
        PICK_TEAM_FIGHTING_LINE_LIST_INVALID = 805,  // ft không hợp lệ (sửa tên cho nó dể hiểu lỗi của lệnh nào )
        [Description("Combat fighting finished")]
        COMBAT_FIGHTING_FINISHED = 806,   //Chặn lệnh đúp ( chưa xử lý )
        [Description("Sailor configuration invalid")]
        SAILOR_CONFIG_INVALID = 807, //Không load được sailor config file
        [Description("Combat sailor and fighiting line invalid")]
        COMBAT_SAILOR_AND_FIGHITING_LINE_INVALID = 808,   //sailor và ft không hợp lệ ( kể cả số lượng lẫn id phải khớp nhau )
        [Description("Stamina sold out")]
        STAMINA_SOLD_OUT = 809,   //hết lượt mua
        [Description("Not enough Beri")]
        STAMINA_BERI_NOT_ENOUGH = 810,   //ko đủ beri để mua
        [Description("Not enough Stamina")]
        COMBAT_STAMINA_NOT_ENOUGH = 811, //ko du stamina de choi
        [Description("Combat not prepare")]
        COMBAT_NOT_PREPARE = 812,    // chua chuan bi
        [Description("Not enough Beri")]
        BUY_SLOT_BERI_NOT_ENOUGH = 813,
        [Description("Slot sold out")]
        BUY_SLOT_POSITION_SOLD_OUT = 814,
        [Description("Invalid data")]
        COMBAT_CONFIRM_SLOT_INVALID = 815,
        [Description("Invalid Game State")]
        INVALID_GAME_STATE = 816,

        [Description("Server under maintenance")]
        SERVER_MAINTAINACE = 817,

        [Description("Your username or password is incorrect")]
        WRONG_USER_NAME_OR_PASS = 819,

        [Description("Your username or password is incorrect")]
        WRONG_USER_NAME_OR_PASS_2 = 818,

        [Description("A suitable opponent could not be found right now")]
        NO_OPPONENT_FOUND = 820,

    }
}