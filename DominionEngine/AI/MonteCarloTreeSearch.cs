//#define RECORD_PLAYOUTS
#define SINGLE_LEVEL
using Dominion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace DominionEngine.AI
{
	[DebuggerDisplay("{Action.ToString()} {TotalValue.ToString(\"N2\")} / {VisitCount} {(TotalValue/VisitCount).ToString(\"P\")}")]
	public class DominionTreeNode
	{
		private static Random r = new Random();
		private const double epsilon = 1e-6;
		public int VisitCount { get; private set; }
		public double TotalValue { get; private set; }

		public IList<DominionTreeNode> Children { get; private set; }

#if RECORD_PLAYOUTS
		public List<GameModel> Playouts { get; private set; }
#endif
		public DominionTreeNode BestChild 
		{
			get
			{
				int bi = 0;
				for (int i = 1; i < this.Children.Count; i++)
				{
					if (this.Children[i].VisitCount > this.Children[bi].VisitCount)
					{
						bi = i;
					}
				}
				return this.Children[bi];
			}
		}

		public GameModel GameState { get; private set; }
		public BuyList BuyList { get; private set; }
		public int PlayerIndex { get; private set; }
		public PlayerAction Action { get; private set; }
		public bool KeepHandInfo { get; private set; }
		private static object treeNodeLock = new object();

		public DominionTreeNode(GameModel state, BuyList buyList, int playerIndex, PlayerAction action, bool keepHandInfo)
		{
			this.GameState = state;
			this.BuyList = buyList;
			this.PlayerIndex = playerIndex;
			this.Action = action;
			this.KeepHandInfo = keepHandInfo;
#if RECORD_PLAYOUTS
			this.Playouts = new List<GameModel>();
#endif
		}

		public void SelectAction()
		{
			List<DominionTreeNode> visited = new List<DominionTreeNode>();
			DominionTreeNode cur = this;

			lock (DominionTreeNode.treeNodeLock)
			{
				visited.Add(this);
#if SINGLE_LEVEL
				if (cur.IsLeaf())
				{
					cur.Expand();
				}
				cur = cur.Select();
				visited.Add(cur);
#else
				while (!cur.IsLeaf())
				{
					cur = cur.Select();
					visited.Add(cur);
				}

				if (!cur.GameState.GameOver)
				{
					cur.Expand();
					cur = cur.Select();
					visited.Add(cur);
				}
#endif
			}
			double value = cur.RollOut();
			lock (DominionTreeNode.treeNodeLock)
			{
				foreach (DominionTreeNode node in visited)
				{
					node.UpdateStats(value);
				}
			}
		}

		private BaseAIStrategy StrategyFunc(GameModel original, GameModel clone, Player player)
		{
			BuyListAIStrategy s = new BuyListAIStrategy(clone);

			BuyListAIStrategy existing = original.Players[this.PlayerIndex].OriginalStrategy as BuyListAIStrategy;
			int which = clone.Players.IndexOf(player);
			if (which != this.PlayerIndex)
			{
				s.BuyList = existing.OtherPlayerEditedBuyLists[original.Players[which]].Clone();
			}
			else
			{
				s.BuyList = existing.EditedBuyList.Clone();
			}
			s.FinalizeBuyList();
			foreach (Player otherPlayer in clone.Players)
			{
				int otherWhich = clone.Players.IndexOf(otherPlayer);

				if (otherWhich != this.PlayerIndex)
				{
					s.OtherPlayerEditedBuyLists[otherPlayer] = existing.OtherPlayerEditedBuyLists[original.Players[otherWhich]].Clone();
				}
			}
			return s;
		}

		public void Expand()
		{
			this.Children = new List<DominionTreeNode>(); 
			List<PlayerAction> actions = DominionTreeNode.CalculatePlayerActions(this.GameState);
			foreach (PlayerAction action in actions)
			{
				BuyList buyList = this.BuyList.Clone();
				GameModel newModel = this.GameState.CloneAndPlay(action, this.KeepHandInfo, this.StrategyFunc);
				DominionTreeNode child = new DominionTreeNode(newModel, buyList, this.PlayerIndex, action, this.KeepHandInfo);
				this.Children.Add(child);
			}
		}

		public static List<PlayerAction> CalculatePlayerActions(GameModel gameModel)
		{
			List<PlayerAction> actions = new List<PlayerAction>();

			switch (gameModel.CurrentPhase)
			{
				case GamePhase.Action:
					Dictionary<string, CardModel> possibleActionCards = new Dictionary<string, CardModel>();
					actions.Add(new PlayerAction(ActionType.EnterBuyPhase));
					foreach (CardModel card in gameModel.CurrentPlayer.Hand)
					{
						if (card.Is(CardType.Action))
						{
							possibleActionCards[card.Name] = card;
						}
					}
					foreach (CardModel card in possibleActionCards.Values)
					{
						actions.Add(new PlayerAction(ActionType.PlayCard, card));
					}

					break;

				case GamePhase.Buy:
					actions.Add(new PlayerAction(ActionType.EndTurn));
					if (gameModel.CurrentPlayer.CoinTokens > 0)
					{
						actions.Add(new PlayerAction(ActionType.PlayCoinTokens));
					}
					if (gameModel.CurrentPlayer.HasBasicTreasures)
					{
						actions.Add(new PlayerAction(ActionType.PlayBasicTreasures));
					}
					Dictionary<string, CardModel> possibleTreasureCards = new Dictionary<string, CardModel>();
					foreach (CardModel card in gameModel.CurrentPlayer.Hand)
					{
						if (card.Is(CardType.Treasure) && card.AffectsTreasurePlayOrder)
						{
							possibleTreasureCards[card.Name] = card;
						}
					}

					foreach (CardModel card in possibleTreasureCards.Values)
					{
						actions.Add(new PlayerAction(ActionType.PlayCard, card));
					}

					foreach (Pile pile in gameModel.SupplyPiles)
					{
						if (!pile.Contrabanded && pile.Count > 0 && gameModel.CurrentPlayer.Coin >= pile.Cost && (gameModel.CurrentPlayer.Potions > 0 || !pile.CostsPotion) && pile.TopCard.CanBuy(gameModel.CurrentPlayer))
						{
							actions.Add(new PlayerAction(ActionType.BuyCard, pile));
						}
					}

					break;

				case GamePhase.CleanUp:
					actions.Add(new PlayerAction(ActionType.EndTurn));
					foreach (CardModel card in gameModel.CurrentPlayer.Cleanup)
					{
						actions.Add(new PlayerAction(ActionType.CleanupCard, card));
					}

					break;
			}
			return actions;
		}

		private DominionTreeNode Select()
		{
			DominionTreeNode selected = null;
			double? bestValue = null;
			foreach (DominionTreeNode c in this.Children)
			{
				double scoreValue = c.TotalValue / (c.VisitCount + epsilon);
				double exploreValue = 1.4 * Math.Sqrt(Math.Log(this.VisitCount + 1) / (c.VisitCount + epsilon));
				double tiebreakerValue = r.NextDouble() * epsilon; // small random number to break ties randomly in unexpanded nodes
				double uctValue;
				bool maximize = this.GameState.CurrentPlayerIndex == this.PlayerIndex;
#if SINGLE_LEVEL
				//maximize = true;
#endif
				if (!maximize)
				{
					uctValue = scoreValue - exploreValue - tiebreakerValue;
					if (!bestValue.HasValue || uctValue < bestValue.Value)
					{
						selected = c;
						bestValue = uctValue;
					}
				}
				else
				{
					uctValue = scoreValue + exploreValue + tiebreakerValue;
					if (!bestValue.HasValue || uctValue > bestValue.Value)
					{
						selected = c;
						bestValue = uctValue;
					}
				}				
			}
			return selected;
		}

		public bool IsLeaf()
		{
			return this.Children == null;
		}

		public double RollOut()
		{
			if (this.GameState.GameOver)
			{
				return this.GameState.Result.Winners.Contains(this.GameState.Players[this.PlayerIndex]) ? 1.0 : 0.0;
			}
			GameModel clone = this.GameState.Clone(this.KeepHandInfo, this.StrategyFunc);
			using (clone.TextLog.SuppressLogging())
			{
				clone.GameLoop(clone.CurrentPlayer.TurnCount + 100);
			}
#if RECORD_PLAYOUTS
			lock (this.Playouts)
			{
				this.Playouts.Add(clone);
			}
#endif
			if (!clone.GameOver || clone.GameOver && clone.Result.Winners.Count > 1)
			{
				return 0.5;
			}
			bool won = clone.GameOver && clone.Result.Winners.Contains(clone.Players[this.PlayerIndex]);
			//return won ? 1.0 : 0.0;

			double playerScore = clone.Players[this.PlayerIndex].Points;
			double bestOpponentScore = -100;
			foreach (Player player in clone.Players)
			{
				if (player != clone.Players[this.PlayerIndex])
				{
					bestOpponentScore = Math.Max(bestOpponentScore, player.Points);
				}
			}
			
			double resultValue = 0.0;
			double diff = playerScore - bestOpponentScore;
			
			if (diff >= 12)
			{
				resultValue = 1.0;
			}
			else if (diff >= 6)
			{
				resultValue = 0.95;
			}
			else if (diff > 0)
			{
				resultValue = 0.9;
			}
			else if (diff == 0 && won)
			{
				resultValue = 0.8;
			}
			else if (diff == 0)
			{
				resultValue = 0.2;
			}
			else if (diff > -6)
			{
				resultValue = 0.1;
			}
			else if (diff > -12)
			{
				resultValue = 0.05;
			}
			else
			{
				resultValue = 0.01;
			}
			if (clone.GameOver)
			{
				double fractionDiff = Math.Max(Math.Min(diff, 100), -100);
				fractionDiff /= 100;
				// fraction diff between -1 and 1
				fractionDiff *= 0.001;
				
				if (won)
				{
					resultValue -= 0.00001 * clone.TurnCount;
					resultValue += fractionDiff;
				}
				else
				{
					resultValue -= 0.01;
					resultValue -= 0.00001 * clone.TurnCount;
					resultValue += 0.001;
					resultValue += fractionDiff;
				}				
			}
			return resultValue;
		}

		public void UpdateStats(double value)
		{
			this.VisitCount++;
			this.TotalValue += value;
		}

		public int Arity()
		{
			return this.Children == null ? 0 : this.Children.Count;
		}
	}

	public class MonteCarloTreeSearch
	{
		private GameModel gameModel;
		private BuyList buyList;
		private bool keepHandInfo;
		private DominionTreeNode root;
		private int nodes;
		public MonteCarloTreeSearch(GameModel gameModel, BuyList buyList, bool keepHandInfo, int nodes)
		{
			this.gameModel = gameModel;
			this.buyList = buyList;
			this.keepHandInfo = keepHandInfo;
			this.root = new DominionTreeNode(gameModel, buyList, gameModel.CurrentPlayerIndex, null, this.keepHandInfo);
			this.nodes = nodes;
		}

		private class TreeSearchWorker
		{
			private DominionTreeNode root;
			private int nodes;
#if JUPITER
			private ManualResetEvent signal;
			private Windows.Foundation.IAsyncAction thread;
#elif PCL

#else
            private Thread thread;
#endif

			public TreeSearchWorker(DominionTreeNode root, int nodes)
			{
				this.root = root;
				this.nodes = nodes;
#if JUPITER
				this.signal = new ManualResetEvent(false);
#elif PCL
                this.workerStart();
#else
                this.thread = new Thread(this.workerStart);
#endif
			}

			public void Go()
			{
#if JUPITER
				this.thread = Windows.System.Threading.ThreadPool.RunAsync(new Windows.System.Threading.WorkItemHandler(this.workerStart));
				this.thread.Completed = new Windows.Foundation.AsyncActionCompletedHandler(completed);
#elif PCL

#else
				this.thread.Start();
#endif
            }

#if JUPITER
			private void completed(Windows.Foundation.IAsyncAction action, Windows.Foundation.AsyncStatus status)
			{
				this.signal.Set();
			}
#endif

            public void Wait()
			{
#if JUPITER
			this.signal.WaitOne();
#elif PCL

#else
				this.thread.Join();
#endif
            }

#if JUPITER
			private void workerStart(Windows.Foundation.IAsyncAction action)
#else
            private void workerStart()
#endif
			{
				for (int i = 0; i < this.nodes; i++)
				{

					this.root.SelectAction();
				}
			}
		}

		public void DoMCTS()
		{
			int nThreads = Environment.ProcessorCount;
			TreeSearchWorker[] threads = new TreeSearchWorker[nThreads];
			for (int i = 0; i < nThreads; i++)
			{
				threads[i] = new TreeSearchWorker(this.root, this.nodes / nThreads);
				threads[i].Go();
			}

			for (int i = 0; i < nThreads; i++)
			{
				threads[i].Wait();
			}
		}

		public DominionTreeNode Root { get { return this.root; } }
	}
}
