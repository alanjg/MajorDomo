using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominion;
using DominionEngine.AI;
using System.Diagnostics;
using Dominion.Model.Actions;
using Dominion.CardSets;
using System.IO;
using System.Threading;

namespace Tests
{
	class Program
	{
		static void Main(string[] args)
		{
			//CardComparerTest();
			//TuneAllDefaultSets();
			//CrashCheck();
			//LogCheck();
			//CardComparerTest();
			//CrashCheck();
			//TuneBuyList();
			//PerfTest();
			//TournamentTest();
			//TournamentTest2();
			
			//CardPrivateFieldCheck();
			//TrainerCompareAITest();
			//MonteCarloTreeSearchTest();
			//MonteCarloTreeSearchTest2();
			//MonteCarloTreeSearchCompareAITest();
			MonteCarloTreeSearchCompareAITestCheckAll();
			//EffectCardTest();
		}

		public static void TrainerCompareAITest()
		{
			DateTime start = DateTime.Now;

			Kingdom kingdom = new Kingdom(new BigMoney().CardCollection, null, null, GameSets.Any, 2, CardUseType.DoNotUse, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);
			/*
			BuyListTrainer trainer = new BuyListTrainer(kingdom, true);
			BuyListTrainer.GamesPerMatchup = 5;
			BuyListTrainer.MaxChallengerCount = 60;
			BuyListTrainer.MaxLeaderCount = 5;
			List<BuyListEntry> best = trainer.Train(20);
			Console.WriteLine("Trained");
			BuyList buyList1 = best.Last().BuyList;
			Console.WriteLine(buyList1.ToString(true));

			List<BuyListEntry> best2 = trainer.TrainThresholds();
			BuyList buyList2 = best2.Last().BuyList;
			Console.WriteLine(buyList2.ToString(true));
			*/

			BuyList buyList1 = new BuyList();
			buyList1.OpeningBuy1 = typeof(Silver);
			buyList1.OpeningBuy2 = typeof(Moneylender);
			buyList1.List.Add(new BuyListItem(typeof(Adventurer), 1));
			buyList1.List.Add(new BuyListItem(typeof(Gold), 2));
			buyList1.List.Add(new BuyListItem(typeof(Province), 99));
			buyList1.List.Add(new BuyListItem(typeof(Adventurer), 2));
			buyList1.List.Add(new BuyListItem(typeof(Gold), 99));
			buyList1.List.Add(new BuyListItem(typeof(Laboratory), 6));
			buyList1.List.Add(new BuyListItem(typeof(Moneylender), 1));
			buyList1.List.Add(new BuyListItem(typeof(Silver), 99));


			BuyList buyList2 = buyList1.Clone();
			buyList2.EstateBuyThreshold = 2;
			buyList2.DuchyBuyThreshold = 5;
			buyList2.ProvinceBuyThreshold = 7;
			buyList2.ColonyBuyThreshold = 0;

			int wins = 0;
			int games = 0;
			for (int p = 0; p < 25600; p++)
			{
				GameModel gameModel = new GameModel();
				BuyListAIStrategy player1 = new BuyListAIStrategy(gameModel);
				player1.BuyList = buyList1.Clone();
			
				BuyListAIStrategy player2 = new BuyListAIStrategy(gameModel);
				player2.BuyList = buyList2.Clone();
			
				
				Player p1 = new Player("player1", player1, gameModel);
				Player p2 = new Player("player2", player2, gameModel);
				gameModel.Players.Add(p1);
				gameModel.Players.Add(p2);
				gameModel.InitializeGameState(kingdom, p % 2);
				gameModel.PlayGame(200);
				//Console.WriteLine(gameModel.TextLog.Text);
				if (gameModel.Result.Winners.Contains(p1) && !gameModel.Result.Winners.Contains(p2))
				{
					wins++;
					games++;
				}
				else if (gameModel.Result.Winners.Contains(p2) && !gameModel.Result.Winners.Contains(p1))
				{
					games++;
				}

			}
			double ratio = wins / (double)games;
			Console.WriteLine(ratio);
		}

