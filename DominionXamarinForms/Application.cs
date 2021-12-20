using System;
using System.ComponentModel;
using Xamarin.Forms;
using Dominion;
using Dominion.Model;
using Dominion.Network;

namespace DominionXamarinForms
{
	public class App : Application
	{
		public static ThreadHelper ThreadHelperInstance { get; set; }
        public static App Instance { get; private set; }

        public static Game CurrentGame { get; set; }
        public static DifficultyModel Difficulty { get; set; }
        public static GameHistory History { get; private set; }
        public static GameSetCollection SupportedSets { get; private set; }
        public static ProhibitedCardsCollection ProhibitedCards { get; private set; }
        public static Settings Settings { get; private set; }
        public static bool HasLoadedGameRecords { get; set; }
        public static CardSetsModel CardSetsModel { get; set; }
        public static CardCollectionModel CardCollectionModel { get; set; }
        public static GameLobbyModel GameLobbyModel { get; private set; }
        public static ServerConnection ServerConnection { get; set; }
        public static ServerModel ServerModel { get; set; }
        public App ()
		{
            App.Instance = this;
            App.Difficulty = new DifficultyModel();

            App.History = new GameHistory();
            App.Settings = new Settings();
            App.SupportedSets = new GameSetCollection();
            App.ProhibitedCards = new ProhibitedCardsCollection();
            App.ServerModel = new ServerModel();
            App.CardSetsModel = new CardSetsModel(App.SupportedSets.AllowedSets);
            App.CardCollectionModel = new CardCollectionModel(App.SupportedSets.AllowedSets);
            App.GameLobbyModel = new GameLobbyModel();
            //WPSocket socket = new WPSocket();
            //App.ServerConnection = new ServerConnection(App.GameLobbyModel, socket);
            

            MainPage = new DominionXamarinForms.MainPage();
		}
	
		protected override void OnStart ()	
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}

	public class XamarinDispatcher : Dominion.IDispatcher
	{
		public void BeginInvoke(Action action)
		{
			Device.BeginInvokeOnMainThread (action);
		}
	}
}

