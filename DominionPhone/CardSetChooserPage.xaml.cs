using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Dominion;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace DominionPhone
{
	public partial class CardSetChooserPage : PhoneApplicationPage
	{
		public CardSetsModel Model { get; private set; }
		public CardSetChooserPage()
		{
			InitializeComponent();			
		}

		private void CardSetTap(object sender, RoutedEventArgs e)
		{
			this.Model.SelectedCardSet = (CardSetViewModel)((FrameworkElement)sender).DataContext;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.Model = App.CardSetsModel;
			this.DataContext = this.Model;
			base.OnNavigatedTo(e);
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
			if (App.CurrentGame != null)
			{
				App.CurrentGame.CancelGame();
			}

			Game game = new LocalGame();
			App.CurrentGame = game;
			IEnumerable<CardModel> cards;
			CardModel bane = null;
			if (this.Model.SelectedCardSet != null)
			{
				cards = this.Model.SelectedCardSet.CardSet.CardCollection;
				bane = this.Model.SelectedCardSet.CardSet.BaneCard;
			}
			else
			{
				cards = new RandomAllCardSet(App.SupportedSets.AllowedSets).CardCollection;				
			}

			Kingdom kingdom = new Kingdom(cards.ToList(), App.ProhibitedCards.ProhibitedCards, bane, App.SupportedSets.AllowedSets, 2, App.Settings.UseColonies, App.Settings.UseShelters, App.Settings.StartingHandType, App.Settings.UseRandomCardsFromChosenSetsOnly);

			game.GamePageModel.Kingdom = kingdom;
			game.PlayGame();
			App.RootFrame.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
		}
	}
}