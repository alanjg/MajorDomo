using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dominion;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DominionJupiter.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DominionJupiter
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CardSetChooserPage : Page
	{
		private NavigationHelper navigationHelper;
		
		public CardSetChooserPage()
		{
			this.InitializeComponent();
			this.DataContext = App.Context.CardSetsModel;
			this.navigationHelper = new NavigationHelper(this);
		}

		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		void ItemView_ItemClick(object sender, ItemClickEventArgs e)
		{
			// Navigate to the appropriate destination page, configuring the new page
			// by passing required information as a navigation parameter
			var itemId = ((CardSetViewModel)e.ClickedItem).CardSetName;
			this.Frame.Navigate(typeof(CardSetChooserItemDetailPage), itemId);
		}

		void Header_Click(object sender, RoutedEventArgs e)
		{
			// Determine what group the Button instance represents
			var group = (sender as FrameworkElement).DataContext;

			// Navigate to the appropriate destination page, configuring the new page
			// by passing required information as a navigation parameter
			this.Frame.Navigate(typeof(CardSetChooserGroupDetailPage), ((CardSetGroup)group).GameSet.SetName);
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}
	}
}
