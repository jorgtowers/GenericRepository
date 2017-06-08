using System;

namespace Utils.ServicioChat
{
	public class Message 
	{
		#region Members
		public string user;
		public string msg;
		public MsgType type;
		#endregion 


		#region Constructors
		public Message(string _user, string _msg, MsgType _type) 
		{
			user = _user;
			msg = _msg;
			type = _type;
		}
		public Message(string _user, MsgType _type) : this(_user, "", _type) { }
		public Message(MsgType _type) : this("", "", _type) { }
		#endregion 

		#region Methods
		public override string ToString()
		{
			switch(this.type)
			{
				case MsgType.Msg:
                    return this.user + " " + DateTime.Now.Hour.ToString().PadLeft(2, '0').ToString() + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0').ToString() + "> " + this.msg;
				case MsgType.Join :
					return this.user + " Bienvenido ";
				case MsgType.Left :
					return this.user + " La conversación ha terminado...";
			}
			return "";
		}
		#endregion 

	}

	public enum MsgType { Msg, Start, Join, Left, Action }

	
}
