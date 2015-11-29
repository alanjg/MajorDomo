using System;
using System.Collections.ObjectModel;
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

namespace DominionSL
{
	public class CardSetViewModel
	{
		private CardSet cardSet;
		public CardSetViewModel(CardSet cardSet)
		{
			this.cardSet = cardSet;
		}

		public CardSet CardSet
		{
			get { return this.cardSet; }
		}

		public ObservableCollection<CardViewModel> CardCollection
		{
			get { return new ObservableCollection<CardViewModel>(this.cardSet.CardCollection.Select(c => new CardViewModel(c))); }
		}

		public string CardSetName
		{
			get { return this.cardSet.CardSetName; }
		}

		public GameSets GameSet
		{
			get { return this.cardSet.GameSet; }
		}
	}
}
