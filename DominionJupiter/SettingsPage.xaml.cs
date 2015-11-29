using Dominion;
using DominionJupiter.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DominionJupiter
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingsPage : Page
	{
		private NavigationHelper navigationHelper;

		public SettingsPage()
		{
			this.InitializeComponent();
			if (Cpu.IsARM())
			{
				this.X86Difficulty.Visibility = Visibility.Collapsed;
			}
			else
			{
				this.ARMDifficulty.Visibility = Visibility.Collapsed;
			}
			this.navigationHelper = new NavigationHelper(this);
			this.DataContext = App.Context.Difficulty;
			this.GameSetsList.DataContext = App.Context.SupportedSets;
			this.ServerAddressTextBox.DataContext = App.Context.ServerModel;
			this.EnableMultiplayerCheckBox.DataContext = App.Context.ServerModel;
			this.UserNameTextBox.DataContext = App.Context.ServerModel;
			this.SettingsPanel.DataContext = App.Context.Settings;
			this.ProhibitedCardsTextBox.DataContext = App.Context.ProhibitedCards;
		}

		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			App.Context.CardSetsModel = new CardSetsModel(App.Context.SupportedSets.AllowedSets);
			navigationHelper.OnNavigatedFrom(e);
			App.Context.ValidateServerAddress(this.ServerAddressTextBox.Text);
		}
	}
}
