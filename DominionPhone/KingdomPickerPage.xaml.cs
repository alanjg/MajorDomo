using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Dominion;
using Dominion.Model.Actions;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DominionPhone
{
	public partial class KingdomPickerPage : PhoneApplicationPage
	{
		private bool pickingBane = false;
		public CardCollectionModel Model { get; private set; }
		
		public KingdomPickerPage()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.Model = App.CardCollectionModel;
			this.DataContext = this.Model;
			base.OnNavigatedTo(e);
		}

		private void CardTap(object sender, RoutedEventArgs e)
		{
			CardViewModel card = (CardViewModel)((FrameworkElement)sender).DataContext;
			if (!this.Model.SelectedCards.Any(c => c.Name == card.Name) && (this.Model.BaneCard == null || this.Model.BaneCard.Name != card.Name))
			{
				if (this.pickingBane)
				{
					if ((card.CardModel.GetBaseCost() == 2 || card.CardModel.GetBaseCost() == 3) && !card.CardModel.CostsPotion)
					{
						this.Model.BaneCard = card;
						this.pickingBane = false;
						this.BaneButton.IsChecked = false;
					}
				}
				else if(this.Model.SelectedCards.Count < 10)
				{
					this.Model.SelectedCards.Add(card);
					if (card.CardModel is YoungWitch)
					{
						this.BaneButton.Visibility = System.Windows.Visibility.Visible;
						this.BaneDisplay.Visibility = System.Windows.Visibility.Visible;
						this.BaneButton.IsChecked = false;
					}
				}
			}
		}

		private void SelectedCardTap(object sender, RoutedEventArgs e)
		{
			CardViewModel card = (CardViewModel)((FrameworkElement)sender).DataContext;
			if (this.Model.BaneCard == card)
			{
				this.Model.BaneCard = null;
			}
			else
			{
				this.Model.SelectedCards.Remove(card);
			}
			if (card.CardModel is YoungWitch)
			{
				this.BaneButton.Visibility = System.Windows.Visibility.Collapsed;
				this.BaneDisplay.Visibility = System.Windows.Visibility.Collapsed;
				this.BaneButton.IsChecked = false;
				this.Model.BaneCard = null;
				this.pickingBane = false;
			}
		}
		
		private void Card_Hold(object sender, System.Windows.Input.GestureEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			CardViewModel card = (CardViewModel)feSender.DataContext;
			this.CardDetailsPopup.DataContext = card.CardInfo;
			this.CardDetailsPopup.Visibility = Visibility.Visible;
		}

		private void CardDetailsClick(object sender, RoutedEventArgs e)
		{
			this.CardDetailsPopup.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void Click_OK(object sender, RoutedEventArgs e)
		{
			if (this.Model.SelectedCards.Count == 10)
			{
				if (App.CurrentGame != null)
				{
					App.CurrentGame.CancelGame();
				}

				Game game = new LocalGame();
				App.CurrentGame = game;
				IList<CardModel> cards = this.Model.SelectedCards.Select(c => c.CardModel).ToList();
				CardModel bane = this.Model.BaneCard != null ? this.Model.BaneCard.CardModel : null;
				Kingdom kingdom = new Kingdom(cards, App.ProhibitedCards.ProhibitedCards, bane, App.SupportedSets.AllowedSets, 2, App.Settings.UseColonies, App.Settings.UseShelters, App.Settings.StartingHandType, App.Settings.UseRandomCardsFromChosenSetsOnly);
				game.GamePageModel.Kingdom = kingdom; 
				game.PlayGame();
				
				App.RootFrame.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
			}
		}

		private void Click_Bane(object sender, RoutedEventArgs e)
		{
			this.pickingBane = true;
		}

		private void Click_Fill(object sender, RoutedEventArgs e)
		{
			CardCollection collection = this.Model.CardCollections.First(c => c.Name == "All");
			while(this.Model.SelectedCards.Count < 10)
			{
				CardViewModel card = collection.Cards[Randomizer.Next(collection.Cards.Count)];
				if (!this.Model.SelectedCards.Any(c => c.Name == card.Name) && (this.Model.BaneCard == null || this.Model.BaneCard.Name != card.Name))
				{
					this.Model.SelectedCards.Add(card); 
				}
			}
		}

		private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Pivot p = (Pivot)sender;
			CardCollection c = (CardCollection)p.SelectedItem;
			if (c != null)
			{
				if (c.DelayLoadedCards.Count == 0)
				{
					c.DelayLoadedCards.AddRange(c.Cards);
				}
			}
		}
	}
}