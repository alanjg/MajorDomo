using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using DominionXamarinForms;
using Dominion;

namespace DominionXamarin.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			App.ThreadHelperInstance = new iOSThreadHelper();
			ViewModelDispatcher.SetDispatcher (new XamarinDispatcher ());
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

