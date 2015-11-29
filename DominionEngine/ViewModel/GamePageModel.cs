using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Dominion;
using Dominion.Model.Actions;
using Dominion.Network;
using DominionEngine.AI;

namespace Dominion
{
	public delegate void GameExceptionHandler(Exception e);

	public abstract class GamePageModel : NotifyingObject
	{
		public event GameExceptionHandler GameException;

		bool singleChoiceMode = false;
		bool zeroOrSingleChoiceMode = false;
		private GameModel gameModel;
		private GameViewModel gameViewModel;
		private PlayerChoiceParameters choiceParameters;
		private HashSet<CardViewModel> selectedCards;
		private HashSet<EffectViewModel> selectedEffects;
		private HashSet<CardViewModel> selectableCards;

		public Kingdom Kingdom { get; set; }

		private string statusText;
		public string StatusText
		{
			get { return this.statusText; }
			set { this.statusText = value; this.OnPropertyChanged("StatusText"); }
		}


		public bool ShowOkButton
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerChoice && (this.choiceParameters.MinChoices != 1 || this.choiceParameters.MaxChoices != 1);
			}
		}

		public bool ShowBuyPhaseButton
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerAction && this.GameViewModel.CurrentPhase == GamePhase.Action;
			}
		}

		public bool ShowPlayAllButton
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerAction && this.GameViewModel.GameModel.CurrentPlayer.Hand.Any(card => card.Is(CardType.Treasure) && !card.AffectsTreasurePlayOrder);
			}
		}

		public bool ShowPlayCoinButton
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerAction && this.GameViewModel.GameModel.CurrentPhase == GamePhase.Buy && this.GameViewModel.GameModel.CurrentPlayer.CoinTokens > 0;
			}
		}

		public bool ShowEndTurnButton
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerAction && !this.GameViewModel.GameOver;
			}
		}

		public bool ShowExitButton
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.GameOver;
			}
		}

		public bool ShowEffectChoice
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerChoice && this.choiceParameters.SourceType == ChoiceSourceType.Effect;
			}
		}

		public bool ShowCardChoice
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerChoice && this.choiceParameters.SourceType == ChoiceSourceType.Card && (this.choiceParameters.Source & Dominion.Model.Chooser.ChoiceSource.None) != 0;
			}
		}

		public bool ShowIsland
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.GameHasIsland;
			}
		}

		public bool ShowNativeVillage
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.GameHasNativeVillage;
			}
		}

		public bool ShowPossessedPlayerNativeVillage
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.GameHasNativeVillage && this.ShowPossessedPlayerHand;
			}
		}

		public bool ShowVP
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.GameHasVPChips;
			}
		}

		public bool ShowCoinTokens
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.GameHasCoinTokens;
			}
		}

		public bool ShowPossessedPlayerCleanup
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.CurrentPhase == GamePhase.CleanUp && this.ShowPossessedPlayerHand;
			}
		}

		public bool ShowCleanup
		{
			get
			{
				return this.GameViewModel != null && this.GameViewModel.CurrentPhase == GamePhase.CleanUp;
			}
		}

		public bool ShowPossessedPlayerHand
		{
			get
			{
				return this.gameModel.CurrentPlayer != null && this.gameModel.CurrentPlayer.IsPossessionTurn && this.gameModel.CurrentPlayer == this.PossessedPlayerViewModel.PlayerModel;
			}
		}

		private void RefreshButtons()
		{
			this.OnPropertyChanged("ShowOkButton");
			this.OnPropertyChanged("ShowBuyPhaseButton");
			this.OnPropertyChanged("ShowPlayAllButton");
			this.OnPropertyChanged("ShowPlayCoinButton");
			this.OnPropertyChanged("ShowEndTurnButton");
			this.OnPropertyChanged("ShowExitButton");
		}

		public ObservableCollection<CardViewModel> CardChoice { get; private set; }
		public ObservableCollection<EffectViewModel> EffectChoice { get; private set; }

		private void ClearChoiceState()
		{
			this.StatusText = string.Empty;
			this.CardChoice.Clear();
			this.EffectChoice.Clear();
			this.selectedCards.Clear();
			foreach(CardViewModel cardViewModel in this.selectableCards)
			{
				cardViewModel.IsSelectable = false;
				cardViewModel.IsSelected = false;
			}
			this.selectableCards.Clear();
			this.selectedEffects.Clear();
			this.OnPropertyChanged("ShowCardChoice");
			this.OnPropertyChanged("ShowEffectChoice");
			this.OnPropertyChanged("ShowCleanup");
		}

		public void PlayGame()
		{
			this.gameModel.InitializeGameState(this.Kingdom);
			foreach (Player player in this.gameModel.Players)
			{
				player.Strategy.OnGameStart(this.Kingdom);
			}
			ViewModelDispatcher.BeginInvoke(new Action(() =>
			{
				this.OnPropertyChanged("ShowIsland");
				this.OnPropertyChanged("ShowNativeVillage");
				this.OnPropertyChanged("ShowPossessedPlayerNativeVillage");
				this.OnPropertyChanged("ShowVP");
			}));

			try
			{
				this.gameModel.PlayGame();
			}
			catch (Exception e)
			{
				ViewModelDispatcher.BeginInvoke(new Action(() =>
				{
					if (this.GameException != null)
					{
						this.GameException(e);
					}
				}));
			}
		}

		public virtual void SetupGame()
		{
			this.gameModel = new GameModel();
			this.gameViewModel = new GameViewModel(this.gameModel);
			this.gameViewModel.RequestPlayerAction += gameViewModel_RequestPlayerAction;
			this.gameViewModel.RequestPlayerChoice += gameViewModel_RequestPlayerChoice;
			this.gameViewModel.TurnEnded += gameViewModel_TurnEnded;
			this.gameViewModel.OnGameOver += gameViewModel_OnGameOver;
			this.selectedCards = new HashSet<CardViewModel>();
			this.selectableCards = new HashSet<CardViewModel>();
			this.selectedEffects = new HashSet<EffectViewModel>();
			this.CardChoice = new ObservableCollection<CardViewModel>();
			this.EffectChoice = new ObservableCollection<EffectViewModel>();
		}

		private void gameViewModel_TurnEnded(object sender, EventArgs e)
		{
			ViewModelDispatcher.BeginInvoke(new Action(() =>
			{
				this.RefreshButtons();
			}));
		}

		private void gameViewModel_OnGameOver(object sender, EventArgs e)
		{
			ViewModelDispatcher.BeginInvoke(new Action(() =>
			{
				this.RefreshButtons();
			}));
		}

		private void gameViewModel_RequestPlayerChoice(GameViewModel sender, PlayerChoiceParameters parameters)
		{
			this.StatusText = parameters.ChoiceText;
			this.singleChoiceMode = parameters.MinChoices == parameters.MaxChoices && parameters.MinChoices == 1;
			this.zeroOrSingleChoiceMode = parameters.MinChoices == 0 && parameters.MaxChoices == 1;
			this.choiceParameters = parameters;
			if (parameters.SourceType == ChoiceSourceType.Effect)
			{
				this.EffectChoice.AddRange(parameters.EffectChoices);
			}
			else if (parameters.SourceType == ChoiceSourceType.Card)
			{
				if (parameters.Order)
				{
					foreach (CardViewModel card in parameters.CardChoices)
					{
						card.Order = -1;
					}
				}
				if ((parameters.Source & Dominion.Model.Chooser.ChoiceSource.None) != 0)
				{
					IEnumerable<CardViewModel> filteredChoices = parameters.CardChoices;
					if ((parameters.Source & Model.Chooser.ChoiceSource.FromHand) != 0)
					{
						filteredChoices = filteredChoices.Where(c => !this.PlayerViewModel.Hand.Any(c2 => c2.CardModel == c.CardModel));
					}
					this.CardChoice.AddRange(filteredChoices);
				}
				if ((parameters.Source & Model.Chooser.ChoiceSource.FromHand) != 0)
				{
					foreach (CardViewModel card in this.PlayerViewModel.Hand)
					{
						if (parameters.CardChoices.Any(c => c.CardModel == card.CardModel))
						{
							card.IsSelectable = true;
							this.selectableCards.Add(card);
						}
					}
				}
				if ((parameters.Source & Model.Chooser.ChoiceSource.InPlay) != 0)
				{
					foreach (CardViewModel card in this.PlayerViewModel.Played)
					{
						if (parameters.CardChoices.Any(c => c.CardModel == card.CardModel))
						{
							card.IsSelectable = true;
							this.selectableCards.Add(card);
						}
					}
				}
				if ((parameters.Source & Model.Chooser.ChoiceSource.FromTrash) != 0)
				{
					foreach (CardViewModel card in this.GameViewModel.Trash)
					{
						if (parameters.CardChoices.Any(c => c.CardModel == card.CardModel))
						{
							card.IsSelectable = true;
							this.selectableCards.Add(card);
						}
					}
				}
			}
			if (parameters.SourceType == ChoiceSourceType.Pile)
			{
				foreach (PileViewModel pile in this.GameViewModel.Piles)
				{
					if (parameters.PileChoices.Any(p => p.PileModel == pile.PileModel))
					{
						pile.IsSelectable = true;
					}
					else
					{
						pile.IsSelectable = false;
					}
				}
			}
			
			this.RefreshButtons();

			this.OnPropertyChanged("ShowCardChoice");
			this.OnPropertyChanged("ShowEffectChoice");
			this.OnPropertyChanged("IsMakeChoiceEnabled");
		}

		private void gameViewModel_RequestPlayerAction(GameViewModel sender)
		{
			this.RefreshButtons();
			this.ClearChoiceState();
			this.OnPropertyChanged("ShowCleanup");
			this.OnPropertyChanged("ShowPossessedPlayerCleanup");
			if (sender.CurrentPhase == GamePhase.CleanUp)
			{
				this.StatusText = "Perform cleanup actions";
			}
			else
			{
				this.StatusText = string.Empty;
			}
			this.OnPropertyChanged("ShowPossessedPlayerHand");
			this.OnPropertyChanged("ShowPossessedPlayerNativeVillage");
		}

		public GameViewModel GameViewModel
		{
			get { return this.gameViewModel; }
		}

		private PlayerViewModel playerViewModel;
		public PlayerViewModel PlayerViewModel
		{
			get { return this.playerViewModel; }
			protected set { this.playerViewModel = value; this.OnPropertyChanged("PlayerViewModel"); }
		}

		private PlayerViewModel possessedPlayerViewModel;
		public PlayerViewModel PossessedPlayerViewModel
		{
			get { return this.possessedPlayerViewModel; }
			protected set { this.possessedPlayerViewModel = value; this.OnPropertyChanged("PossessedPlayerViewModel"); }
		}

		public void InvokeEffect(EffectViewModel effect)
		{
			if (this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerChoice)
			{
				if (this.singleChoiceMode)
				{
					this.ClearChoiceState();
					this.GameViewModel.PerformUIPlayerChoice(null, null, new EffectViewModel[] { effect });
				}
				else
				{
					effect.IsSelected = !effect.IsSelected;
					if (effect.IsSelected)
					{
						this.selectedEffects.Add(effect);
					}
					else
					{
						this.selectedEffects.Remove(effect);
					}
				}
				this.OnPropertyChanged("IsMakeChoiceEnabled");
				this.RefreshButtons();
			}
		}

		public void InvokeCard(CardViewModel card)
		{
			if (this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerChoice)
			{
				if (this.choiceParameters.SourceType == ChoiceSourceType.Card && this.choiceParameters.CardChoices.Any(c => c.CardModel == card.CardModel))
				{
					if (this.singleChoiceMode || this.zeroOrSingleChoiceMode)
					{
						this.ClearChoiceState();
						this.GameViewModel.PerformUIPlayerChoice(new CardViewModel[] { card }, null, null);
					}
					else
					{
						card.IsSelected = !card.IsSelected;
						if (card.IsSelected)
						{
							this.selectedCards.Add(card);
							if (this.choiceParameters.Order)
							{
								int count = 1;
								foreach (CardViewModel c in this.selectedCards)
								{
									c.Order = count++;
								}
							}
						}
						else
						{
							this.selectedCards.Remove(card);
							if (this.choiceParameters.Order)
							{
								card.Order = -1;

								int count = 1;
								foreach (CardViewModel c in this.selectedCards)
								{
									c.Order = count++;
								}
							}
						}
					}
				}
				this.OnPropertyChanged("IsMakeChoiceEnabled");
				this.RefreshButtons();
			}
			else
			{
				PlayerViewModel player = this.gameModel.CurrentPlayer.IsPossessionTurn ? this.PossessedPlayerViewModel : this.PlayerViewModel;
				if ((this.GameViewModel.CurrentPhase == GamePhase.Action && card.Is(CardType.Action) ||
					this.GameViewModel.CurrentPhase == GamePhase.Buy && card.Is(CardType.Treasure)) && player.Hand.Any(c => c.CardModel == card.CardModel))
				{
					this.gameViewModel.PerformUIPlayerAction(ActionType.PlayCard, card, null);
				}
				else if (this.GameViewModel.CurrentPhase == GamePhase.CleanUp && card.CardModel.HasCleanupEffect(this.gameModel) && player.Cleanup.Any(c => c.CardModel == card.CardModel))
				{
					this.gameViewModel.PerformUIPlayerAction(ActionType.CleanupCard, card, null);
					this.RefreshButtons();
				}
			}
		}

		public void InvokePile(PileViewModel pile)
		{
			if (this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerChoice)
			{
				if (this.choiceParameters.SourceType == ChoiceSourceType.Pile && this.choiceParameters.PileChoices.Any(p => p.PileModel == pile.PileModel))
				{
					if (this.singleChoiceMode || this.zeroOrSingleChoiceMode)
					{
						this.ClearChoiceState();
						foreach (PileViewModel p in this.GameViewModel.Piles)
						{
							p.IsSelectable = false;
						}
						this.GameViewModel.PerformUIPlayerChoice(null, new PileViewModel[] { pile }, null);
					}
					else
					{
						Debug.Assert(false);
					}
				}
				this.OnPropertyChanged("IsMakeChoiceEnabled");
				this.RefreshButtons();
			}
			else
			{
				if (this.GameViewModel.CurrentPhase == GamePhase.Buy && pile.CanBuy && this.gameViewModel.GameModel.SupplyPiles.Contains(pile.PileModel))
				{
					this.gameViewModel.PerformUIPlayerAction(ActionType.BuyCard, null, pile);
					this.RefreshButtons();
				}
			}
		}

		public void EnterBuyPhase()
		{
			this.gameViewModel.PerformUIPlayerAction(ActionType.EnterBuyPhase, null, null);
		}

		public void PlayAll()
		{
			this.gameViewModel.PerformUIPlayerAction(ActionType.PlayBasicTreasures, null, null);
		}

		public void PlayCoinTokens()
		{
			this.gameViewModel.PerformUIPlayerAction(ActionType.PlayCoinTokens, null, null);
		}

		public void EndTurn()
		{
			this.gameViewModel.PerformUIPlayerAction(ActionType.EndTurn, null, null);
			this.RefreshButtons();
		}

		public bool IsMakeChoiceEnabled
		{
			get
			{
				if (this.GameViewModel == null || this.GameViewModel.CurrentPlayerState != ClientState.WaitingForPlayerChoice) return false;
				int selectionCount = 0;
				switch (this.choiceParameters.SourceType)
				{
					case ChoiceSourceType.Card:
						selectionCount = this.selectedCards.Count;
						break;
					case ChoiceSourceType.Effect:
						selectionCount = this.choiceParameters.EffectChoices.Count(effect => effect.IsSelected);
						break;
					case ChoiceSourceType.Pile:
						selectionCount = this.choiceParameters.PileChoices.Count(pile => pile.IsSelected);
						break;
				}
				return this.choiceParameters.MinChoices <= selectionCount && selectionCount <= this.choiceParameters.MaxChoices ||
					this.choiceParameters.Order;
			}
		}

		public void MakeChoice()
		{
			if (this.GameViewModel.CurrentPlayerState == ClientState.WaitingForPlayerChoice)
			{
				switch (this.choiceParameters.SourceType)
				{
					case ChoiceSourceType.Card:
						int cardCount = this.choiceParameters.CardChoices.Count(card => this.selectedCards.Any(c => c.CardModel == card.CardModel));
						if (cardCount >= this.choiceParameters.MinChoices && cardCount <= this.choiceParameters.MaxChoices || this.choiceParameters.Order)
						{
							IEnumerable<CardViewModel> cardChoices = this.choiceParameters.CardChoices;
							if (this.choiceParameters.Order)
							{
								int c = cardChoices.Count();
								foreach (CardViewModel choice in cardChoices)
								{
									if (choice.Order == -1)
									{
										choice.Order = c++;
									}
								}
								cardChoices = cardChoices.OrderBy(card => card.Order);
							}
							else
							{
								cardChoices = cardChoices.Where(card => this.selectedCards.Any(c => c.CardModel == card.CardModel));
							}
							IEnumerable<CardViewModel> chosen = cardChoices.ToList();
							this.ClearChoiceState();
							this.gameViewModel.PerformUIPlayerChoice(chosen, null, null);
						}
						break;
					case ChoiceSourceType.Effect:
						int effectCount = this.choiceParameters.EffectChoices.Count(effect => effect.IsSelected);
						if (effectCount >= this.choiceParameters.MinChoices && effectCount <= this.choiceParameters.MaxChoices)
						{
							this.ClearChoiceState();
							this.gameViewModel.PerformUIPlayerChoice(null, null, this.choiceParameters.EffectChoices.Where(effect => effect.IsSelected));
						}
						break;

					case ChoiceSourceType.Pile:
						Debug.Assert(false);
						int pileCount = this.choiceParameters.PileChoices.Count(pile => pile.IsSelected);
						if (pileCount >= this.choiceParameters.MinChoices && pileCount <= this.choiceParameters.MaxChoices)
						{
							this.ClearChoiceState();
							foreach (PileViewModel pile in this.GameViewModel.Piles)
							{
								pile.IsSelectable = false;
							}
							this.gameViewModel.PerformUIPlayerChoice(null, this.choiceParameters.PileChoices.Where(pile => pile.IsSelected), null);
						}
						break;
				}
			}
			this.RefreshButtons();
		}
	}

	public class LocalGamePageModel : GamePageModel
	{
		public int Difficulty { get; set; }
		public override void SetupGame()
		{
			base.SetupGame();
			Player humanPlayer = new Player("Human", new UIPlayerStrategy(this.GameViewModel), this.GameViewModel.GameModel);
			this.PlayerViewModel = new PlayerViewModel(humanPlayer, this.GameViewModel);
			
			BaseAIStrategy strategy;
			if (this.Difficulty == 0) //scout
			{
				strategy = new OpeningBookRandomizedAIStrategy(this.GameViewModel.GameModel);
			}
			else if (this.Difficulty == 1) //advisor
			{
				strategy = new BuyListTrainingAIStrategy(this.GameViewModel.GameModel, 4, 3, 15, 4, false);
			}
			else if (this.Difficulty == 2) //tactician
			{
				BuyListTrainingAIStrategy s = new BuyListTrainingAIStrategy(this.GameViewModel.GameModel, 4, 3, 15, 4, true);
				s.UseSearch = true;
				s.SearchThreshold = 0.05;
				s.SearchNodes = 320;
				s.SearchConfirm = true;
				s.SearchKeepsHandInfo = true;
				strategy = s;
			}
			else if (this.Difficulty == 3) //familiar
			{
				BuyListTrainingAIStrategy s = new BuyListTrainingAIStrategy(this.GameViewModel.GameModel, 6, 4, 20, 6, true);
				s.UseSearch = true;
				s.SearchThreshold = 0.05;
				s.SearchNodes = 200;
				s.SearchConfirm = true;
				s.SearchKeepsHandInfo = true;
				strategy = s;
			}
			else if(this.Difficulty == 4) //golem
			{
				BuyListTrainingAIStrategy s = new BuyListTrainingAIStrategy(this.GameViewModel.GameModel, 8, 4, 32, 8, true);
				s.UseSearch = true;
				s.SearchThreshold = 0.05;
				s.SearchNodes = 400;
				s.SearchConfirm = true;
				s.SearchKeepsHandInfo = true;
				strategy = s;
			}
			else if (this.Difficulty == 5) //witch
			{
				BuyListTrainingAIStrategy s = new BuyListTrainingAIStrategy(this.GameViewModel.GameModel, 12, 5, 48, 12, true);
				s.UseSearch = true;
				s.SearchThreshold = 0.05;
				s.SearchNodes = 3200;
				s.SearchConfirm = true;
				s.SearchKeepsHandInfo = true;
				strategy = s;
			}
			else if (this.Difficulty == 6) //goons
			{
				BuyListTrainingAIStrategy s = new BuyListTrainingAIStrategy(this.GameViewModel.GameModel, 16, 5, 64, 12, true);
				s.UseSearch = true;
				s.SearchThreshold = 0.05;
				s.SearchNodes = 6400;
				s.SearchConfirm = true;
				s.SearchKeepsHandInfo = true;
				strategy = s;
			}
			else
			{
				// shouldn't get here for now...
				strategy = new BuyListTrainingAIStrategy(this.GameViewModel.GameModel, 5, 3, 20, 5, false);
			}
			Player aiPlayer = new Player("Computer", strategy, this.GameViewModel.GameModel);
			this.PossessedPlayerViewModel = new PlayerViewModel(aiPlayer, this.GameViewModel);
			this.GameViewModel.GameModel.Players.Add(humanPlayer);
			this.GameViewModel.GameModel.Players.Add(aiPlayer);
		}
	}

	public class RemoteGamePageModel : GamePageModel
	{
		public void OnSetupComplete(ServerConnection connection)
		{
			ViewModelDispatcher.BeginInvoke(new Action(() =>
			{
				this.PlayerViewModel = connection.Player;
				this.PossessedPlayerViewModel = connection.PossessedPlayer;
			}));
		}

		public string username { get; set; }
	}
}