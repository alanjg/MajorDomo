using System;
using System.Collections.Generic;
using Dominion;
using Xamarin.Forms;
using System.Collections.Specialized;

namespace DominionXamarinForms
{
	public partial class GamePage : ContentPage
	{
		private GamePageModel gamePageModel;
		private bool initializedStacks = false;
		private UICollectionSynchronizer handSynchronizer;
		private UICollectionSynchronizer playedSynchronizer;
		private UICollectionSynchronizer possessionHandSynchronizer;
		private UICollectionSynchronizer possessionPlayedSynchronizer;
		private UICollectionSynchronizer cardChoiceSynchronizer;
		private UICollectionSynchronizer cleanupSynchronizer;
		private UICollectionSynchronizer possessionCleanupSynchronizer;
		private UICollectionSynchronizer islandSynchronizer;
		private UICollectionSynchronizer nativeVillageSynchronizer;
		private UICollectionSynchronizer possessionNativeVillageSynchronizer;
		private UICollectionSynchronizer trashSynchronizer;

        private const double ButtonWidth = 70.0;
		public GamePage ()
		{
			App.CurrentGame = new LocalGame ();
			Kingdom kingdom = new Kingdom (null, null, GameSets.Any, 2);
			App.CurrentGame.GamePageModel.Kingdom = kingdom;
			App.CurrentGame.PlayGame ();

			this.gamePageModel = App.CurrentGame.GamePageModel;
			this.BindingContext = this.gamePageModel;
			InitializeComponent ();
			App.CurrentGame.GamePageModel.GameViewModel.GameInitialized += (object sender, EventArgs e) => Device.BeginInvokeOnMainThread(() => this.OnGameStarted());

			App.CurrentGame.GamePageModel.GameViewModel.TextLog.Turns.CollectionChanged += (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => {
				if(e.NewItems != null)
				{
					foreach(LogTurn turn in e.NewItems)
					{
						turn.Lines.CollectionChanged += (object sender2, NotifyCollectionChangedEventArgs e2) => {
							if(e2.NewItems != null)
							{
								foreach(string line in e2.NewItems)
								{
									Label turnLabel = new Label() { Text = line };
									this.TextLog.Children.Add(turnLabel);
									this.LogScrollViewer.ScrollToAsync(turnLabel, ScrollToPosition.MakeVisible, true);
								}
							}
						};
						foreach(string line in turn.Lines)
						{
							Label turnLabel = new Label() { Text = line };
							this.TextLog.Children.Add(turnLabel);
							this.LogScrollViewer.ScrollToAsync(turnLabel, ScrollToPosition.MakeVisible, true);
						}
					}
				}
			};

		}

		private void OnGameStarted()
		{
			if (!this.initializedStacks) {
				this.initializedStacks = true;
				foreach (PileViewModel pile in App.CurrentGame.GamePageModel.GameViewModel.BasicTreasurePiles) {
					Button button = new Button () { Text = pile.TopCard.Name, WidthRequest = ButtonWidth };
					button.Clicked += (object sender, EventArgs e) => this.gamePageModel.InvokePile(pile);
					this.Treasure.Children.Add (button);
				}
				foreach (PileViewModel pile in App.CurrentGame.GamePageModel.GameViewModel.BasicVictoryPiles) {
					Button button = new Button () { Text = pile.TopCard.Name, WidthRequest = ButtonWidth };
					button.Clicked += (object sender, EventArgs e) => this.gamePageModel.InvokePile(pile);
					this.Victory.Children.Add (button);
				}
				foreach (PileViewModel pile in App.CurrentGame.GamePageModel.GameViewModel.KingdomPiles1of3) {
					Button button = new Button () { Text = pile.TopCard.Name, WidthRequest = ButtonWidth };
					button.Clicked += (object sender, EventArgs e) => this.gamePageModel.InvokePile(pile);
					this.Kingdom1.Children.Add (button);
				}
				foreach (PileViewModel pile in App.CurrentGame.GamePageModel.GameViewModel.KingdomPiles2of3) {
					Button button = new Button () { Text = pile.TopCard.Name, WidthRequest = ButtonWidth };
					button.Clicked += (object sender, EventArgs e) => this.gamePageModel.InvokePile(pile);
					this.Kingdom2.Children.Add (button);
				}
				foreach (PileViewModel pile in App.CurrentGame.GamePageModel.GameViewModel.KingdomPiles3of3) {
					Button button = new Button () { Text = pile.TopCard.Name, WidthRequest = ButtonWidth };
					button.Clicked += (object sender, EventArgs e) => this.gamePageModel.InvokePile(pile);
					this.Kingdom3.Children.Add (button);
				}
                this.CardGrid.ColumnDefinitions[3].Width = 0;

                this.handSynchronizer = new UICollectionSynchronizer (this.gamePageModel.PlayerViewModel.Hand, this.Hand, this.gamePageModel);
				this.playedSynchronizer = new UICollectionSynchronizer (this.gamePageModel.PlayerViewModel.Played, this.Played, this.gamePageModel);
				this.cardChoiceSynchronizer = new UICollectionSynchronizer (this.gamePageModel.CardChoice, this.CardChoice, this.gamePageModel);
				this.possessionHandSynchronizer = new UICollectionSynchronizer (this.gamePageModel.PossessedPlayerViewModel.Hand, this.PossHand, this.gamePageModel);
				this.possessionPlayedSynchronizer = new UICollectionSynchronizer (this.gamePageModel.PossessedPlayerViewModel.Played, this.PossPlay, this.gamePageModel);
				this.cleanupSynchronizer = new UICollectionSynchronizer (this.gamePageModel.PlayerViewModel.Cleanup, this.Cleanup, this.gamePageModel);
				this.islandSynchronizer = new UICollectionSynchronizer (this.gamePageModel.PlayerViewModel.Island, this.Island, this.gamePageModel);
				this.nativeVillageSynchronizer = new UICollectionSynchronizer (this.gamePageModel.PlayerViewModel.NativeVillage, this.NativeVillage, this.gamePageModel);
				this.possessionNativeVillageSynchronizer = new UICollectionSynchronizer (this.gamePageModel.PossessedPlayerViewModel.NativeVillage, this.PossNV, this.gamePageModel);
				this.trashSynchronizer = new UICollectionSynchronizer (this.gamePageModel.GameViewModel.Trash, this.Trash, this.gamePageModel);

				this.gamePageModel.EffectChoice.CollectionChanged += (object sender, NotifyCollectionChangedEventArgs e) => {
					this.EffectChoice.Children.Clear();
					foreach(EffectViewModel effect in this.gamePageModel.EffectChoice)
					{
						Button button = new Button () { Text = effect.Choice };
						button.Clicked += (object sender2, EventArgs e2) => this.gamePageModel.InvokeEffect(effect);
						this.EffectChoice.Children.Add (button);
					}
				};
			}
		}

		private void cardTapped(object sender, ItemTappedEventArgs e)
		{
			CardViewModel card = (CardViewModel)e.Item;
			this.gamePageModel.InvokeCard(card);		
		}

		private void pileTapped(object sender, ItemTappedEventArgs e)
		{
			PileViewModel pile = (PileViewModel)e.Item;
			this.gamePageModel.InvokePile(pile);
		}

		private void BuyPhase_Click(object sender, EventArgs e)
		{
			this.gamePageModel.EnterBuyPhase();
		}

		private void PlayAll_Click(object sender, EventArgs e)
		{
			this.gamePageModel.PlayAll();
		}

		private void PlayCoins_Click(object sender, EventArgs e)
		{
			this.gamePageModel.PlayCoinTokens();
		}

		private void EndTurn_Click(object sender, EventArgs e)
		{
			this.gamePageModel.EndTurn();
		}

		private void OK_Click(object sender, EventArgs e)
		{
			this.gamePageModel.MakeChoice();
		}

		private void Exit_Click(object sender, EventArgs e)
		{
		}
	}
}

