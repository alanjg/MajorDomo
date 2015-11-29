using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DominionSL
{
	public class EffectViewModel : IDisplayable
	{
		private string text;
		public EffectViewModel(string choiceText)
		{
			this.text = choiceText;
		}

		public CardDisplayModel DisplayModel
		{
			get { return new TextCardDisplayModel(this.text); }
		}
	}
}
