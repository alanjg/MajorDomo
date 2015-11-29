using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Dominion.Model.Actions;
using Dominion.Model.Chooser;

namespace Dominion
{
	public static class ListExtensions
	{
		public static void Shuffle<T>(this List<T> list)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = Randomizer.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		public static int Points(this IList<CardModel> list, Player player)
		{
			int sum = 0;
			for(int i=0;i<list.Count;i++)
			{
				sum += list[i].GetVictoryPoints(player);
			}
			return sum;
		}
	}
	public sealed class Deck : IEnumerable<CardModel>, INotifyCollectionChanged
	{
		private List<CardModel> cards;
		public int GetPoints(Player player)
		{
			return cards.Points(player);
		}

		private Player owner;
		public Deck(Player owner)
		{
			this.owner = owner;
			this.cards = new List<CardModel>();
		}

		public void Shuffle()
		{
			this.Shuffle(false);
		}

		public void Shuffle(bool ignoreStash)
		{
			this.cards.Shuffle();

			if (this.owner.GameModel.GameHasStash && !ignoreStash)
			{
				IEnumerable<CardModel> stashes = this.cards.Where(c => c is Stash);
				if (stashes.Any())
				{
					List<CardModel> toShuffle = new List<CardModel>();
					foreach (CardModel card in this.cards)
					{
						if (card is Stash)
						{
							toShuffle.Add(card);
						}
						else
						{
							toShuffle.Add(new BlankCard());
						}
					}
					List<CardModel> newDeck = new List<CardModel>();
					IEnumerator<CardModel> deckEnumerator = this.cards.GetEnumerator();
					foreach (CardModel ordered in this.owner.Chooser.ChooseOrder(CardOrderType.Stash, "You may put your stashes anywhere in the deck", toShuffle))
					{
						if (ordered is Stash)
						{
							newDeck.Add(ordered);
						}
						else
						{
							deckEnumerator.MoveNext();
							while (deckEnumerator.Current is Stash)
							{
								deckEnumerator.MoveNext();
							}
							newDeck.Add(deckEnumerator.Current);
						}
					}
					this.cards = newDeck;
				}
			}

			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		public int Count
		{
			get { return this.cards.Count; }
		}

		public CardModel Draw()
		{
			CardModel card = null;
			if (this.cards.Count > 0)
			{
				card = this.cards[cards.Count - 1];
				this.cards.RemoveAt(cards.Count - 1);
				if (this.CollectionChanged != null)
				{
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, card, cards.Count));
				}
			}
			return card;
		}

		public CardModel DrawFromBottom()
		{
			CardModel card = null;
			if (this.cards.Count > 0)
			{
				card = this.cards[0];
				this.cards.RemoveAt(0);
				if (this.CollectionChanged != null)
				{
					this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, card, 0));
				}
			}
			return card;
		}

		public void Populate(IList<CardModel> cards)
		{
			this.Populate(cards, true);
		}

		public void Populate(IList<CardModel> cards, bool shuffle)
		{
			this.cards.AddRange(cards);
			if (shuffle)
			{
				this.Shuffle();
			}
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		#region IEnumerable<CardModel> Members

		public IEnumerator<CardModel> GetEnumerator()
		{
			return this.cards.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.cards.GetEnumerator();
		}

		#endregion

		internal void PlaceOnTop(CardModel card)
		{
			this.cards.Add(card);
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, card, this.cards.Count - 1));
			}
		}

		internal void PlaceOnBottom(CardModel card)
		{
			this.cards.Insert(0, card);
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, card, 0));
			}
		}

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion

		internal Deck Clone(Player owner)
		{
			Deck clone = new Deck(owner);
			clone.cards.AddRange(this.cards.Select(c => c.Clone()));
			return clone;
		}
	}
}
