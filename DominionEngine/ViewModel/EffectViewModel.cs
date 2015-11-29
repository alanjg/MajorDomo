using System;
using System.Net;

namespace Dominion
{
	public class EffectViewModel : NotifyingObject, IDisplayable
	{
		private string choice;
		private string choiceDescription;
		public EffectViewModel(string choice, string choiceDescription)
		{
			this.choice = choice;
			this.choiceDescription = choiceDescription;
		}

		public CardDisplayModel DisplayModel
		{
			get { return new TextCardDisplayModel(this.choice); }
		}

		public string Choice { get { return this.choice; } }
		public string ChoiceDescription { get { return this.choiceDescription; } }

		private bool isSelected;
		public bool IsSelected
		{
			get { return this.isSelected; }
			set { this.isSelected = value; this.OnPropertyChanged("IsSelected"); }
		}
	}
}
