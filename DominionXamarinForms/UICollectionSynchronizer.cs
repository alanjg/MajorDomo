using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using Xamarin.Forms;
using Dominion;
using System.Collections.ObjectModel;

namespace DominionXamarinForms
{
	public class UICollectionSynchronizer
	{
		private Dictionary<CardViewModel, Button> mapper = new Dictionary<CardViewModel, Button>();
		private StackLayout stackLayout;
		private GamePageModel gamePageModel;
		public UICollectionSynchronizer (ReadOnlyObservableCollection<CardViewModel> collection, StackLayout stackLayout, GamePageModel gamePageModel)
			:this(collection, collection, stackLayout, gamePageModel)
		{
			
		}

		public UICollectionSynchronizer (ObservableCollection<CardViewModel> collection, StackLayout stackLayout, GamePageModel gamePageModel)
			:this(collection, collection, stackLayout, gamePageModel)
		{

		}

		private UICollectionSynchronizer (IList<CardViewModel> collection, INotifyCollectionChanged collectionChanged, StackLayout stackLayout, GamePageModel gamePageModel)
		{
			this.stackLayout = stackLayout;
			this.gamePageModel = gamePageModel;
			((INotifyCollectionChanged)collection).CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => {
				if (e.Action == NotifyCollectionChangedAction.Reset) {
					this.stackLayout.Children.Clear ();
					this.mapper.Clear ();
				} else {
					if(e.OldItems != null)
					{
						foreach (CardViewModel card in e.OldItems) {
							this.RemoveItem(card);
						}
					}

					if(e.NewItems != null)
					{
						foreach (CardViewModel card in e.NewItems) {
							this.AddItem(card);
						}
					}
				}
			};
			foreach (CardViewModel card in collection) {
				this.AddItem (card);
			}
		}

		private void AddItem(CardViewModel card)
		{
			Button button = new Button () { Text = card.Name };
			button.Clicked += (object sender2, EventArgs e2) => this.gamePageModel.InvokeCard (card);
			ViewModelDispatcher.BeginInvoke (new Action (() => {
				this.stackLayout.Children.Add (button);
			}));

			this.mapper [card] = button;
		}

		private void RemoveItem(CardViewModel card)
		{
			Button button;
			if (this.mapper.TryGetValue (card, out button)) {
				this.mapper.Remove (card);

				ViewModelDispatcher.BeginInvoke (new Action (() => {
					this.stackLayout.Children.Remove (button);

				}));
			}
		}
	}
}

