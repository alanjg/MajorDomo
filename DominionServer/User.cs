using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DominionServer
{
	public class User
	{
		private static int userSeed = 0;
		public User(string name, Connection connection)
		{
			this.Name = name;
			this.Authentication = userSeed++;
			this.Connection = connection;
		}
		public string Name { get; private set; }
		public int Authentication { get; private set; }
		public Connection Connection { get; private set; }
		public Lobby Lobby { get; set; }
	}
}
