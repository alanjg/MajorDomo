using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Dominion;
using Dominion.Network;

namespace DominionXamarinForms
{
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
			InitializeComponent ();
		}

        private void newSinglePlayerGamePreset(object sender, EventArgs e)
        {
            App.Instance.MainPage = new CardSetChooserPage();
        }

        private void newSinglePlayerGameCustom(object sender, EventArgs e)
        {
            App.Instance.MainPage = new KingdomPickerPage();
        }

        private void newSinglePlayerGameRandom(object sender, EventArgs e)
        {
            if (App.CurrentGame != null)
            {
                App.CurrentGame.CancelGame();
            }

            Game game = new LocalGame();
            App.CurrentGame = game;
            Kingdom kingdom = new Kingdom(null, App.ProhibitedCards.ProhibitedCards, null, App.SupportedSets.AllowedSets, 2, App.Settings.UseColonies, App.Settings.UseShelters, App.Settings.StartingHandType, App.Settings.UseRandomCardsFromChosenSetsOnly);
            game.GamePageModel.Kingdom = kingdom;
            game.PlayGame();
            App.Instance.MainPage = new GamePage();
        }

        private void newMultiPlayerGame(object sender, EventArgs e)
        {
            if (App.CurrentGame != null)
            {
                App.CurrentGame.CancelGame();
            }
            string address = !string.IsNullOrEmpty(App.ServerModel.ServerAddress) ? App.ServerModel.ServerAddress : ServerModel.PublicServerAddress;
            string username = App.ServerModel.UserName;
            App.ServerConnection.Connect(address, username);

            App.Instance.MainPage = new GameLobbyPage();
        }

        private void resumeGame(object sender, EventArgs e)
        {
            if (App.CurrentGame != null && !App.CurrentGame.GamePageModel.GameViewModel.GameOver)
            {
                App.Instance.MainPage = new GamePage();
            }
        }

        private void settings(object sender, EventArgs e)
        {
            App.Instance.MainPage = new SettingsPage();
        }

        private void about(object sender, EventArgs e)
        {
            App.Instance.MainPage = new AboutPage();
        }

        private void records(object sender, EventArgs e)
        {
            App.Instance.MainPage = new RecordsPage();
        }
    }
}

