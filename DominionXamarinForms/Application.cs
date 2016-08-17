using System;
using System.ComponentModel;
using Xamarin.Forms;
using Dominion;

namespace DominionXamarinForms
{
	public class App : Application
	{
		public static Game CurrentGame { get; set; }
		public static ThreadHelper ThreadHelperInstance { get; set; }
		public App ()
		{
			// The root page of your application
			MainPage = new NavigationPage(new GamePage());
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

	public class XamarinDispatcher : IDispatcher
	{
		public void BeginInvoke(Action action)
		{
			Device.BeginInvokeOnMainThread (action);
		}
	}
}

