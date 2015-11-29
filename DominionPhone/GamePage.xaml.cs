using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Dominion;
using DominionEngine.AI;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;

namespace DominionPhone
{
	public partial class GamePage : PhoneApplicationPage
	{
		private GamePageModel gamePageModel;

		public GamePage()
		{
			InitializeComponent();			
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (this.gamePageModel == null && App.CurrentGame != null)
			{
				this.HookModel(App.CurrentGame.GamePageModel);
			}
			else
			{
				App.RootFrame.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
				App.RootFrame.RemoveBackEntry();
			}
		}

		private void HookModel(GamePageModel model)
		{
			this.gamePageModel = model;
			this.DataContext = this.gamePageModel;
			this.gamePageModel.GameViewModel.TextLog.PropertyChanged += TextLog_PropertyChanged;
			this.gamePageModel.GameException += gamePageModel_GameException;
		}

		void gamePageModel_GameException(Exception e)
		{
			this.LogScrollViewer.Content = new TextBlock() { Text = e.StackTrace };
			this.StatusText.Text = e.Message;
		}

		void TextLog_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Text")
			{
				this.Dispatcher.BeginInvoke(new Action(() =>
				{
					this.LogScrollViewer.ScrollToVerticalOffset(((FrameworkElement)this.LogScrollViewer.Content).ActualHeight + this.LogScrollViewer.ActualHeight);
				}));
			}
		}

		private void Card_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			CardViewModel card = (CardViewModel)feSender.DataContext;
			this.gamePageModel.InvokeCard(card);		
		}

		private void Card_Hold(object sender, System.Windows.Input.GestureEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			CardViewModel card = (CardViewModel)feSender.DataContext;
			//this.CardDetailsImage.Source = new BitmapImage(new Uri(card.Image, UriKind.Relative));
			this.CardDetailsPopup.DataContext = card.CardInfo;
			this.CardDetailsPopup.Visibility = Visibility.Visible;
		}

		private void CardDetailsClick(object sender, RoutedEventArgs e)
		{
			this.CardDetailsPopup.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void Pile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			PileViewModel pile = (PileViewModel)feSender.DataContext;
			this.gamePageModel.InvokePile(pile);
		}

		private void Pile_Hold(object sender, System.Windows.Input.GestureEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			PileViewModel pile = (PileViewModel)feSender.DataContext;
			if(pile.TopCard != null)
			{
				//this.CardDetailsImage.Source = new BitmapImage(new Uri(pile.TopCard.Image, UriKind.Relative));
				this.CardDetailsPopup.DataContext = pile.TopCard.CardInfo;
				this.CardDetailsPopup.Visibility = Visibility.Visible;
			}
		}

		private void Effect_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			EffectViewModel effect = (EffectViewModel)feSender.DataContext;
			this.gamePageModel.InvokeEffect(effect);
		}		

		private void BuyPhase_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.EnterBuyPhase();
		}

		private void PlayAll_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.PlayAll();
		}

		private void PlayCoins_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.PlayCoinTokens();
		}

		private void EndTurn_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.EndTurn();
		}

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.MakeChoice();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			App.CurrentGame.ExitGame();
			App.CurrentGame = null;
		}
	}
}