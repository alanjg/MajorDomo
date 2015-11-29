using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dominion;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DominionJupiter.Common;
using Windows.Graphics.Display;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DominionJupiter
{
	public class SizingHelper : NotifyingObject
	{
		private FrameworkElement source;
		public SizingHelper()
		{
		}

		public void SetSource(FrameworkElement source)
		{
			this.source = source;
			this.source.SizeChanged += source_SizeChanged;
			this.UpdateSize(source.ActualWidth, source.ActualHeight);
		}


		private void source_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateSize(e.NewSize.Width, e.NewSize.Height);
		}

		private void UpdateSize(double width, double height)
		{
			this.PageWidth = width;
			this.PageHeight = height;
			this.CardHeight = Math.Max(this.PageHeight / 6 - 32, 0);
			this.CardWidth = Math.Max(this.PageWidth / 10 - 8, 0);
			this.CardRowHeight = this.CardHeight + 8;
		}

		private double pageWidth;
		public double PageWidth
		{
			get { return this.pageWidth; }
			set { this.pageWidth = value; this.OnPropertyChanged("PageWidth"); }
		}

		private double pageHeight;
		public double PageHeight
		{
			get { return this.pageHeight; }
			set { this.pageHeight = value; this.OnPropertyChanged("PageHeight"); }
		}

		// Width=130, Height=98 are the right values for Surface, e.g. 1300x768
		private double cardWidth;
		public double CardWidth
		{
			get { return this.cardWidth; }
			set { this.cardWidth = value; this.OnPropertyChanged("CardWidth"); }
		}

		private double cardHeight;
		public double CardHeight
		{
			get { return this.cardHeight; }
			set { this.cardHeight = value; this.OnPropertyChanged("CardHeight"); }
		}

		private double cardRowHeight;
		public double CardRowHeight
		{
			get { return this.cardRowHeight; }
			set { this.cardRowHeight = value; this.OnPropertyChanged("CardRowHeight"); }
		}
	}

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GamePage : Page
	{
		private NavigationHelper navigationHelper;
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		private GamePageModel gamePageModel;

		public SizingHelper SizingHelper { get; private set; }
		public GamePage()
		{			
			this.navigationHelper = new NavigationHelper(this);
			this.InitializeComponent();
			this.SizingHelper = (SizingHelper)this.Resources["SizingHelper"];
			this.SizingHelper.SetSource(this);
		}
		
		private void HookModel(GamePageModel model)
		{
			this.gamePageModel = model;
			this.DataContext = this.gamePageModel;
			this.gamePageModel.GameViewModel.TextLog.PropertyChanged += TextLog_PropertyChanged;
			this.gamePageModel.GameException += gamePageModel_GameException;
			if (model.GameViewModel.GameModel.ExtraPiles.Count == 0 && this.gamePageModel.GameViewModel.GameModel.Has10KingdomPiles)
			{
				Grid.SetRow(this.CardScrollViewer, 2);
				Grid.SetRowSpan(this.CardScrollViewer, 4);
			}
			else
			{
				Grid.SetRow(this.CardScrollViewer, 3);
				Grid.SetRowSpan(this.CardScrollViewer, 3);
			}
		}

		void gamePageModel_GameException(Exception e)
		{
			this.LogScrollViewer.Content = new TextBlock() { Text = e.StackTrace };
			this.StatusText.Text = e.Message;
		}

		private void TextLog_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Text")
			{
				this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, new DispatchedHandler(() => {
					//this.LogScrollViewer.ChangeView(0, this.LogScrollViewer.ScrollableHeight, null);
					this.LogScrollViewer.ScrollToVerticalOffset(this.LogScrollViewer.ScrollableHeight);
				}));
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}
		
		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.  The Parameter
		/// property is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (this.gamePageModel == null && App.Context.CurrentGame != null)
			{
				this.HookModel(App.Context.CurrentGame.GamePageModel);
			}
			this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
			{
				this.CardScrollViewer.ChangeView(0, this.CardScrollViewer.ScrollableHeight, null);
			}));
			navigationHelper.OnNavigatedTo(e);
		}

		private void Card_Tap(object sender, TappedRoutedEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			CardViewModel card = (CardViewModel)feSender.DataContext;
			this.gamePageModel.InvokeCard(card);
		}

		private void Pile_Tap(object sender, TappedRoutedEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			PileViewModel pile = (PileViewModel)feSender.DataContext;
			this.gamePageModel.InvokePile(pile);
		}

		private void Effect_Tap(object sender, TappedRoutedEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			EffectViewModel effect = (EffectViewModel)feSender.DataContext;
			this.gamePageModel.InvokeEffect(effect);
		}

		private void BuyPhase_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.EnterBuyPhase();
		}

		private void PlayAll_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.PlayAll();
		}

		private void PlayCoins_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.PlayCoinTokens();
		}

		private void EndTurn_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.EndTurn();
		}

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.MakeChoice();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			App.Context.CurrentGame.ExitGame();
			App.Context.CurrentGame = null;			
		}

		private void AppBarButton_Click(object sender, RoutedEventArgs e)
		{
			this.NavigationHelper.GoBack();
		}

		private void Pile_Holding(object sender, HoldingRoutedEventArgs e)
		{
			this.ShowPileDetails(((FrameworkElement)sender).DataContext as PileViewModel);
		}

		private void Card_Holding(object sender, HoldingRoutedEventArgs e)
		{
			this.ShowCardDetails(((FrameworkElement)sender).DataContext as CardViewModel);
		}

		private void CardDetailsClick(object sender, RoutedEventArgs e)
		{
			this.CardDetailsPopup.Visibility = Visibility.Collapsed;
		}

		private void Pile_RightTap(object sender, RightTappedRoutedEventArgs e)
		{
			this.ShowPileDetails(((FrameworkElement)sender).DataContext as PileViewModel);
			e.Handled = true;
		}

		private void ShowPileDetails(PileViewModel pile)
		{
			if (pile.TopCard != null)
			{
				this.CardDetailsPopup.DataContext = pile.TopCard.CardInfo;
				this.CardDetailsPopup.Visibility = Visibility.Visible;
			}
		}

		private void ShowCardDetails(CardViewModel card)
		{
			if (card != null)
			{
				this.CardDetailsPopup.DataContext = card.CardInfo;
				this.CardDetailsPopup.Visibility = Visibility.Visible;
			}
		}

		private void Card_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			this.ShowCardDetails(((FrameworkElement)sender).DataContext as CardViewModel);
			e.Handled = true;
		}
	}
}
