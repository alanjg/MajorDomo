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
	public sealed class DeckViewModel : IEnumerable<CardViewModel>, INotifyCollectionChanged
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
			App.UIDispatcher.BeginInvoke(new Action(delegate()
			{
				if (this.CollectionChanged != null)
				{
					this.CollectionChanged(this, e);
				}
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