		public static void MonteCarloTreeSearchCompareAITest()
		{
			DateTime start = DateTime.Now;
			/*
			CardModel[] cards = new CardModel[]
			{
				new Chapel(),
				new ShantyTown(),
				new Militia(),
				new Moneylender(),
				new City(),
				new Mint(),
				new Goons(),
				new Hoard(),
				new Nobles(),
				new Expand()
			};
			
			BuyList buyList = new BuyList();
			buyList.OpeningBuy1 = typeof(Chapel);
			buyList.OpeningBuy2 = typeof(Silver);
			buyList.List.Add(new BuyListItem(typeof(Goons), 2));
			buyList.List.Add(new BuyListItem(typeof(City), 5));
			buyList.List.Add(new BuyListItem(typeof(Goons), 1));
			buyList.List.Add(new BuyListItem(typeof(Nobles), 4));
			buyList.List.Add(new BuyListItem(typeof(Militia), 1));
			buyList.List.Add(new BuyListItem(typeof(Nobles), 2));
			buyList.List.Add(new BuyListItem(typeof(City), 4));
			buyList.List.Add(new BuyListItem(typeof(Province), 8));
			buyList.List.Add(new BuyListItem(typeof(Nobles), 2));
			buyList.List.Add(new BuyListItem(typeof(Goons), 3));
			buyList.List.Add(new BuyListItem(typeof(Hoard), 5));
			buyList.List.Add(new BuyListItem(typeof(Gold), 99));
			buyList.List.Add(new BuyListItem(typeof(Silver), 99));
			*/
			//Kingdom kingdom = new Kingdom(cards, null, GameSets.Any, 2, CardUseType.DoNotUse, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);


			/*
			IList<CardModel> cards = new List<CardModel>(new BigMoney().CardCollection);
			BuyList buyList = new BuyList();
			buyList.OpeningBuy1 = typeof(Chancellor);
			buyList.OpeningBuy2 = typeof(Silver);
			buyList.List.Add(new BuyListItem(typeof(Gold), 1));
			buyList.List.Add(new BuyListItem(typeof(Laboratory), 2));
			buyList.List.Add(new BuyListItem(typeof(Province), 99));
			buyList.List.Add(new BuyListItem(typeof(Gold), 99));
			buyList.List.Add(new BuyListItem(typeof(Moneylender), 1));
			buyList.List.Add(new BuyListItem(typeof(Laboratory), 8));
			buyList.List.Add(new BuyListItem(typeof(Silver), 99));
			*/
			

			IList<CardModel> cards = new List<CardModel>(new PoolsToolsAndFools().CardCollection);
			BuyList buyList = new BuyList();
			buyList.OpeningBuy1 = typeof(Silver);
			buyList.OpeningBuy2 = typeof(Silver);
			buyList.List.Add(new BuyListItem(typeof(ScryingPool), 2));
			buyList.List.Add(new BuyListItem(typeof(Golem), 8));
			buyList.List.Add(new BuyListItem(typeof(Nobles), 1));
			buyList.List.Add(new BuyListItem(typeof(Province), 99));
			buyList.List.Add(new BuyListItem(typeof(Gold), 3));
			buyList.List.Add(new BuyListItem(typeof(Nobles), 8));
			buyList.List.Add(new BuyListItem(typeof(Gold), 99));
			buyList.List.Add(new BuyListItem(typeof(ScryingPool), 2));
			buyList.List.Add(new BuyListItem(typeof(Silver), 99));
			buyList.List.Add(new BuyListItem(typeof(Potion), 1));
			buyList.List.Add(new BuyListItem(typeof(ScryingPool), 6));
			
			Kingdom kingdom = new Kingdom(cards, null, null, GameSets.Any, 2, CardUseType.DoNotUse, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);
			
			/*
			BuyListTrainer trainer = new BuyListTrainer(kingdom, true);
			BuyListTrainer.GamesPerMatchup = 5;
			BuyListTrainer.MaxChallengerCount = 60;
			BuyListTrainer.MaxLeaderCount = 5;
			List<BuyListEntry> best = trainer.Train(20);
			Console.WriteLine("Trained");
			BuyList buyList2 = best.Last().BuyList;
			Console.WriteLine(buyList2.ToString());
			return;
			*/
			int wins = 0;
			int games = 0;
			for (int p = 0; p < 256; p++)
			{
				GameModel gameModel = new GameModel();
				BuyListAIStrategy player1 = new BuyListAIStrategy(gameModel);
				player1.BuyList = buyList.Clone();
				player1.UseSearch = true;
				player1.SearchThreshold = 0.05;
				player1.SearchNodes = 320;
				player1.SearchConfirm = true;
				player1.SearchKeepsHandInfo = false;
				BuyListAIStrategy player2 = new BuyListAIStrategy(gameModel);
				player2.BuyList = buyList.Clone();
				Player p1 = new Player("player1", player1, gameModel);
				Player p2 = new Player("player2", player2, gameModel);
				gameModel.Players.Add(p1);
				gameModel.Players.Add(p2);
				gameModel.InitializeGameState(kingdom, p % 2);
				gameModel.PlayGame(200);
				//Console.WriteLine(gameModel.TextLog.Text);
				if (gameModel.Result.Winners.Contains(p1) && !gameModel.Result.Winners.Contains(p2))
				{
					Console.WriteLine("won");
					wins++;
					games++;
				}
				else if(gameModel.Result.Winners.Contains(p2) && !gameModel.Result.Winners.Contains(p1))
				{
					Console.WriteLine("lost");
					games++;
				}
				
			}
			double ratio = wins / (double)games;
			Console.WriteLine(ratio);
			DateTime end = DateTime.Now;
			Console.WriteLine((end - start).TotalSeconds + " seconds");
		}

		public static void MonteCarloTreeSearchCompareAITestCheckAll()
		{
			string log = "";
			
			DateTime start = DateTime.Now;
			int overallWins = 0;
			int overallGames = 0;
			CardSetsModel cardSets = new CardSetsModel(GameSets.Any);
			foreach (CardSetGroup group in cardSets.CardSetGroups)
			{
				foreach (CardSetViewModel cardSet in group.CardSets)
				{
					Kingdom kingdom = new Kingdom(cardSet.CardSet.CardCollection, null, cardSet.CardSet.BaneCard, GameSets.Any, 2, CardUseType.DoNotUse, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);

					Console.WriteLine("start train");
					BuyListTrainer trainer = new BuyListTrainer(kingdom, true);
					BuyListTrainer.GamesPerMatchup = 4;
					BuyListTrainer.MaxChallengerCount = 15;
					BuyListTrainer.MaxLeaderCount = 3;
					List<BuyListEntry> best = trainer.Train(8);
					BuyList buyList2 = best.Last().BuyList;
					Console.WriteLine("end train");
					int wins = 0;
					int ties = 0;
					int games = 0;
					while(games < 2560)
					{
						GameModel gameModel = new GameModel();
						BuyListAIStrategy player1 = new BuyListAIStrategy(gameModel);
						player1.BuyList = buyList2.Clone();
						player1.UseSearch = true;
						player1.SearchThreshold = 0.05;
						//player1.SearchThreshold = 1.0;
						player1.SearchNodes = 320;
						player1.SearchKeepsHandInfo = true;
						BuyListAIStrategy player2 = new BuyListAIStrategy(gameModel);
						player2.BuyList = buyList2.Clone();
						Player p1 = new Player("player1", player1, gameModel);
						Player p2 = new Player("player2", player2, gameModel);
						gameModel.Players.Add(p1);
						gameModel.Players.Add(p2);
						gameModel.InitializeGameState(kingdom, games % 2);
						using (gameModel.TextLog.SuppressLogging())
						{
							gameModel.PlayGame();
						}
						if (gameModel.Result.Winners.Count == 2)
						{
							ties++;
							games++;
						}
						else if (gameModel.Result.Winners.Contains(p1) && !gameModel.Result.Winners.Contains(p2))
						{
							wins++;
							games++;
						}
						else if (gameModel.Result.Winners.Contains(p2) && !gameModel.Result.Winners.Contains(p1))
						{
							games++;
						}
					}

					DateTime end2 = DateTime.Now;
					Console.WriteLine((end2 - start).TotalSeconds + " seconds");

					double ratio = wins / (double)(games-ties);
					Console.WriteLine(Log.FormatSortedCards(cardSet.CardSet.CardCollection));
					Console.WriteLine(buyList2.ToString());
					Console.WriteLine(wins + " / " + games);
					Console.WriteLine((wins +ties) + " / " + games);
					Console.WriteLine(ratio);
					Console.WriteLine();

					log += Log.FormatSortedCards(cardSet.CardSet.CardCollection);
					log += buyList2.ToString();
					log += wins + " / " + games;
					log += ratio;
					log += Environment.NewLine;

					overallWins += wins;
					overallGames += games;
				}
			}
			double overallRatio = overallWins / (double)overallGames;
					
			Console.WriteLine(overallWins + " / " + overallGames);
			Console.WriteLine(overallRatio);

			log += overallWins + " / " + overallGames;
			log += overallRatio;

			DateTime end = DateTime.Now;
			Console.WriteLine((end - start).TotalSeconds + " seconds");

			File.WriteAllText("output.txt", log);
		}
		
