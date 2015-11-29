using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionServer
{
	public class Lobby
	{
		public Lobby()
		{
			this.Users = new List<User>();
			this.Chats = new List<Chat>();
		}

		public IEnumerable<Chat> GetChatsSince(DateTime time)
		{
			foreach (Chat chat in this.Chats)
			{
				if (chat.Time >= time)
				{
					yield return chat;
				}
			}
		}

		public List<User> Users { get; private set; }
		public List<Chat> Chats { get; private set; }
	}
}
