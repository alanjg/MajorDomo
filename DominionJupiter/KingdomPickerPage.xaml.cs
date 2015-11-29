using Dominion;
using DominionJupiter.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	public sealed partial class KingdomPickerPage : Page
	{
		private NavigationHelper navigationHelper;
		public CardCollectionModel Model { get; private set; }

		public SizingHelper SizingHelper { get; private set; }
		public KingdomPickerPage()
		{
			this.InitializeComponent();
			this.navigationHelper = new NavigationHelper(this);

			this.SizingHelper = (SizingHelper)this.Resources["SizingHelper"];
			this.SizingHelper.SetSource(this);
		}

		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.Model = App.Context.CardCollectionModel;			
			this.DataContext = this.Model;
			navigationHelper.OnNavigatedTo(e);
			this.GoButton.IsEnabled = this.Model.SelectedCards.Count == 10;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
		{
			CardViewModel card = ((CardViewModel)e.ClickedItem);
			if (!this.Model.SelectedCards.Contains(card) && this.Model.SelectedCards.Count < 10)
			{
				this.Model.SelectedCards.Add(card);
			}

			this.GoButton.IsEnabled = this.Model.SelectedCards.Count == 10;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Kingdom kingdom = new Kingdom(this.Model.SelectedCards.Select(c => c.CardModel).ToList(), App.Context.ProhibitedCards.ProhibitedCards, this.Model.BaneCard != null ? this.Model.BaneCard.CardModel : null, App.Context.SupportedSets.AllowedSets, 2);
			App.StartLocalGame(kingdom);
		}

		private void Button_ClickFill(object sender, RoutedEventArgs e)
		{
			CardCollection collection = this.Model.CardCollections.First(c => c.Name == "All");
			while (this.Model.SelectedCards.Count < 10)
			{
				CardViewModel card = collection.Cards[Randomizer.Next(collection.Cards.Count)];
				if (!this.Model.SelectedCards.Any(c => c.Name == card.Name) && (this.Model.BaneCard == null || this.Model.BaneCard.Name != card.Name))
				{
					this.Model.SelectedCards.Add(card);
				}
			}
			this.GoButton.IsEnabled = this.Model.SelectedCards.Count == 10;
		}


		private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			FrameworkElement fe = (FrameworkElement)sender;
			CardViewModel card = (CardViewModel)fe.DataContext;
			if (this.Model.SelectedCards.Contains(card))
			{
				this.Model.SelectedCards.Remove(card);
			}
			this.GoButton.IsEnabled = this.Model.SelectedCards.Count == 10;
		}
	}
}
