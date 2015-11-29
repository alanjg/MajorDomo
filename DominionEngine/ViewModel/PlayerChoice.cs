using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion
{
	public enum ChoiceSourceType
	{
		Card,
		Pile,
		Effect
	}

	public class PlayerChoiceParameters
	{
		public ChoiceSourceType SourceType { get; set; }
		public ChoiceSource Source { get; set; }
		public string ChoiceText { get; set; }
		public int MinChoices { get; set; }
		public int MaxChoices { get; set; }
		public bool Order { get; set; }
		public IEnumerable<CardViewModel> CardChoices { get; set; }
		public IEnumerable<PileViewModel> PileChoices { get; set; }
		public IEnumerable<EffectViewModel> EffectChoices { get; set; }
	}

	public class PlayerChoice
	{
		public IEnumerable<CardViewModel> ChosenCards { get; set; }
		public IEnumerable<PileViewModel> ChosenPiles { get; set; }
		public IEnumerable<EffectViewModel> ChosenEffects { get; set; }
	}
}
