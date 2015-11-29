using Dominion;
using DominionJupiter.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
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
	public sealed partial class RecordsPage : Page
	{

		private NavigationHelper navigationHelper;
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);
		}

		public static GameRecord CurrentRecord { get; set; }
		public RecordsPage()
		{
			this.DataContext = App.Context.History;
			this.ReadRecords();
			this.navigationHelper = new NavigationHelper(this);
			InitializeComponent();			
		}

		private async void ReadRecords()
		{
			await App.Context.ReadHistoryRecords();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.DeleteHistory();
		}

		private async void DeleteHistory()
		{
			await App.Context.DeleteHistory();
		}

		private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			GameRecord record = ((FrameworkElement)sender).DataContext as GameRecord;
			if (record != null)
			{
				RecordsPage.CurrentRecord = record;
				this.Frame.Navigate(typeof(GameRecordViewerPage));
			}
		}
	}
}
