using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public class ChoiceInfo
	{
		public ChoiceInfo()
		{
			this.Choices = new List<string>();
		}
		public List<string> Choices { get; set; }
	}
}
