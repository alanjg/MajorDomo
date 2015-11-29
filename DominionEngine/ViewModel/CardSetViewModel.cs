using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Dominion;

namespace Dominion
{
	public class CardCollection
	{
		public string Name { get; private set; }
		public ObservableCollection<CardViewModel> Cards { get; private set; }
		public ObservableCollection<CardViewModel> DelayLoadedCards { get; private set; }
		public CardCollection(string name)
		{
			this.Name = name;
			this.Cards = new ObservableCollection<CardViewModel>();
			this.DelayLoadedCards = new ObservableCollection<CardViewModel>();
		}
	}

	public class CustomCardSet : CardSet
	{
		public CustomCardSet(IEnumerable<CardViewModel> cards, CardViewModel bane)
		{
			base.cardCollection = new ObservableCollection<CardModel>();
			foreach (CardViewModel c in cards)
			{
				base.cardCollection.Add(c.CardModel);
			}
			if (bane != null)
			{
				this.BaneCard = bane.CardModel;
			}
		}

		public override string CardSetName
		{
			get { return "Custom"; }
		}
		public override GameSets GameSet
		{
			get { return GameSets.Any; }
		}
	}

	public class CardCollectionModel : NotifyingObject
	{
		public CardCollectionModel(GameSets allowedSets)
		{
			this.SelectedCards = new ObservableCollection<CardViewModel>();
			this.CardCollections = new ObservableCollection<CardCollection>();
			Dictionary<string, CardCollection> lookup = new Dictionary<string,CardCollection>();
			CardCollection all = new CardCollection("All");
			foreach (CardModel card in CardModelFactory.AllCards)
			{
				if (card.IsKingdomCard && (card.GameSet & allowedSets) != 0)
				{
					CardCollection collection;
					if (!lookup.TryGetValue(card.Expansion, out collection))
					{
						collection = new CardCollection(card.Expansion);
						lookup[card.Expansion] = collection;
						this.CardCollections.Add(collection);
					}
					this.InsertSorted(collection.Cards, new CardViewModel(card));
					this.InsertSorted(all.Cards, new CardViewModel(card));
				}
			}
			this.CardCollections.Add(all);
		}
		private void InsertSorted(ObservableCollection<CardViewModel> collection, CardViewModel card)
		{
			for (int i = 0; i < collection.Count; i++)
			{
				if (string.Compare(collection[i].Name, card.Name) > 0)
				{
					collection.Insert(i, card);
					return;
				}
			}
			collection.Add(card);
		}

		public ObservableCollection<CardCollection> CardCollections { get; private set; }
		public ObservableCollection<CardViewModel> SelectedCards { get; private set; }
		private CardViewModel baneCard;
		public CardViewModel BaneCard 
		{
			get { return this.baneCard; }
			set { this.baneCard = value; this.OnPropertyChanged("BaneCard"); }
		}
	}

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
