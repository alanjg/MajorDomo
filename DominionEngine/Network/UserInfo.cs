using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public class UserInfo
	{
		public UserInfo()
		{
			this.Users = new List<string>();
		}
		public List<string> Users { get; set; }
	}
}
