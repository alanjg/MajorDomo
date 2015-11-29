using Dominion;
using Dominion.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace DominionUWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
		public static DominionAppContext Context { get; private set; }
		public static Frame RootFrame;

		public static void StartLocalGame(Kingdom kingdom)
		{
			if (App.Context.CurrentGame != null)
			{
				App.Context.CurrentGame.CancelGame();
			}
			Game game = new LocalGame(App.Context);
			App.Context.CurrentGame = game;
			game.GamePageModel.Kingdom = kingdom;
			game.PlayGame();
			App.RootFrame.Navigate(typeof(GamePage));
			game.ExitingGame += game_ExitingGame;
		}

		static private void game_ExitingGame(object sender, EventArgs e)
		{
			App.RootFrame.Navigate(typeof(MainPage));
		}

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
			App.Context = new DominionAppContext();
            this.Suspending += OnSuspending;
		}

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }
			RootFrame = rootFrame;

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(MainPage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
			Dominion.ViewModelDispatcher.SetDispatcher(new JupiterDispatcher(rootFrame.Dispatcher));
            // Ensure the current window is active
            Window.Current.Activate();
        }

		private class JupiterDispatcher : Dominion.IDispatcher
		{
			private CoreDispatcher dispatcher;
			public JupiterDispatcher(CoreDispatcher dispatcher)
			{
				this.dispatcher = dispatcher;
			}

			public void BeginInvoke(Action action)
			{
				this.dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() => { action(); }));
			}
		}

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
			SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
			this.DoSuspendTasks(deferral);
		}
		private async void DoSuspendTasks(SuspendingDeferral deferral)
		{
			await App.Context.DoSuspendTasks();
            deferral.Complete();
        }
    }
}
