using Dominion;
using Dominion.Model.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DominionEngine.AI
{
	public class BuyListCardGainComparer : Comparer<CardModel>
	{
		private BaseAIChooser chooser;
		private GameSegment gameSegment;
		private BuyList buyList;
		public BuyListCardGainComparer(BaseAIChooser chooser, GameSegment gameSegment, BuyList buyList)
		{
			this.chooser = chooser;
			this.gameSegment = gameSegment;
			this.buyList = buyList;
		}

		public override int Compare(CardModel x, CardModel y)
		{
			if (x.Name == y.Name)
			{
				return 0;
			}
			foreach (BuyListItem item in this.buyList.List)
			{
				if (item.Count > 0)
				{
					if (item.Card.Equals(x.GetType()))
					{
						return 1;
					}
					else if (item.Card.Equals(y.GetType()))
					{
						return -1;
					}
				}
			}
			return DiscardCardComparer.Compare(x, y, false, true, this.chooser.Player, this.gameSegment);
		}
	}

	public class PriorityCardPlayComparer : Comparer<CardModel>
	{
		public PriorityCardPlayComparer()
		{
		}

		public override int Compare(CardModel x, CardModel y)
		{
			return x.CardPriority.PlayPriority.CompareTo(y.CardPriority.PlayPriority);
		}
	}

	public class BuyListChooser : BaseAIChooser
	{
		private BuyList buyList;
		public BuyListChooser(GameModel gameModel)
			:base(gameModel)
		{
		}

		public void SetBuyList(BuyList buyList)
		{
			this.buyList = buyList;
		}

		protected override Comparer<CardModel> GetCardGainComparer(GameSegment gameSegment)
		{
			return new BuyListCardGainComparer(this, gameSegment, this.buyList);
		}

		protected override Comparer<CardModel> GetCardPlayComparer(GameSegment gameSegment)
		{
			return new PriorityCardPlayComparer();
		}
	}
	public class BuyListAIStrategy : BaseAIStrategy
	{
		public BuyListAIStrategy(GameModel gameModel)
			: base(gameModel, new BuyListChooser(gameModel))
		{
			((BaseAIChooser)this.Chooser).SetStrategy(this);
			this.BuyList = new BuyList();
			this.OtherPlayerEditedBuyLists = new Dictionary<Player, BuyList>();
		}

		public bool UseSearch { get; set; }
		public double SearchThreshold { get; set; }
		public int SearchNodes { get; set; }
		public bool SearchConfirm { get; set; }
		public bool SearchKeepsHandInfo { get; set; }

		public BuyList BuyList { get; set; }
		public BuyList EditedBuyList { get; private set; }

		public Dictionary<Player, BuyList> OtherPlayerEditedBuyLists { get; private set; }

		public void FinalizeBuyList()
		{
			if (this.EditedBuyList == null)
			{
				this.EditedBuyList = this.BuyList.Clone();
				((BuyListChooser)this.Chooser).SetBuyList(this.EditedBuyList);
				foreach (Player player in this.GameModel.Players)
				{
					if (player != this.Player)
					{
						this.OtherPlayerEditedBuyLists[player] = this.BuyList.Clone();
					}
				}
			}
		}

		public override string Name
		{
			get { return "Buy List AI"; }
		}

		protected GameSegment GetGameSegmentSlow()
		{
			return this.GetGameSegment(slow:true);
		}

		public override GameSegment GetGameSegment()
		{
			return this.GetGameSegment(slow: false);
		}

		private GameSegment GetGameSegment(bool slow)
		{
			int buysToEndGame = this.GameModel.PileMap[typeof(Province)].Count;
			Pile colony;
			if (this.GameModel.PileMap.TryGetValue(typeof(Colony), out colony))
			{
				buysToEndGame = Math.Min(buysToEndGame, colony.Count);
			}

			// check for 3 pile ending
			if (slow)
			{
				Pile[] smallest3 = new Pile[3];
				foreach (Pile p in this.GameModel.SupplyPiles)
				{
					for (int i = 0; i < 3; i++)
					{
						if (smallest3[i] == null || smallest3[i].Count > p.Count)
						{
							Pile toInsert = p;
							int at = i;
							while (at < 3)
							{
								Pile t = smallest3[at];
								smallest3[at] = toInsert;
								toInsert = t;
								at++;
							}
							break;
						}
					}
				}
				buysToEndGame = Math.Min(buysToEndGame, smallest3[0].Count + smallest3[1].Count + smallest3[2].Count);
			}

			if (buysToEndGame >= 8 && this.GameModel.CurrentPlayer.TurnCount < 12)
			{
				return GameSegment.Early;
			}
			else if (buysToEndGame > 3)
			{
				return GameSegment.Middle;
			}
			else
			{
				return GameSegment.Endgame;
			}
		}
		protected override PlayerAction BuyPhase()
		{
			if (this.Player.Bought.Count == 0)
			{
				if (this.Player.CoinTokens > 0)
				{
					// todo - be smarter about spending, e.g. opening buy with baker or ensure buy will improve
					return new PlayerAction() { ActionType = ActionType.PlayCoinTokens };
				}
				if (this.Player.HasBasicTreasures)
				{
					return new PlayerAction() { ActionType = ActionType.PlayBasicTreasures };
				}

				foreach (CardModel card in this.Player.Hand)
				{
					if (card.Is(CardType.Treasure))
					{
						return new PlayerAction() { ActionType = ActionType.PlayCard, Card = card };
					}
				}
			}

			PlayerAction playerAction = null;
			if (this.Player.TurnCount == 1 || this.Player.TurnCount == 2)
			{
				if (this.EditedBuyList.OpeningBuy2 != null)
				{
					if (this.TryBuyCard(this.EditedBuyList.OpeningBuy2, out playerAction)) { this.EditedBuyList.OpeningBuy2 = null;  return playerAction; }
				}

				if (this.EditedBuyList.OpeningBuy1 != null)
				{
					if (this.TryBuyCard(this.EditedBuyList.OpeningBuy1, out playerAction)) { this.EditedBuyList.OpeningBuy1 = null; return playerAction; }
				}
				bool failedToBuy = false;
				if (this.Player.TurnCount == 2)
				{
					// opener failed(noble brigand, etc?), add to normal buy list
					if (this.EditedBuyList.OpeningBuy1 != null)
					{
						this.EditedBuyList.List.Insert(0, new BuyListItem(this.EditedBuyList.OpeningBuy1, 1));
						failedToBuy = true;
					}
					if (this.EditedBuyList.OpeningBuy2 != null)
					{
						this.EditedBuyList.List.Insert(0, new BuyListItem(this.EditedBuyList.OpeningBuy2, 1));
						failedToBuy = true;
					}
				}
				if (!failedToBuy) { return new PlayerAction() { ActionType = ActionType.EndTurn }; }
			}

			int buysToEndGame = this.GameModel.PileMap[typeof(Province)].Count;
			Pile colonyPile;
			if (this.GameModel.PileMap.TryGetValue(typeof(Colony), out colonyPile))
			{
				buysToEndGame = Math.Min(buysToEndGame, colonyPile.Count);				
			}

			if (buysToEndGame <= this.BuyList.ColonyBuyThreshold || buysToEndGame <= this.BuyList.ProvinceBuyThreshold)
			{
				if (this.TryBuyCard(typeof(Colony), out playerAction)) { return playerAction; }
			}

			if (buysToEndGame <= this.BuyList.ProvinceBuyThreshold)
			{
				if (this.TryBuyCard(typeof(Province), out playerAction)) { return playerAction; }
			}

			if (buysToEndGame <= this.BuyList.DuchyBuyThreshold)
			{
				if (this.TryBuyCard(typeof(Duchy), out playerAction)) { return playerAction; }
			}

			if (buysToEndGame <= this.BuyList.EstateBuyThreshold)
			{
				if (this.TryBuyCard(typeof(Estate), out playerAction)) { return playerAction; }
			}

			foreach (BuyListItem item in this.EditedBuyList.List)
			{
				if (item.Count > 0 && this.TryBuyCard(item.Card, out playerAction)) { return playerAction; }
			}
			return new PlayerAction() { ActionType = ActionType.EndTurn };
		}

		public override void OnThisPlayerGainedCard(CardModel card)
		{
			this.FinalizeBuyList();
			base.OnThisPlayerGainedCard(card);
			foreach (BuyListItem item in this.EditedBuyList.List)
			{
				if ((item.Card.Equals(card.GetType()) || item.Card.Equals(typeof(Knights)) && card is Knights) && item.Count > 0)
				{
					item.Count--;
					break;
				}
			}
		}

		public override void OnOtherPlayerGainedCard(Player player, CardModel card)
		{
			this.FinalizeBuyList();
			base.OnOtherPlayerGainedCard(player, card);
			List<BuyListItem> list = this.OtherPlayerEditedBuyLists[player].List;
			foreach (BuyListItem item in list)
			{
				if ((item.Card.Equals(card.GetType()) || item.Card.Equals(typeof(Knights)) && card is Knights) && item.Count > 0)
				{
					item.Count--;
					break;
				}
			}
		}

		public override PlayerAction GetNextAction()
		{
			this.FinalizeBuyList();
			PlayerAction defaultAction = base.GetNextAction();
			if (this.UseSearch)
			{
				bool shouldSearchAction = defaultAction.ActionType != ActionType.PlayBasicTreasures && defaultAction.ActionType != ActionType.PlayCoinTokens;
				bool shouldSearchGamePhase = this.GetGameSegmentSlow() == GameSegment.Endgame;

				if (shouldSearchGamePhase && shouldSearchAction)
				{
					MonteCarloTreeSearch search = new MonteCarloTreeSearch(this.GameModel, this.EditedBuyList, this.SearchKeepsHandInfo, this.SearchNodes);

					PlayerAction searchedAction;

					search.DoMCTS();
					DominionTreeNode root = search.Root;
					DominionTreeNode bestChild = root.BestChild;
					searchedAction = bestChild.Action;
					
					if (!searchedAction.Equals(defaultAction))
					{
						bool useChoice = true;
						foreach (DominionTreeNode child in root.Children)
						{
							if (child.Action.Equals(defaultAction))
							{
								double choiceWin = bestChild.TotalValue / bestChild.VisitCount;
								double actionWin = child.TotalValue / child.VisitCount;
								double diff = Math.Abs(choiceWin - actionWin);
								if (diff < this.SearchThreshold)
								{
									useChoice = false;									
								}
								else if (child.GameState.GameOver && child.GameState.Result.Winners.Count == 1 && child.GameState.Result.Winners[0] == child.GameState.Players[child.PlayerIndex])
								{
									useChoice = false;
								}
								break;
								// Other options looking at visit count, with and without a threshold.  Doesn't work as well as current method.
								/*
								double diff = bestChild.VisitCount - child.VisitCount;
								diff /= root.VisitCount;
								if(diff < this.SearchThreshold)
								{
									useChoice = false;
									break;
								}
								 * */
								/*
								if (bestChild.VisitCount < child.VisitCount)
								{
									useChoice = false;
									break;
								}
								 * */
							}
						}
						if (!useChoice && (!this.SearchConfirm || root.Children.Count <= 2))
						{
							return defaultAction;
						}
						else
						{
							if (root.Children.Count > 2 && this.SearchConfirm)
							{
								MonteCarloTreeSearch refinedSearch = new MonteCarloTreeSearch(this.GameModel, this.EditedBuyList, false, this.SearchNodes);
								refinedSearch.Root.Expand();
								for (int i = refinedSearch.Root.Children.Count - 1; i >= 0; i--)
								{
									if (!(refinedSearch.Root.Children[i].Action.Equals(defaultAction) || refinedSearch.Root.Children[i].Action.Equals(searchedAction)))
									{
										refinedSearch.Root.Children.RemoveAt(i);
									}
								}
								refinedSearch.DoMCTS();
								DominionTreeNode defaultActionChild = null, searchedActionChild = null;
								foreach (DominionTreeNode child in refinedSearch.Root.Children)
								{
									if (child.Action.Equals(defaultAction))
									{
										defaultActionChild = child;
									}
									else if (child.Action.Equals(searchedAction))
									{
										searchedActionChild = child;
									}
									else
									{
										Debug.Assert(false);
									}
								}
								double defaultWinRate = defaultActionChild.TotalValue / defaultActionChild.VisitCount;
								double searchedWinRate = searchedActionChild.TotalValue / searchedActionChild.VisitCount;
								double diff = Math.Abs(defaultWinRate - searchedWinRate);
								if (diff >= this.SearchThreshold)
								{
									return searchedAction;
								}
								else
								{
									return defaultAction;
								}
							}
							else
							{
								return searchedAction;
							}
						}
					}
					return defaultAction;
				}
			}

			return defaultAction;
		}
	}
}