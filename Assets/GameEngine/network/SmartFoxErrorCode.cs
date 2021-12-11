using System.ComponentModel;

public enum SFSErrorCode
{
    [Description("Success")]
    SUCCESS = 0,
    [Description("Unknown error")]
    UNKNOWN_ERROR = 1,
    [Description("Invalid data")]
    INVALID_DATA = 2,
    [Description("Fighting line list empty")]
    FIGHTING_LINE_LIST_EMPTY = 3,
    [Description("Sailor list empty")]
    SAILOR_LIST_EMPTY = 4,
    [Description("Pick team fighting line list invalid")]
    PICK_TEAM_FIGHTING_LINE_LIST_INVALID =	5,  // ft không hợp lệ (sửa tên cho nó dể hiểu lỗi của lệnh nào )
    [Description("Combat fighting finished")]
    COMBAT_FIGHTING_FINISHED = 6,   //Chặn lệnh đúp ( chưa xử lý )
    [Description("Sailor configuration invalid")]
    SAILOR_CONFIG_INVALID =	7, //Không load được sailor config file
    [Description("Combat sailor and fighiting line invalid")]
    COMBAT_SAILOR_AND_FIGHITING_LINE_INVALID = 8,   //sailor và ft không hợp lệ ( kể cả số lượng lẫn id phải khớp nhau )
    [Description("Stamina sold out")]
    STAMINA_SOLD_OUT = 9,   //hết lượt mua
    [Description("Not enough Beri")]
    STAMINA_BERI_NOT_ENOUGH = 10,   //ko đủ beri để mua
    [Description("Not enough Stamina")]
    COMBAT_STAMINA_NOT_ENOUGH =	11, //ko du stamina de choi
    [Description("Combat not prepare")]
    COMBAT_NOT_PREPARE = 12,    // chua chuan bi
    [Description("Not enough Beri")]
    BUY_SLOT_BERI_NOT_ENOUGH = 13,
    [Description("Slot sold out")]
    BUY_SLOT_POSITION_SOLD_OUT = 14,
    [Description("Invalid data")]
    COMBAT_CONFIRM_SLOT_INVALID = 15,
    [Description("Invalid Game State")]
    INVALID_GAME_STATE = 69,
   
}