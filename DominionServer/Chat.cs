using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionServer
{
	public class Chat
	{
		public Chat(User sender, string message, DateTime time)
		{
			this.Sender = sender;
			this.Message = message;
			this.Time = time;
		}

		public User Sender { get; private set; }
		public string Message { get; private set; }
		public DateTime Time { get; private set; }
	}
}
