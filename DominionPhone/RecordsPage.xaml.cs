using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using Dominion;
using System.Windows.Data;

namespace DominionPhone
{
	public partial class RecordsPage : PhoneApplicationPage
	{
		public static GameRecord CurrentRecord { get; set; }
		public RecordsPage()
		{
			this.DataContext = App.History;
			this.ReadRecords();
			InitializeComponent();
		}

		private void ReadRecords()
		{
			if (!App.HasLoadedGameRecords)
			{
				List<GameRecord> records = new List<GameRecord>();
				IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
				if (isoStore.DirectoryExists("games"))
				{
					foreach (string game in isoStore.GetFileNames("games\\*.txt"))
					{
						using (IsolatedStorageFileStream stream = isoStore.OpenFile("games\\" + game, System.IO.FileMode.Open))
						{
							XmlSerializer x = new XmlSerializer(typeof(GameRecord));
							try
							{
								GameRecord record = (GameRecord)x.Deserialize(stream);
								foreach(GameRecord r in App.History.GameRecords)
								{
									if(r.Log.Count == record.Log.Count)
									{
										bool same = true;
										for (int i = 0; i < r.Log.Count; i++)
										{
											if (r.Log[i] != record.Log[i])
											{
												same = false;
												break;
											}
										}
										if (same)
										{
											record = null;
											break;
										}
									}
								}
								// skip dupes already logged this session.
								if (record != null)
								{
									records.Add(record);
								}
							}
							catch
							{
								isoStore.DeleteFile("games\\" + game);
							}
						}
					}
				}
				records.Reverse();
				App.History.GameRecords.AddRange(records);
				App.HasLoadedGameRecords = true;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();
			
			// reset history
			if (isoStore.DirectoryExists("games"))
			{
				foreach (string game in isoStore.GetFileNames("games\\*.txt"))
				{
					isoStore.DeleteFile("games\\" + game);
				}
			}
			App.History.GameRecords.Clear();
			App.History.Wins = 0;
			App.History.Losses = 0;
		}

		private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
		{
			GameRecord record = ((FrameworkElement)sender).DataContext as GameRecord;
			if (record != null)
			{
				RecordsPage.CurrentRecord = record;
				App.RootFrame.Navigate(new Uri("/GameRecordViewerPage.xaml", UriKind.Relative));
			}
		}
	}
}