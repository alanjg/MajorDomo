using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Dominion;
using System.IO.IsolatedStorage;
using System.IO;

namespace DominionPhone
{
	public class DifficultyModel : NotifyingObject
	{
		public DifficultyModel()
		{
			switch (this.ReadDifficulty())
			{
				case 0:
					this.Easy = true;
					break;
				case 1:
					this.Medium = true;
					break;
				case 2:
					this.Hard = true;
					break;
			}
		}

		public int ReadDifficulty()
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

			if (isoStore.FileExists("difficulty.txt"))
			{
				using (IsolatedStorageFileStream stream = isoStore.OpenFile("difficulty.txt", System.IO.FileMode.Open))
				{
					StreamReader r = new StreamReader(stream);
					string d = r.ReadLine();
					int i;
					if (int.TryParse(d, out i))
					{
						return i;
					}
				}
			}
			return 1;
		}

		public int GetDifficulty()
		{
			if (this.easy) return 0;
			if (this.medium) return 1;
			if (this.hard) return 2;
			// shouldn't get here
			return 1;
		}
		public void WriteDifficulty()
		{			
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

			using (IsolatedStorageFileStream stream = isoStore.CreateFile("difficulty.txt"))
			{
				using (StreamWriter r = new StreamWriter(stream))
				{
					r.WriteLine(this.GetDifficulty());
				}
			}
		}

		private bool easy;
		public bool Easy
		{
			get
			{
				return this.easy;
			}
			set
			{
				this.easy = value;
				this.OnPropertyChanged("Easy");
			}
		}

		private bool medium;
		public bool Medium
		{
			get
			{
				return this.medium;
			}
			set
			{
				this.medium = value;
				this.OnPropertyChanged("Medium");
			}
		}

		private bool hard;
		public bool Hard
		{
			get
			{
				return this.hard;
			}
			set
			{
				this.hard = value;
				this.OnPropertyChanged("Hard");
			}
		}
	}

	public partial class SettingsPage : PhoneApplicationPage
	{
		public SettingsPage()
		{
			this.DataContext = App.Difficulty;
			InitializeComponent();
			this.GameSetsList.DataContext = App.SupportedSets;
			this.SettingsPanel.DataContext = App.Settings;
			this.ServerAddressTextBox.DataContext = App.ServerModel;
			this.EnableMultiplayerCheckBox.DataContext = App.ServerModel;
			this.UserNameTextBox.DataContext = App.ServerModel;
			this.ProhibitedCardsTextBox.DataContext = App.ProhibitedCards;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			App.CardSetsModel = new CardSetsModel(App.SupportedSets.AllowedSets);
			App.CardCollectionModel = new CardCollectionModel(App.SupportedSets.AllowedSets);
			App.ValidateServerAddress(this.ServerAddressTextBox.Text);
			base.OnNavigatedFrom(e);
		}
	}
}