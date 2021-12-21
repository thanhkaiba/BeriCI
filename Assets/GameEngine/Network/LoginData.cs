namespace Piratera.Network
{
	public class LoginData
	{
		public LoginData(string username, string password, GameLoginType type)
		{
			Username = username;
			Password = password;
			Type = type;
		}
		public string Username = "";
		public string Password = "";
		public GameLoginType Type = GameLoginType.AUTHENTICATON;

	};
}