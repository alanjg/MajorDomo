using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Dominion.Model.Actions;

namespace Dominion
{
	public class Pile : NotifyingObject
	{
		public CardModel Card { get { return this.instance; } }

		private CardModel topCard;
		public CardModel TopCard
		{
			get { return this.topCard; }
			set { this.topCard = value; this.OnPropertyChanged("TopCard"); }
		}

		private int count;
		public int Count
		{
			get { return this.count; }
			set { this.count = value; this.OnPropertyChanged("Count"); }
		}

		private int embargoCount;
		public int EmbargoCount
		{
			get { return this.embargoCount; }
			set { this.embargoCount = value; this.OnPropertyChanged("EmbargoCount"); }
		}

		private int tradeRouteCount;
		public int TradeRouteCount
		{
			get { return this.tradeRouteCount; }
			set { this.tradeRouteCount = value; this.OnPropertyChanged("TradeRouteCount"); }
		}

		private int cost;
		public int Cost
		{
			get { return this.cost; }
			set { this.cost = value; this.OnPropertyChanged("Cost"); }
		}

		private bool costsPotion;
		public bool CostsPotion
		{
			get { return this.costsPotion; }
			set { this.costsPotion = value; this.OnPropertyChanged("CostsPotion"); }
		}

		private bool contrabanded;
		public bool Contrabanded
		{
			get { return this.contrabanded; }
			set { this.contrabanded = value; this.OnPropertyChanged("Contrabanded"); }
		}
		private string name;
		private GameModel gameModel;
		private CardModel instance;
		private Type cardType;
		private int originalCost;
		// used for Ruins, Knights
		private Stack<CardModel> uniqueCards;

		public Pile(int count, GameModel gameModel, Type cardType)
			: this(count, gameModel, cardType, (CardModel)Activator.CreateInstance(cardType))
		{			
		}

		public Pile(int count, GameModel gameModel, Type cardType, CardModel instance)
		{
			this.gameModel = gameModel;
			this.cardType = cardType;
			this.instance = instance;
			this.CostsPotion = this.instance.CostsPotion;
			this.Contrabanded = false;
			this.originalCost = gameModel.GetCost(this.instance);
			this.Cost = this.originalCost;
			this.Count = count;
			this.EmbargoCount = 0;
			this.TradeRouteCount = 0;
			this.name = instance.Name;
			
			if (this.instance.Is(CardType.Knight))
			{
				List<CardModel> cards = new List<CardModel>();
				cards.AddRange(Knights.AllKnights);
				
				this.uniqueCards = new Stack<CardModel>();
				foreach (CardModel card in cards.OrderBy(c => Randomizer.Next()))
				{
					this.uniqueCards.Push(card);
				}
				this.TopCard = this.uniqueCards.Peek();
			}
			else if (this.instance.Is(CardType.Ruins))
			{
				List<CardModel> cards = new List<CardModel>();
				for (int i = 0; i < 10; i++)
				{
					cards.Add(new AbandonedMine());
					cards.Add(new RuinedLibrary());
					cards.Add(new RuinedMarket());
					cards.Add(new RuinedVillage());
					cards.Add(new Survivors());
				}

				this.uniqueCards = new Stack<CardModel>();
				foreach (CardModel card in cards.OrderBy(c => Randomizer.Next()))
				{
					this.uniqueCards.Push(card);
				}
				this.TopCard = this.uniqueCards.Peek();
			}
			else
			{
				this.TopCard = this.instance;
			}
			this.UpdateCost();
		}

		void pileState_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e.PropertyName);
		}

		public CardModel DrawCard()
		{
			if (this.Count == 0)
			{
				return null;
			}
			this.Count--;
			if (this.TradeRouteCount == 1)
			{
				this.gameModel.AddTradeRouteToken();
				this.TradeRouteCount = 0;
			}

			this.OnPropertyChanged("Count");
			if (this.uniqueCards != null)
			{
				CardModel card = this.uniqueCards.Pop();
				this.TopCard = this.uniqueCards.Count > 0 ? this.uniqueCards.Peek() : null;
				this.OnPropertyChanged("TopCard");
				this.OnPropertyChanged("Name");
				this.UpdateCost();
				return card;
			}
			return this.instance.Clone();
		}

		public void PutCardOnPile(CardModel card)
		{
			this.Count++;
			this.OnPropertyChanged("Count");
			if (this.uniqueCards != null)
			{
				this.uniqueCards.Push(card);
				this.TopCard = card;
				this.OnPropertyChanged("TopCard");
				this.OnPropertyChanged("Name");
				this.UpdateCost();
			}
		}

		public int GetCost()
		{
			return this.TopCard != null ? this.gameModel.GetCost(this.TopCard) : this.Cost;
		}

		public void UpdateCost()
		{
			this.Cost = this.gameModel.GetCost(this.TopCard != null ? this.TopCard : this.Card);
		}

		public string Name
		{
			get { return this.TopCard != null ? this.TopCard.Name : this.name; }
		}

		public string ID
		{
			get { return this.TopCard != null ? this.TopCard.ID: this.instance.ID; }
		}

		public Pile Clone(GameModel gameModel)
		{
			Pile clone = new Pile(this.count, gameModel, this.cardType, this.instance.Clone());
			clone.contrabanded = this.contrabanded;
			clone.cost = this.cost;
			clone.costsPotion = this.costsPotion;
			clone.embargoCount = this.embargoCount;
			clone.topCard = this.topCard.Clone();
			clone.tradeRouteCount = this.tradeRouteCount;
			if (this.uniqueCards != null)
			{
				foreach (CardModel card in this.uniqueCards.Reverse())
				{
					clone.uniqueCards.Push(card.Clone());
				}
			}
			return clone;
		}
	}
}
