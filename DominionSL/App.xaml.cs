using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Dominion;
using Dominion.Network;

namespace DominionSL
{
	public partial class App : Application
	{
		public static Game CurrentGame { get; set; }
		public static DifficultyModel Difficulty { get; set; }
		public static GameHistory History { get; private set; }
		public static bool HasLoadedGameRecords { get; set; }

		public static Settings Settings { get; private set; }
		public static GameSetCollection SupportedSets { get; private set; }
		public static CardSetsModel CardSetsModel { get; set; }
		public static CardCollectionModel CardCollectionModel { get; set; }
		public static GameLobbyModel GameLobbyModel { get; private set; }
		public static ServerConnection ServerConnection { get; set; }
		public static ServerModel ServerModel { get; set; }

		public GameViewModel GameViewModel { get; set; }
		public GamePageModel GamePageModel { get; set; }
		
		public static Dispatcher UIDispatcher { get; private set; }
		
		public App()
		{
			this.Startup += this.Application_Startup;
			this.Exit += this.Application_Exit;
			this.UnhandledException += this.Application_UnhandledException;
			
			App.GameLobbyModel = new GameLobbyModel();
			SLSocket socket = new SLSocket();
			App.ServerConnection = new ServerConnection(App.GameLobbyModel, socket);

			InitializeComponent();
		}

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			this.RootVisual = new MainPage();
			App.UIDispatcher = this.RootVisual.Dispatcher;
			ViewModelDispatcher.SetDispatcher(new SLDispatcher(this.RootVisual.Dispatcher));
		}

		private class SLDispatcher : IDispatcher
		{
			private Dispatcher dispatcher;
			public SLDispatcher(Dispatcher dispatcher)
			{
				this.dispatcher = dispatcher;
			}

			public void BeginInvoke(Action action)
			{
				this.dispatcher.BeginInvoke(action);
			}
		}

		private void Application_Exit(object sender, EventArgs e)
		{
			//((MainPage)this.RootVisual).OnExit();
		}

		private void OnCardClicked(object sender, MouseButtonEventArgs e)
		{
			CardViewModel card = ((FrameworkElement)sender).DataContext as CardViewModel;
			if (card != null)
			{
				this.GamePageModel.InvokeCard(card);
			}
			EffectViewModel effect = ((FrameworkElement)sender).DataContext as EffectViewModel;
			if (effect != null)
			{
				this.GamePageModel.InvokeEffect(effect);
			}
		}

		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			// If the app is running outside of the debugger then report the exception using
			// the browser's exception mechanism. On IE this will display it a yellow alert 
			// icon in the status bar and Firefox will display a script error.
			if (!System.Diagnostics.Debugger.IsAttached)
			{
				// NOTE: This will allow the application to continue running after an exception has been thrown
				// but not handled. 
				// For production applications this error handling should be replaced with something that will 
				// report the error to the website and stop the application.
				e.Handled = true;
				Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
			}
		}

		private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
		{
			try
			{
				string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
				errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

				System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
			}
			catch (Exception)
			{
			}
		}
	}
}
