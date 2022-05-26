using Newtonsoft.Json;
using UnityEngine;

public class LoginLogData
{
    /**
		* Là key, được tạo ra từ các thành phần bên dưới
		*/
    public static string device_id = SystemInfo.deviceUniqueIdentifier;

    /**
		* Tên thiết bị
		*/
    public static string name = SystemInfo.deviceName;

    /**
		* thông số mạng
		*/
    private string subscriber_Id;

    /**
	* thông số sim
	*/
    private string sim_serial;


    /**
	* Chỉ dùng cho máy Android
	*/
    private string android_id;
    /**
		* 
		*/
    private string mac_address;
    /**
		* platform : android, ios, web, windowsphone8
		*/
    private string platform = Application.platform.ToString();
    /**
		* true/false . Máy iOS là dùng cho jailbreak
		*/
    private string rooted;
    /**
		* 
		*/
    private string finger_print;
    /**
		* ios,windows phone, android, windows, mac
		*/
    private string os = SystemInfo.operatingSystem;
    private string os_version = "";
    /**
		* số tự sinh
		*/
    private string udid;
    /**
		* 
		*/
    private string bluetooth_address;
    /**
		* ID quảng cáo
		*/
    private string advertising_id;
    /**
		* Là check sum tất cả các thông tin user gửi lên.
		*/
    private string checksum;
    /**
		* IP login lần đầu
		*/
    private string first_ip;
    /**
		* IP login lần cuối
		*/
    private string last_ip;

    private string channel;
    /**
		* ngôn ngữ đang sử dụng
		*/
    private string lang = "en";
    /**
		* version của user
		*/
    private string app_version = Application.version;

    private int width = Screen.width;
    private int height = Screen.width;
    private string carrier;
    private string bundle_id;
    private string location;
    private string network = Application.internetReachability.ToString();

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
