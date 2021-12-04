public enum SFSErrorCode
{
    SUCCESS = 0,
    UNKNOWN_ERROR = 1,
    INVALID_DATA = 2,
    FIGHTING_LINE_LIST_EMPTY = 3,
    SAILOR_LIST_EMPTY = 4,
    PICK_TEAM_FIGHTING_LINE_LIST_INVALID =	5,	// ft không hợp lệ (sửa tên cho nó dể hiểu lỗi của lệnh nào )
    COMBAT_FIGHTING_FINISHED = 6,	//Chặn lệnh đúp ( chưa xử lý )
    SAILOR_CONFIG_INVALID =	7, //Không load được sailor config file
    COMBAT_SAILOR_AND_FIGHITING_LINE_INVALID = 8,	//sailor và ft không hợp lệ ( kể cả số lượng lẫn id phải khớp nhau )
    STAMINA_SOLD_OUT = 9,	//hết lượt mua
    STAMINA_BERI_NOT_ENOUGH = 10,	//ko đủ beri để mua
    COMBAT_STAMINA_NOT_ENOUGH =	11,	//ko du stamina de choi
    COMBAT_NOT_PREPARE = 12,	// chua chuan bi
    BUY_SLOT_BERI_NOT_ENOUGH = 13,
    BUY_SLOT_POSITION_SOLD_OUT = 14,
    COMBAT_CONFIRM_SLOT_INVALID = 15
}