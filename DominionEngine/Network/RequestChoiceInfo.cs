using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public class RequestChoiceInfo
	{
		public RequestChoiceInfo()
		{
			this.Choices = new List<string>();
			this.ChoiceDescriptions = new List<string>();
		}
		public string ChoiceText { get; set; }
		public string ChoiceType { get; set; }
		public string ChoiceSource { get; set; }
		public int MinChoices { get; set; }
		public int MaxChoices { get; set; }
		public List<string> Choices { get; set; }
		public List<string> ChoiceDescriptions { get; set; }
	}
}