		public static void MonteCarloTreeSearchTest()
		{
			CardModel[] cards = new CardModel[]
			{
				new Chapel(),
				new ShantyTown(),
				new Militia(),
				new Moneylender(),
				new City(),
				new Mint(),
				new Goons(),
				new Hoard(),
				new Nobles(),
				new Expand()
			};
			GameModel gameModel = new GameModel();
			Kingdom kingdom = new Kingdom(cards, null, null, GameSets.Any, 2, CardUseType.DoNotUse, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);

			BuyList bigMoney = new BuyList();
			bigMoney.List.Add(new BuyListItem(typeof(Province), 8));
			bigMoney.List.Add(new BuyListItem(typeof(Gold), 99));
			bigMoney.List.Add(new BuyListItem(typeof(Silver), 99));

			BuyListAIStrategy player1 = new BuyListAIStrategy(gameModel);
			BuyListAIStrategy player2 = new BuyListAIStrategy(gameModel);
			player1.BuyList = bigMoney.Clone();
			player2.BuyList = bigMoney.Clone();
			Player p1 = new Player("player1", player1, gameModel);
			Player p2 = new Player("player2", player2, gameModel);
			gameModel.Players.Add(p1);
			gameModel.Players.Add(p2);
			gameModel.InitializeGameState(kingdom);
			p1.Hand.Clear();
			p2.Hand.Clear();
			for (int i = 0; i < 5; i++)
			{
				p1.Deck.Draw();
				p2.Deck.Draw();
			}
			for (int i = 0; i < 6; i++)
			{
				gameModel.PileMap[typeof(Province)].DrawCard();
			}
			// skip opening book logic
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.EndTurn));
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.EndTurn));
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.EndTurn));
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.EndTurn));
			p1.Hand.Add(new Gold());
			p1.Hand.Add(new Gold());
			p1.Hand.Add(new Gold());
			p1.Hand.Add(new Gold());
			p1.Hand.Add(new Gold());
			p2.Hand.Add(new Gold());
			p2.Hand.Add(new Gold());
			p2.Hand.Add(new Gold());
			p2.Hand.Add(new Gold());
			p2.Hand.Add(new Estate());
			List<CardModel> d1 = new List<CardModel>();
			for(int i=0;i<5;i++) d1.Add(new Gold());
			List<CardModel> d2 = new List<CardModel>();
			for(int i=0;i<5;i++) d2.Add(new Gold());
			p1.Deck.Populate(d1);
			p2.Deck.Populate(d2);
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.EnterBuyPhase));
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.PlayBasicTreasures));
			MonteCarloTreeSearch search = new MonteCarloTreeSearch(gameModel, bigMoney, true, 320);
			search.DoMCTS();
			DominionTreeNode node = search.Root;
			PlayerAction action = node.BestChild.Action;
			Debug.Assert(action.Pile.Name != "Province");
		}

		public static void MonteCarloTreeSearchTest2()
		{
			CardModel[] cards = new CardModel[]
			{
				new Steward(),
				new DeathCart(),
				new Spy(),
				new Counterfeit(),
				new IllGottenGains(),
				new Laboratory(),
				new RoyalSeal(),
				new Stables(),
				new Venture(),
				new HuntingGrounds()
			};
			GameModel gameModel = new GameModel();
			Kingdom kingdom = new Kingdom(cards, null, null, GameSets.Any, 2, CardUseType.DoNotUse, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);

			BuyList bigMoney = new BuyList();
			bigMoney.List.Add(new BuyListItem(typeof(IllGottenGains), 1));
			bigMoney.List.Add(new BuyListItem(typeof(Province), 8));
			bigMoney.List.Add(new BuyListItem(typeof(Gold), 99));
			bigMoney.List.Add(new BuyListItem(typeof(Silver), 99));
			bigMoney.ProvinceBuyThreshold = 5;
			bigMoney.DuchyBuyThreshold = 5;
			bigMoney.EstateBuyThreshold = 2;
							
			BuyListAIStrategy player1 = new BuyListAIStrategy(gameModel);
			BuyListAIStrategy player2 = new BuyListAIStrategy(gameModel);
			player1.BuyList = bigMoney.Clone();			
			player2.BuyList = bigMoney.Clone();
			
			Player p1 = new Player("player1", player1, gameModel);
			Player p2 = new Player("player2", player2, gameModel);
			gameModel.Players.Add(p1);
			gameModel.Players.Add(p2);
			player1.FinalizeBuyList();
			player2.FinalizeBuyList();
			gameModel.InitializeGameState(kingdom, 0);
			p1.Hand.Clear();
			p2.Hand.Clear();
			for (int i = 0; i < 5; i++)
			{
				p1.Deck.Draw();
				p2.Deck.Draw();
			}
			for (int i = 0; i < 6; i++)
			{
				gameModel.PileMap[typeof(Province)].DrawCard();
				gameModel.PileMap[typeof(IllGottenGains)].DrawCard();
			}
			// skip opening book logic
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.EndTurn));
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.EndTurn));
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.EndTurn));
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.EndTurn));

			//human had 18 points, computer had 21 points
			// computer had 8 coins worth of stuff in hand
			// human had 5 coins worth of stuff in hand
			// 2 provinces left
			//6 duchys left
			// 8 estates left
			// 4 curses & ill gotten gains left
			// computer bought ill gotten gains, then reshuffled.
			p1.Hand.Add(new Copper());
			p1.Hand.Add(new Silver());
			p1.Hand.Add(new Gold());
			p1.Hand.Add(new Silver());
			p1.Hand.Add(new Estate());

			List<CardModel> d1 = new List<CardModel>();
			for (int i = 0; i < 6; i++) d1.Add(new Copper());
			for (int i = 0; i < 6; i++) d1.Add(new Curse());
			d1.Add(new Duchy());
			d1.Add(new Duchy());
			for (int i = 0; i < 3; i++) d1.Add(new Province());
			for (int i = 0; i < 2; i++) d1.Add(new Estate());
			for (int i = 0; i < 5; i++) d1.Add(new Silver());
			d1.Add(new HuntingGrounds());
			d1.Add(new Venture());
			d1.Add(new Venture());
			p1.Discard.AddRange(d1);

			List<CardModel> d2 = new List<CardModel>();
			for (int i = 0; i < 7; i++) d2.Add(new Copper());
			for (int i = 0; i < 5; i++) d2.Add(new IllGottenGains());
			for (int i = 0; i < 2; i++) d2.Add(new Province());
			
			d2.Add(new Silver());
			d2.Add(new Steward());
			d2.Add(new Steward());

			p2.Hand.Add(new Copper());
			p2.Hand.Add(new Copper());
			p2.Hand.Add(new Copper());
			p2.Hand.Add(new IllGottenGains());
			p2.Hand.Add(new Province());
			p2.Deck.Populate(d2);
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.EnterBuyPhase));
			gameModel.HandlePlayerAction(new PlayerAction(ActionType.PlayBasicTreasures));
		
			MonteCarloTreeSearch search = new MonteCarloTreeSearch(gameModel, bigMoney.Clone(), false, 320);
			search.DoMCTS();
			DominionTreeNode node = search.Root;
			DominionTreeNode bestChild = null;
			DominionTreeNode defaultChild = null;
			foreach (DominionTreeNode child in node.Children.OrderByDescending(c => c.TotalValue / c.VisitCount))
			{
				Console.WriteLine(child.Action.ToString() + " " + child.TotalValue + " / " + child.VisitCount + " " + (child.TotalValue / child.VisitCount));
				if (bestChild == null || (child.TotalValue / child.VisitCount) > (bestChild.TotalValue / bestChild.VisitCount))
				{
					bestChild = child;
				}
				if (child.Action.Pile != null && child.Action.Pile.Card is Province)
				{
					defaultChild = child;
				}
			}

			PlayerAction defaultAction = defaultChild.Action;
			PlayerAction searchedAction = bestChild.Action;
			if (!defaultAction.Equals(searchedAction))
			{
				Console.WriteLine("difference!!!!!");

				MonteCarloTreeSearch refinedSearch = new MonteCarloTreeSearch(gameModel, bigMoney.Clone(), true, 310);
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

				foreach (DominionTreeNode child in refinedSearch.Root.Children.OrderByDescending(c => c.TotalValue / c.VisitCount))
				{
					Console.WriteLine(child.Action.ToString() + " " + child.TotalValue + " / " + child.VisitCount + " " + (child.TotalValue / child.VisitCount));
				}
			}
		}
		public static void CardPrivateFieldCheck()
		{
			foreach(Type cardType in typeof(CardModel).Assembly.GetTypes())
			{
				if (typeof(CardModel).IsAssignableFrom(cardType) && !cardType.IsAbstract)
				{
					System.Reflection.FieldInfo[] fields = cardType.GetFields(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
					foreach(System.Reflection.FieldInfo field in fields)
					{
						Console.WriteLine(cardType.Name + " " + field.Name);
					}
					List<CardModel> cards = new List<CardModel>();
					cards.Add((CardModel)Activator.CreateInstance(cardType));
					cards.Add((CardModel)Activator.CreateInstance(cardType));
					Console.WriteLine(Log.FormatSortedCards(cards));
				}
			}
		}
		public static void CardComparerTest()
		{
			GameModel model = new GameModel();
			BaseAIStrategy s = new OpeningBookRandomizedAIStrategy(model);
			DiscardCardComparer comp = new DiscardCardComparer((BaseAIChooser)s.Chooser, GameSegment.Early);
			int result = comp.Compare(new Estate(), new Copper());
			Console.WriteLine(result);
			result = comp.Compare(new Copper(), new Silver());
			Console.WriteLine(result);
			result = comp.Compare(new Spoils(), new Silver());
			Console.WriteLine(result);

			PriorityCardPlayComparer c = new PriorityCardPlayComparer();
			result = c.Compare(new AbandonedMine(), new Adventurer());
			Console.WriteLine(result);
			List<CardModel> sorted = new List<CardModel>();
			sorted.Add(new AbandonedMine());
			sorted.Add(new Militia());
			foreach(CardModel cm in sorted.OrderBy(cc => cc, c).Reverse())
			{
				Console.Write(cm.Name + " ");
			}
			Console.WriteLine();
			
		}

		public static void TournamentTest()
		{
			Randomizer.SetRandomSeed(123456789);
			CardModel[] cards = new CardModel[]
			{
				new Chapel(),
				new ShantyTown(),
				new Militia(),
				new Moneylender(),
				new City(),
				new Mint(),
				new Goons(),
				new Hoard(),
				new Nobles(),
				new Expand()
			};
			cards = new CardModel[]
			{
				new Pawn(),
				new Village(),
				new Swindler(),
				new Tournament(),
				new Watchtower(),
				new City(),
				new Duke(),
				new Festival(),
				new Mint(),
				new TradingPost()
			};
			
			/*
			CardSet cardSet = new TheGoodLife();
			IEnumerable<CardModel> cards = cardSet.CardCollection;
			
			new Contraband(),
			new CountingHouse(),
			new Hoard(),
			new Monument(),
			new Mountebank(),
			new Bureaucrat(),
			new Cellar(),
			new Chancellor(),
			new Gardens(),
			new Village()
			 * */
			//Kingdom kingdom = new Kingdom(cards, null, 2, CardUseType.Use, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);
			Kingdom kingdom = new Kingdom(cards, null, null, GameSets.Any, 2, CardUseType.DoNotUse, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);
			
			GameModel model = new GameModel();
			using (model.TextLog.SuppressLogging())
			{
				List<BuyList> lists = new List<BuyList>();
				BuyList list = new BuyList();

				list.ProvinceBuyThreshold = 5;
				list.DuchyBuyThreshold = 5;
				list.EstateBuyThreshold = 2;
				list.OpeningBuy1 = typeof(Steward);
				list.OpeningBuy2 = typeof(Steward);
				list.List.Add(new BuyListItem(typeof(Platinum), 1));				
				list.List.Add(new BuyListItem(typeof(Colony), 99));
				list.List.Add(new BuyListItem(typeof(Platinum), 99));
				list.List.Add(new BuyListItem(typeof(Mint), 1));				  
				list.List.Add(new BuyListItem(typeof(Province), 99));
				list.List.Add(new BuyListItem(typeof(Gold), 1));    
				list.List.Add(new BuyListItem(typeof(Gold), 99));
				list.List.Add(new BuyListItem(typeof(Silver), 99));
				lists.Add(list);

				list = new BuyList();
				list.ProvinceBuyThreshold = 5;
				list.DuchyBuyThreshold = 5;
				list.EstateBuyThreshold = 2;
				list.OpeningBuy1 = typeof(Steward);
				list.OpeningBuy2 = typeof(Tournament);

				list.List.Add(new BuyListItem(typeof(Platinum), 1));
				list.List.Add(new BuyListItem(typeof(Mint), 1));		
				list.List.Add(new BuyListItem(typeof(Tournament), 3));
				list.List.Add(new BuyListItem(typeof(Colony), 99));
				list.List.Add(new BuyListItem(typeof(Platinum), 99));
				list.List.Add(new BuyListItem(typeof(Gold), 1));
				list.List.Add(new BuyListItem(typeof(Province), 99));
				list.List.Add(new BuyListItem(typeof(Gold), 99));
				list.List.Add(new BuyListItem(typeof(Silver), 99));
				lists.Add(list);				
				
				/*
				list.ProvinceBuyThreshold = 2;
				list.DuchyBuyThreshold = 2;
				list.EstateBuyThreshold = 1;
				list.List.Add(new BuyListItem(typeof(Colony), 99));
				list.List.Add(new BuyListItem(typeof(Platinum), 99));
				list.List.Add(new BuyListItem(typeof(Goons), 1));
				list.List.Add(new BuyListItem(typeof(Nobles), 8));
				list.List.Add(new BuyListItem(typeof(Gold), 99));
				list.List.Add(new BuyListItem(typeof(City), 10));
				list.List.Add(new BuyListItem(typeof(Militia), 1));
				list.List.Add(new BuyListItem(typeof(Silver), 1));
				list.List.Add(new BuyListItem(typeof(Chapel), 1));				
				list.List.Add(new BuyListItem(typeof(ShantyTown), 7));
				list.List.Add(new BuyListItem(typeof(Silver), 99));
				lists.Add(list);
				
				list = new BuyList();
				list.ProvinceBuyThreshold = 3;
				list.DuchyBuyThreshold = 2;
				list.EstateBuyThreshold = 0;
				list.List.Add(new BuyListItem(typeof(Militia), 1));
				list.List.Add(new BuyListItem(typeof(Chapel), 1));
				list.List.Add(new BuyListItem(typeof(Goons), 1));
				list.List.Add(new BuyListItem(typeof(City), 8));
				list.List.Add(new BuyListItem(typeof(Nobles), 8));
				list.List.Add(new BuyListItem(typeof(Colony), 99));
				list.List.Add(new BuyListItem(typeof(Expand), 8));
				
				list.List.Add(new BuyListItem(typeof(Goons), 1));
				list.List.Add(new BuyListItem(typeof(City), 1));
				list.List.Add(new BuyListItem(typeof(Goons), 4));
				list.List.Add(new BuyListItem(typeof(Platinum), 8));
				list.List.Add(new BuyListItem(typeof(City), 1));
				list.List.Add(new BuyListItem(typeof(Goons), 4));
				list.List.Add(new BuyListItem(typeof(Platinum), 99));
				list.List.Add(new BuyListItem(typeof(Province), 99));
				list.List.Add(new BuyListItem(typeof(Gold), 1));
				list.List.Add(new BuyListItem(typeof(Gold), 99));
				list.List.Add(new BuyListItem(typeof(Silver), 99));
				lists.Add(list);
				 * */
				/*
				list = new BuyList();
				list.DuchyBuyThreshold = 5;
				list.EstateBuyThreshold = 3;
				list.List.Add(new BuyListItem(typeof(Platinum), 99));
				list.List.Add(new BuyListItem(typeof(Nobles), 8));
				list.List.Add(new BuyListItem(typeof(Gold), 99));
				list.List.Add(new BuyListItem(typeof(City), 10));
				list.List.Add(new BuyListItem(typeof(Militia), 2));
				list.List.Add(new BuyListItem(typeof(Chapel), 1));
				list.List.Add(new BuyListItem(typeof(ShantyTown), 8));
				list.List.Add(new BuyListItem(typeof(Silver), 99));
				lists.Add(list);

				list = new BuyList();
				list.DuchyBuyThreshold = 5;
				list.EstateBuyThreshold = 3;
				list.List.Add(new BuyListItem(typeof(Platinum), 99));
				list.List.Add(new BuyListItem(typeof(Goons), 1));
				list.List.Add(new BuyListItem(typeof(Nobles), 8));
				list.List.Add(new BuyListItem(typeof(Gold), 99));
				list.List.Add(new BuyListItem(typeof(City), 10));
				list.List.Add(new BuyListItem(typeof(Chapel), 1));
				list.List.Add(new BuyListItem(typeof(Silver), 99));
				lists.Add(list);
				
				list = new BuyList();

				list.ColonyBuyThreshold = 8;
				list.ProvinceBuyThreshold = 2;
				list.DuchyBuyThreshold = 2;
				list.EstateBuyThreshold = 1;
				list.List.Add(new BuyListItem(typeof(Colony), 12));
				list.List.Add(new BuyListItem(typeof(Platinum), 99));
				list.List.Add(new BuyListItem(typeof(Hoard), 1));
				list.List.Add(new BuyListItem(typeof(Nobles), 8));
				list.List.Add(new BuyListItem(typeof(Gold), 99));
				list.List.Add(new BuyListItem(typeof(City), 10));
				list.List.Add(new BuyListItem(typeof(Chapel), 1));
				list.List.Add(new BuyListItem(typeof(Silver), 99));
				lists.Add(list);
				
				list = new BuyList();
				list.ColonyBuyThreshold = 0;
				list.ProvinceBuyThreshold = 0;
				list.DuchyBuyThreshold = 0;
				list.EstateBuyThreshold = 0;
				list.List.Add(new BuyListItem(typeof(Militia), 1));
				list.List.Add(new BuyListItem(typeof(Chapel), 1));
				list.List.Add(new BuyListItem(typeof(Gold), 3));
				list.List.Add(new BuyListItem(typeof(City), 10));
				list.List.Add(new BuyListItem(typeof(Goons), 10));
				list.List.Add(new BuyListItem(typeof(Silver), 4));
				list.List.Add(new BuyListItem(typeof(Hoard), 2));
				list.List.Add(new BuyListItem(typeof(Nobles), 8));
				list.List.Add(new BuyListItem(typeof(Gold), 99));
				lists.Add(list);
				*/

				int[] wins = new int[lists.Count];
				int[] losses = new int[lists.Count];
				int[] games = new int[lists.Count];
				BuyList[] listsArray = new BuyList[wins.Length];
				for (int i = 0; i < lists.Count; i++)
				{
					listsArray[i] = lists[i];
				}

				for (int i = 0; i < 100; i++)
				{
					for (int j = 0; j < lists.Count; j++)
					{
						for (int k = j + 1; k < lists.Count; k++)
						{
							int res = PlayGame(kingdom, listsArray[j], listsArray[k]);
							if (res == -1)
							{
								wins[j]++;
								losses[k]++;
							}
							else if (res == 1)
							{
								losses[j]++;
								wins[k]++;
							}

							res = PlayGame(kingdom, listsArray[k], listsArray[j]);
							if (res == -1)
							{
								wins[k]++;
								losses[j]++;
							}
							else if (res == 1)
							{
								losses[k]++;
								wins[j]++;
							}
							games[j] += 2;
							games[k] += 2;
						}
					}
				}

				for (int i = 0; i < listsArray.Length; i++)
				{
					Console.WriteLine(wins[i].ToString() + "/" + losses[i].ToString() + "/" + games[i].ToString());
					Console.WriteLine(listsArray[i]);
				}
			}
		}

		public static void TournamentTest2()
		{
			Randomizer.SetRandomSeed(123456789);
			CardModel[] cards = new CardModel[]
			{
				new Familiar(),
				new Wharf(),
				new FishingVillage(),
				new Smugglers(),
				new Warehouse(),
				new NomadCamp(),
				new Laboratory(),
				new Rabble(),
				new Nobles(),
				new KingsCourt()
			};
			Kingdom kingdom = new Kingdom(cards, null, null, GameSets.Any, 2, CardUseType.DoNotUse, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);

			GameModel model = new GameModel();
			using (model.TextLog.SuppressLogging())
			{
				List<BuyList> lists = new List<BuyList>();
				BuyList list = new BuyList();

				list.ProvinceBuyThreshold = 5;
				list.DuchyBuyThreshold = 5;
				list.EstateBuyThreshold = 2;
				list.OpeningBuy1 = typeof(FishingVillage);
				list.OpeningBuy2 = typeof(FishingVillage);
				list.List.Add(new BuyListItem(typeof(Wharf), 4));
				list.List.Add(new BuyListItem(typeof(Gold), 1));
				list.List.Add(new BuyListItem(typeof(Province), 99));				
				list.List.Add(new BuyListItem(typeof(Gold), 99));
				list.List.Add(new BuyListItem(typeof(Silver), 99));
				lists.Add(list);

				list = new BuyList();
				list.ProvinceBuyThreshold = 5;
				list.DuchyBuyThreshold = 5;
				list.EstateBuyThreshold = 2;
				list.OpeningBuy1 = typeof(Silver);
				list.OpeningBuy2 = typeof(Potion);
				list.List.Add(new BuyListItem(typeof(Familiar), 3));
				list.List.Add(new BuyListItem(typeof(Gold), 1));
				list.List.Add(new BuyListItem(typeof(Province), 99));
				list.List.Add(new BuyListItem(typeof(Gold), 99));
				list.List.Add(new BuyListItem(typeof(Silver), 99));
				lists.Add(list);

				int[] wins = new int[lists.Count];
				int[] losses = new int[lists.Count];
				int[] games = new int[lists.Count];
				BuyList[] listsArray = new BuyList[wins.Length];
				for (int i = 0; i < lists.Count; i++)
				{
					listsArray[i] = lists[i];
				}

				for (int i = 0; i < 100; i++)
				{
					for (int j = 0; j < lists.Count; j++)
					{
						for (int k = j + 1; k < lists.Count; k++)
						{
							int res = PlayGame(kingdom, listsArray[j], listsArray[k]);
							if (res == -1)
							{
								wins[j]++;
								losses[k]++;
							}
							else if (res == 1)
							{
								losses[j]++;
								wins[k]++;
							}

							res = PlayGame(kingdom, listsArray[k], listsArray[j]);
							if (res == -1)
							{
								wins[k]++;
								losses[j]++;
							}
							else if (res == 1)
							{
								losses[k]++;
								wins[j]++;
							}
							games[j] += 2;
							games[k] += 2;
						}
					}
				}

				for (int i = 0; i < listsArray.Length; i++)
				{
					Console.WriteLine(wins[i].ToString() + "/" + losses[i].ToString() + "/" + games[i].ToString());
					Console.WriteLine(listsArray[i]);
				}
			}
		}

		public static void PerfTest()
		{
			Randomizer.SetRandomSeed(123456789);
			int games = 0;
			DateTime start = DateTime.Now;
			CardSetsModel cardSets = new CardSetsModel(GameSets.Any);
			foreach (CardSetGroup group in cardSets.CardSetGroups)
			{
				foreach(CardSetViewModel cardSet in group.CardSets)
				{
					for (int k = 0; k < 800; k++)
					{
						games++;
						GameModel model = new GameModel();
						using (model.TextLog.SuppressLogging())
						{
							BuyList list = new BuyList();
							list.DuchyBuyThreshold = 5;
							list.EstateBuyThreshold = 3;
							list.List.Add(new BuyListItem(typeof(Gold), 99));
							foreach (CardModel card in cardSet.CardSet.CardCollection)
							{
								list.List.Add(new BuyListItem(card.GetType(), 1));
							}
							list.List.Add(new BuyListItem(typeof(Silver), 99));

							BuyListAIStrategy player1 = new BuyListAIStrategy(model);
							player1.BuyList = list;
							BuyListAIStrategy player2 = new BuyListAIStrategy(model);
							player2.BuyList = list;
							Player p1 = new Player("player1", player1, model);
							Player p2 = new Player("player2", player2, model);
							model.Players.Add(p1);
							model.Players.Add(p2);

							Kingdom kingdom = new Kingdom(cardSet.CardSet.CardCollection, null, GameSets.Any, 2);
							model.InitializeGameState(kingdom);
							model.PlayGame(100);
						}
					}
				}
			}
			TimeSpan elapsed = DateTime.Now - start;
			Console.WriteLine("total time = " + elapsed.TotalSeconds);
			Console.WriteLine("Total games = " + games);
			Console.WriteLine("games / sec = " + (games / elapsed.TotalSeconds));
		}

		public static void CrashCheck()
		{
			foreach (Type type in typeof(CardModel).Assembly.GetTypes())
			{
				if (typeof(CardModel).IsAssignableFrom(type) && !type.IsAbstract)
				{
					CardModel card = (CardModel)Activator.CreateInstance(type);
					if (card is BlankCard || card is Knights) continue;
					if (card.CardPriority == null)
					{
						Console.WriteLine("Empty cardpriority for " + card.Name);
					}
					if (string.IsNullOrEmpty(card.CardInfo.Type))
					{
						Console.WriteLine("empty card info for " + card.Name);
					}
				}
			}

			Thread[] threads = new Thread[Environment.ProcessorCount];
			for(int i=0;i<threads.Length;i++)
			{
				threads[i] = new Thread(new ThreadStart(Program.CrashCheckWorker));
			}

			for (int i = 0; i < threads.Length; i++)
			{
				threads[i].Start();
			}

			for (int i = 0; i < threads.Length; i++)
			{
				threads[i].Join();
			}
		}
		public static void CrashCheckWorker()
		{			
			int max = 1000000;
			for (int i = 0; i < max; i++)
			{
				GameModel model = new GameModel();
				using (model.TextLog.SuppressLogging())
				{
					RandomAIStrategy player1 = new RandomAIStrategy(model);
					//BaseAIStrategy player2 = new OpeningBookRandomizedAIStrategy(model);
					BuyListTrainingAIStrategy player2 = new BuyListTrainingAIStrategy(model, 4, 2, 4, 2, false);
					player2.UseSearch = true;
					player2.SearchNodes = 20;
					player2.SearchThreshold = 0.05;
					player2.SearchKeepsHandInfo = true;
					player2.SearchConfirm = true;
					GameViewModel viewModel = new GameViewModel(model);
					Player p1 = new Player("player1", player1, model);
					Player p2 = new Player("player2", player2, model);
					model.Players.Add(p1);
					model.Players.Add(p2);
					IEnumerable<CardModel> cards = new RandomAllCardSet(GameSets.Any).CardCollection;
					List<CardModel> cardList = cards.ToList();
					if (!cardList.Any(c => c is Prince))
					{
						cardList.RemoveAt(0);
						cardList.Add(new Prince());
					}
					Kingdom kingdom = new Kingdom(cardList, null, null, GameSets.Any, 2);
					model.InitializeGameState(kingdom);
					try
					{
						model.PlayGame(100);
					}
					catch (Exception e)
					{
						Console.WriteLine("error!");
						Console.WriteLine(e.Message);
						Console.WriteLine(e.StackTrace);
						Debugger.Launch();
						Console.WriteLine("error!");
						Console.WriteLine(e.Message);
						Console.WriteLine(e.StackTrace);
					}
					if (i % (max / 100) == 0)
					{
						Console.WriteLine(i / (double)max);
					}
				}
			}
		}

		public static void LogCheck()
		{
			int max = 1000; 
			for (int i = 0; i < max; i++)
			{
				GameModel model = new GameModel();
				
				RandomAIStrategy player1 = new RandomAIStrategy(model);
				BaseAIStrategy player2 = new OpeningBookRandomizedAIStrategy(model);
				GameViewModel viewModel = new GameViewModel(model);
				Player p1 = new Player("player1", player1, model);
				Player p2 = new Player("player2", player2, model);
				model.Players.Add(p1);
				model.Players.Add(p2);

				Kingdom kingdom = new Kingdom(new RandomAllCardSet(GameSets.Any).CardCollection, null, GameSets.Any, 2);
				model.InitializeGameState(kingdom);
				try
				{
					model.PlayGame(250);
					if (model.GameOver)
					{
						GameRecord record = model.Result.ToGameRecord(p1);
						var s = new System.Xml.Serialization.XmlSerializer(typeof(GameRecord));
						StringWriter w = new StringWriter();
						s.Serialize(w, record);
						GameRecord g2 = (GameRecord)s.Deserialize(new StringReader(w.ToString()));
						if (record.Name != g2.Name)
						{
							Console.WriteLine("Error");
						}
						if (record.Won != g2.Won)
						{
							Console.WriteLine("Error");
						}
						
						if (record.Players.Count != g2.Players.Count)
						{
							Console.WriteLine("Error");
						}
						else
						{
							for (int j = 0; j < record.Players.Count; j++)
							{
								if (record.Players[j].Name != g2.Players[j].Name)
								{
									Console.WriteLine("Error");
								}
								if (record.Players[j].Score != g2.Players[j].Score)
								{
									Console.WriteLine("Error");
								}
								if (record.Players[j].Deck != g2.Players[j].Deck)
								{
									Console.WriteLine("Error");
								}
							}
						}
						if (record.Log.Count != g2.Log.Count)
						{
							Console.WriteLine("Error");
						}
						else
						{
							for (int j = 0; j < record.Log.Count; j++)
							{
								if (record.Log[j].Replace(Environment.NewLine, "\n") != g2.Log[j])
								{
									Console.WriteLine("Error");
								}
							}
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("error!");
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
					Debugger.Launch();
				}
				if (i % (max / 100) == 0)
				{
					Console.WriteLine(i / (double)max);
				}
			}
			
		}

		public static void TuneBuyList()
		{
			Randomizer.SetRandomSeed(123456789);
			//IEnumerable<CardModel> cards = new RandomAllCardSet().CardCollection.ToList();
			//IList<CardModel> cards = new AdventuresAbroad().CardCollection;
			/*
			CardModel[] cards = new CardModel[]{
				new Chapel(),
				new Militia(),
				new Goons(),
				new City(),
				new ShantyTown(),
				new Moneylender(),
				new Mint(),
				new Hoard(),
				new Nobles(),
				new Expand()
			};*/
			//IList<CardModel> cards = new AdventuresAbroad().CardCollection;
			CardModel[] cards = new CardModel[]
			{
				new Pawn(),
				new FishingVillage(),
				new Steward(),
				new Tournament(),
				new Watchtower(),
				new City(),
				new Duke(),
				new Festival(),
				new Mint(),
				new Prince()
			};
			Kingdom kingdom = new Kingdom(cards, null, null, GameSets.Any, 2, CardUseType.Use, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);

			BuyListTrainer trainer = new BuyListTrainer(kingdom, true);
			List<BuyListEntry> winners = trainer.Train(32, Console.Out);
			foreach(BuyListEntry entry in winners)
			{
				Console.WriteLine(entry.WinRatio);
				Console.WriteLine(entry.BuyList.ToString());
				Console.WriteLine();
			}
		}

		public static void TuneAllDefaultSets()
		{
			Randomizer.SetRandomSeed(123456789);

			CardSetsModel cardSets = new CardSetsModel(GameSets.Any);
			foreach (CardSetGroup group in cardSets.CardSetGroups)
			{
				foreach (CardSetViewModel cardSet in group.CardSets)
				{
					Kingdom kingdom = new Kingdom(cardSet.CardSet.CardCollection, null, cardSet.CardSet.BaneCard, GameSets.Any, 2, CardUseType.DoNotUse, CardUseType.DoNotUse, StartingHandType.FourThreeSplit);

					BuyListTrainer trainer = new BuyListTrainer(kingdom, true);
					List<BuyListEntry> winners = trainer.Train(32, Console.Out);
					Console.WriteLine(winners.Last().BuyList.ToString());
				}
			}
		}


		public static BuyList[] PlayRound(Kingdom kingdom, List<BuyList> lists)
		{
			Console.WriteLine("Playing round with " + lists.Count + "players");
			int[] wins = new int[lists.Count];
			BuyList[] listsArray = new BuyList[wins.Length];
			for (int i = 0; i < lists.Count;i++)
			{
				listsArray[i] = lists[i];
			}

			for (int i = 0; i < listsArray.Length; i++)
			{
				for (int j = i+1; j< listsArray.Length; j ++)
				{
					for (int k = 0; k < 15; k++)
					{
						int res = PlayGame(kingdom, listsArray[i], listsArray[j]);
						if (res == -1)
						{
							wins[i]++;
							wins[j]--;
						}
						else if (res == 1)
						{
							wins[i]--;
							wins[j]++;
						}

						res = PlayGame(kingdom, listsArray[j], listsArray[i]);
						if (res == -1)
						{
							wins[j]++;
							wins[i]--;
						}
						else if (res == 1)
						{
							wins[j]--;
							wins[i]++;
						}
					}
				}
			}
			//Array.Sort(wins, listsArray);
			for (int i = 0; i < listsArray.Length;i++)
			{
				Console.WriteLine(listsArray[i].List[3].Card.Name);
				Console.WriteLine(wins[i] + " wins");
				Console.WriteLine(listsArray[i].List[3].Card.Name);
				Console.WriteLine(listsArray[i].ToString());
			}
			return listsArray;
		}

		public static int PlayGame(Kingdom kingdom, BuyList a, BuyList b)
		{
			GameModel model = new GameModel();
			BuyListAIStrategy player1 = new BuyListAIStrategy(model);
			BuyListAIStrategy player2 = new BuyListAIStrategy(model);
			player1.BuyList = a;
			player2.BuyList = b;
			Player p1 = new Player("player1", player1, model);
			Player p2 = new Player("player2", player2, model);
			model.Players.Add(p1);
			model.Players.Add(p2);

			model.InitializeGameState(kingdom, 0);
			
			model.PlayGame(100); 

			if(!model.GameOver || model.Result.Winners.Count == 2)
			{
				return 0;
			}
			else if (model.Result.ResultMap[p1].Won)
			{
				return -1;
			}
			else
			{
				return 1;
			}			
		}

		public static void EffectCardTest()
		{
			GameModel model = new GameModel();
			List<CardModel> cardList = new List<CardModel>();
			cardList.Add(new Masquerade());
			Kingdom kingdom = new Kingdom(cardList, null, GameSets.Any, 2);
			model.InitializeGameState(kingdom);
			BaseAIChooser chooser = new BaseAIChooser(model);
			List<CardModel> choices = new List<CardModel>();
			choices.Add(new Masquerade());
			choices.Add(new Duchy());
			CardModel chosen = chooser.ChooseOneCard(Dominion.Model.Chooser.CardChoiceType.Masquerade, string.Empty, Dominion.Model.Chooser.ChoiceSource.FromHand, choices);
		}
	}
}
