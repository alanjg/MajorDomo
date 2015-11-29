using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using DominionPhone.Resources;
using Dominion;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using Dominion.Network;
using System.IO;
using System.Net;

namespace DominionPhone
{
	public partial class App : Application
	{
		/// <summary>
		/// Provides easy access to the root frame of the Phone Application.
		/// </summary>
		/// <returns>The root frame of the Phone Application.</returns>
		public static PhoneApplicationFrame RootFrame { get; private set; }

		public static Dispatcher UIDispatcher { get; private set; }

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
				
		/// <summary>
		/// Constructor for the Application object.
		/// </summary>
		public App()
		{
			// Global handler for uncaught exceptions.
			UnhandledException += Application_UnhandledException;

			// Standard XAML initialization
			InitializeComponent();

			// Phone-specific initialization
			InitializePhoneApplication();

			// Language display initialization
			InitializeLanguage();

			App.Difficulty = new DifficultyModel();
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

			if (isoStore.FileExists("history.txt"))
			{
				try
				{
					using (IsolatedStorageFileStream stream = isoStore.OpenFile("history.txt", System.IO.FileMode.Open))
					{
						XmlSerializer x = new XmlSerializer(typeof(GameHistory));
						App.History = (GameHistory)x.Deserialize(stream);
					}
				}
				catch
				{
					App.History = new GameHistory();
				}
			}
			else
			{
				App.History = new GameHistory();
			}

			if (isoStore.FileExists("settings.xml"))
			{
				try
				{
					using (IsolatedStorageFileStream stream = isoStore.OpenFile("settings.xml", System.IO.FileMode.Open))
					{
						XmlSerializer x = new XmlSerializer(typeof(Settings));
						App.Settings = (Settings)x.Deserialize(stream);
					}
				}
				catch
				{
					App.Settings = new Settings();
				}
			}
			else
			{
				App.Settings = new Settings();
			}

			if (isoStore.FileExists("supportedsets.xml"))
			{
				using (IsolatedStorageFileStream stream = isoStore.OpenFile("supportedsets.xml", System.IO.FileMode.Open))
				{
					XmlSerializer x = new XmlSerializer(typeof(GameSetCollection));
					App.SupportedSets = (GameSetCollection)x.Deserialize(stream);
				}
			}
			else
			{
				App.SupportedSets = new GameSetCollection();
			}

			if (isoStore.FileExists("prohibitedcards.xml"))
			{
				try
				{
					using (IsolatedStorageFileStream stream = isoStore.OpenFile("prohibitedcards.xml", System.IO.FileMode.Open))
					{
						XmlSerializer x = new XmlSerializer(typeof(ProhibitedCardsCollection));
						App.ProhibitedCards = (ProhibitedCardsCollection)x.Deserialize(stream);
					}
				}
				catch
				{
					App.ProhibitedCards = new ProhibitedCardsCollection();
				}
			}
			else
			{
				App.ProhibitedCards = new ProhibitedCardsCollection();
			}


			App.ServerModel = new ServerModel();
			if (isoStore.FileExists("serveraddress.xml"))
			{
				try
				{
					using (IsolatedStorageFileStream stream = isoStore.OpenFile("serveraddress.xml", System.IO.FileMode.Open))
					{
						XmlSerializer x = new XmlSerializer(typeof(ServerModel));
						App.ServerModel = (ServerModel)x.Deserialize(stream);
					}
				}
				catch
				{
					App.ServerModel = new ServerModel();
				}
			}
			else
			{
				App.ServerModel.ServerAddress = string.Empty;
				App.ServerModel.IsValidAddress = false;
			}

			App.CardSetsModel = new CardSetsModel(App.SupportedSets.AllowedSets);
			App.CardCollectionModel = new CardCollectionModel(App.SupportedSets.AllowedSets);
			App.GameLobbyModel = new GameLobbyModel();
			WPSocket socket = new WPSocket();
			App.ServerConnection = new ServerConnection(App.GameLobbyModel, socket);

			// Show graphics profiling information while debugging.
			if (Debugger.IsAttached)
			{
				// Display the current frame rate counters.
				Application.Current.Host.Settings.EnableFrameRateCounter = true;

				// Show the areas of the app that are being redrawn in each frame.
				//Application.Current.Host.Settings.EnableRedrawRegions = true;

				// Enable non-production analysis visualization mode,
				// which shows areas of a page that are handed off to GPU with a colored overlay.
				//Application.Current.Host.Settings.EnableCacheVisualization = true;

				// Prevent the screen from turning off while under the debugger by disabling
				// the application's idle detection.
				// Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
				// and consume battery power when the user is not using the phone.
				PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
			}

		}

