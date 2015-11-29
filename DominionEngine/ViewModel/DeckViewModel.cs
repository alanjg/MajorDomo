using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Dominion;
using Dominion.Model.Actions;

namespace Dominion
{
	public sealed class DeckViewModel : NotifyingObject, IEnumerable<CardViewModel>, INotifyCollectionChanged
	{
		private List<CardViewModel> cards;
		private Deck deck;
		public DeckViewModel(Deck deck)
		{
			this.deck = deck;
			this.cards = new List<CardViewModel>();
			this.deck.CollectionChanged += new NotifyCollectionChangedEventHandler(deck_CollectionChanged);
		}

		void deck_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			ViewModelDispatcher.BeginInvoke(new Action(delegate()
			{
				if (this.CollectionChanged != null)
				{
					this.CollectionChanged(this, e);
				}
				this.OnPropertyChanged("Count");
			}));
		}

		public int Count
		{
			get { return this.deck.Count; }
		}

		#region IEnumerable<CardModel> Members

		public IEnumerator<CardViewModel> GetEnumerator()
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

		#region INotifyCollectionChanged Members

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion
	}
}
