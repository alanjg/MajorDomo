using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Dominion.Model.Chooser
{
	public abstract class ChooserBase : IChooser
	{
		public CardModel ChooseZeroOrOneCard(CardChoiceType choiceType, string choiceText, ChoiceSource source, IEnumerable<CardModel> choices)
		{
			return this.ChooseSeveralCards(choiceType, choiceText, source, 0, 1, choices).FirstOrDefault();
		}

		public CardModel ChooseZeroOrOneCard(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, IEnumerable<CardModel> choices)
		{
			return this.ChooseSeveralCards(choiceType, choiceText, source, 0, 1, choices).FirstOrDefault();
		}

		public Pile ChooseZeroOrOnePile(CardChoiceType choiceType, string choiceText, IEnumerable<Pile> choices)
		{
			return this.ChooseSeveralPiles(choiceType, choiceText, 0, 1, choices).FirstOrDefault();
		}

		public CardModel ChooseOneCard(CardChoiceType choiceType, string choiceText, ChoiceSource source, IEnumerable<CardModel> choices)
		{
			return ChooseOneCard(choiceType, null, choiceText, source, choices);
		}

		public CardModel ChooseOneCard(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, IEnumerable<CardModel> choices)
		{
			if (choices.GroupBy(new Func<CardModel, string>(c => c.Name)).Count() <= 1)
			{
				return choices.FirstOrDefault();
			}
			else
			{
				return this.ChooseCards(choiceType, cardInfo, choiceText, source, 1, 1, choices).FirstOrDefault();
			}
		}

		public Pile ChooseOnePile(CardChoiceType choiceType, string choiceText, IEnumerable<Pile> choices)
		{
			if (choices.Count() <= 1)
			{
				return choices.FirstOrDefault();
			}
			else
			{
				return this.ChoosePiles(choiceType, choiceText, 1, 1, choices).First();
			}
		}

		public IEnumerable<CardModel> ChooseSeveralCards(CardChoiceType choiceType, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices)
		{
			return ChooseSeveralCards(choiceType, null, choiceText, source, minChoices, maxChoices, choices);
		}

		public IEnumerable<CardModel> ChooseSeveralCards(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices)
		{
			if(choices.Count() <= minChoices)
			{
				return choices;
			}
			else if (minChoices == maxChoices && choices.GroupBy(new Func<CardModel, string>(c => c.Name)).Count() <= 1)
			{
				return choices.Take(minChoices);
			}
			else
			{
				return this.ChooseCards(choiceType, cardInfo, choiceText, source, minChoices, maxChoices, choices);
			}
		}

		public IEnumerable<Pile> ChooseSeveralPiles(CardChoiceType choiceType, string choiceText, int minChoices, int maxChoices, IEnumerable<Pile> choices)
		{
			if (choices.Count() <= minChoices)
			{
				return choices;
			}
			else
			{
				return this.ChoosePiles(choiceType, choiceText, minChoices, maxChoices, choices);
			}
		}

		public IEnumerable<CardModel> ChooseOrder(CardOrderType choiceType, string choiceText, IEnumerable<CardModel> choices)
		{
			if (choices.GroupBy(new Func<CardModel, string>(c => c.Name)).Count() <= 1)
			{
				return choices;
			}
			else
			{
				return Order(choiceType, choiceText, choices);
			}
		}

		public int ChooseOneEffect(EffectChoiceType choiceType, CardModel cardInfo, string choiceText, string[] choices, string[] choiceDescriptions)
		{
			return this.ChooseEffects(choiceType, new CardModel[] { cardInfo }, choiceText, 1, 1, choices, choiceDescriptions).First();
		}

		public int ChooseOneEffect(EffectChoiceType choiceType, IEnumerable<CardModel> cardInfo, string choiceText, string[] choices, string[] choiceDescriptions)
		{
			return this.ChooseEffects(choiceType, cardInfo, choiceText, 1, 1, choices, choiceDescriptions).First();
		}

		public int ChooseOneEffect(EffectChoiceType choiceType, string choiceText, string[] choices, string[] choiceDescriptions)
		{
			return this.ChooseEffects(choiceType, new CardModel[] { }, choiceText, 1, 1, choices, choiceDescriptions).First();
		}

		public IEnumerable<int> ChooseSeveralEffects(EffectChoiceType choiceType, IEnumerable<CardModel> cardInfo, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions)
		{
			return this.ChooseEffects(choiceType, cardInfo, choiceText, minChoices, maxChoices, choices, choiceDescriptions);
		}

		public IEnumerable<int> ChooseSeveralEffects(EffectChoiceType choiceType, CardModel cardInfo, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions)
		{
			return this.ChooseEffects(choiceType, new CardModel[] { cardInfo }, choiceText, minChoices, maxChoices, choices, choiceDescriptions);
		}

		public IEnumerable<int> ChooseSeveralEffects(EffectChoiceType choiceType, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions)
		{
			return this.ChooseEffects(choiceType, new CardModel[] { } , choiceText, minChoices, maxChoices, choices, choiceDescriptions);
		}

		public abstract IEnumerable<CardModel> ChooseCards(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices);
		public abstract IEnumerable<Pile> ChoosePiles(CardChoiceType choiceType, string choiceText, int minChoices, int maxChoices, IEnumerable<Pile> choices);
		public abstract IEnumerable<int> ChooseEffects(EffectChoiceType choiceType, IEnumerable<CardModel> cardInfo, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions);
		public abstract IEnumerable<CardModel> Order(CardOrderType choiceType, string choiceText, IEnumerable<CardModel> choices);
	}
}