		public static void ValidateServerAddress(string address)
		{
			IPAddress ipAddress;
			if (IPAddress.TryParse(address, out ipAddress))
			{
				App.ServerModel.ServerAddress = address;
				App.ServerModel.IsValidAddress = true;
			}
			else
			{
				App.ServerModel.ServerAddress = string.Empty;
				App.ServerModel.IsValidAddress = false;
			}
		}

		private static void WriteServerModel()
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

			using (IsolatedStorageFileStream stream = isoStore.CreateFile("serveraddress.xml"))
			{
				XmlSerializer x = new XmlSerializer(typeof(ServerModel));
				x.Serialize(stream, App.ServerModel);
			}
		}

		private static void WriteHistory()
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

			using (IsolatedStorageFileStream stream = isoStore.CreateFile("history.txt"))
			{
				XmlSerializer x = new XmlSerializer(typeof(GameHistory));
				x.Serialize(stream, App.History);
			}
		}

		private static void WriteSupportedSets()
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

			using (IsolatedStorageFileStream stream = isoStore.CreateFile("supportedsets.xml"))
			{
				XmlSerializer x = new XmlSerializer(typeof(GameSetCollection));
				x.Serialize(stream, App.SupportedSets);
			}
		}

		private static void WriteProhibitedCards()
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

			using (IsolatedStorageFileStream stream = isoStore.CreateFile("prohibitedcards.xml"))
			{
				XmlSerializer x = new XmlSerializer(typeof(ProhibitedCardsCollection));
				x.Serialize(stream, App.ProhibitedCards);
			}
		}

		private static void WriteSettings()
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

			using (IsolatedStorageFileStream stream = isoStore.CreateFile("settings.xml"))
			{
				XmlSerializer x = new XmlSerializer(typeof(Settings));
				x.Serialize(stream, App.Settings);
			}
		}

		// Code to execute when the application is launching (eg, from Start)
		// This code will not execute when the application is reactivated
		private void Application_Launching(object sender, LaunchingEventArgs e)
		{
		}

		// Code to execute when the application is activated (brought to foreground)
		// This code will not execute when the application is first launched
		private void Application_Activated(object sender, ActivatedEventArgs e)
		{
		}

		// Code to execute when the application is deactivated (sent to background)
		// This code will not execute when the application is closing
		private void Application_Deactivated(object sender, DeactivatedEventArgs e)
		{
			App.Difficulty.WriteDifficulty();
			App.WriteHistory();
			App.WriteSupportedSets();
			App.WriteProhibitedCards();
			App.WriteServerModel();
			App.WriteSettings();
		}

		// Code to execute when the application is closing (eg, user hit Back)
		// This code will not execute when the application is deactivated
		private void Application_Closing(object sender, ClosingEventArgs e)
		{
			App.Difficulty.WriteDifficulty();
			App.WriteHistory(); 
			App.WriteSupportedSets();
			App.WriteProhibitedCards();
			App.WriteServerModel();
			App.WriteSettings();
		}

		// Code to execute if a navigation fails
		private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			if (Debugger.IsAttached)
			{
				// A navigation has failed; break into the debugger
				Debugger.Break();
			}
		}

		// Code to execute on Unhandled Exceptions
		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (Debugger.IsAttached)
			{
				// An unhandled exception has occurred; break into the debugger
				Debugger.Break();
			}
			ErrorPage.LastException = e.ExceptionObject;
			e.Handled = true;
			App.RootFrame.Navigate(new Uri("/ErrorPage.xaml", UriKind.Relative));
		}

		#region Phone application initialization

		// Avoid double-initialization
		private bool phoneApplicationInitialized = false;

		// Do not add any additional code to this method
		private void InitializePhoneApplication()
		{
			if (phoneApplicationInitialized)
				return;

			// Create the frame but don't set it as RootVisual yet; this allows the splash
			// screen to remain active until the application is ready to render.
			RootFrame = new PhoneApplicationFrame();
			RootFrame.Navigated += CompleteInitializePhoneApplication;

			// Handle navigation failures
			RootFrame.NavigationFailed += RootFrame_NavigationFailed;

			// Handle reset requests for clearing the backstack
			RootFrame.Navigated += CheckForResetNavigation;

			// Ensure we don't initialize again
			phoneApplicationInitialized = true;
		}

		private class PhoneDispatcher : Dominion.IDispatcher
		{
			private Dispatcher dispatcher;
			public PhoneDispatcher(Dispatcher dispatcher)
			{
				this.dispatcher = dispatcher;
			}

			public void BeginInvoke(Action action)
			{
				this.dispatcher.BeginInvoke(action);
			}
		}

		// Do not add any additional code to this method
		private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
		{
			// Set the root visual to allow the application to render
			if (RootVisual != RootFrame)
				RootVisual = RootFrame;
			
			Dominion.ViewModelDispatcher.SetDispatcher(new PhoneDispatcher(RootVisual.Dispatcher));

			// Remove this handler since it is no longer needed
			RootFrame.Navigated -= CompleteInitializePhoneApplication;
		}

		private void CheckForResetNavigation(object sender, NavigationEventArgs e)
		{
			// If the app has received a 'reset' navigation, then we need to check
			// on the next navigation to see if the page stack should be reset
			if (e.NavigationMode == NavigationMode.Reset)
				RootFrame.Navigated += ClearBackStackAfterReset;
		}

		private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
		{
			// Unregister the event so it doesn't get called again
			RootFrame.Navigated -= ClearBackStackAfterReset;

			// Only clear the stack for 'new' (forward) and 'refresh' navigations
			if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
				return;

			// For UI consistency, clear the entire page stack
			while (RootFrame.RemoveBackEntry() != null)
			{
				; // do nothing
			}
		}

		#endregion

		// Initialize the app's font and flow direction as defined in its localized resource strings.
		//
		// To ensure that the font of your application is aligned with its supported languages and that the
		// FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
		// and ResourceFlowDirection should be initialized in each resx file to match these values with that
		// file's culture. For example:
		//
		// AppResources.es-ES.resx
		//    ResourceLanguage's value should be "es-ES"
		//    ResourceFlowDirection's value should be "LeftToRight"
		//
		// AppResources.ar-SA.resx
		//     ResourceLanguage's value should be "ar-SA"
		//     ResourceFlowDirection's value should be "RightToLeft"
		//
		// For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
		//
		private void InitializeLanguage()
		{
			try
			{
				// Set the font to match the display language defined by the
				// ResourceLanguage resource string for each supported language.
				//
				// Fall back to the font of the neutral language if the Display
				// language of the phone is not supported.
				//
				// If a compiler error is hit then ResourceLanguage is missing from
				// the resource file.
				RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

				// Set the FlowDirection of all elements under the root frame based
				// on the ResourceFlowDirection resource string for each
				// supported language.
				//
				// If a compiler error is hit then ResourceFlowDirection is missing from
				// the resource file.
				FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
				RootFrame.FlowDirection = flow;
			}
			catch
			{
				// If an exception is caught here it is most likely due to either
				// ResourceLangauge not being correctly set to a supported language
				// code or ResourceFlowDirection is set to a value other than LeftToRight
				// or RightToLeft.

				if (Debugger.IsAttached)
				{
					Debugger.Break();
				}

				throw;
			}
		}
	}
}