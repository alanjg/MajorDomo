using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Dominion;
using Dominion.Model.Actions;

namespace DominionSL
{
	public class CardViewModel : IDisplayable
	{
		private CardModel card;
		public CardModel CardModel { get { return this.card; } }

		public CardViewModel(CardModel card)
		{
			this.card = card;
		}

		public string Name
		{
			get { return this.card.Name; }
		}

		public string Image
		{
			get { return "Resources/CardImages/" + this.card.Expansion + "/" + this.card.Name.Replace(" ", "").Replace("-","").Replace("'","") + ".jpg"; }
		}

		public override string ToString()
		{
			return this.Name;
		}

		public bool Is(CardType cardType)
		{
			return this.CardModel.Is(cardType);
		}

		public CardDisplayModel DisplayModel
		{
			get { return new ImageCardDisplayModel(this.Image); }
		}
	}
}