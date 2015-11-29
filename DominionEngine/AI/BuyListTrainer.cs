using Dominion;
using Dominion.Model.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace DominionEngine.AI
{
	public class BuyListEntryComparer : Comparer<BuyListEntry>
	{
		public override int Compare(BuyListEntry x, BuyListEntry y)
		{
			if (x.Games == 0 && y.Games == 0) { return 0; }
			if (x.Games == 0) { return -1; }
			if (y.Games == 0) { return 1; }
			double xRatio = x.Wins / ((double)x.Games);
			double yRatio = y.Wins / ((double)y.Games);
			if (xRatio < yRatio) return -1;
			if (yRatio < xRatio) return 1;
			return 0;
		}
	}

	[DebuggerDisplay("{BuyList.ToString()}")]
	public class BuyListEntry
	{
		public BuyListEntry()
		{
			this.SelectionProbability = 1.0;
		}
		public BuyList BuyList { get; set; }
		public int Wins { get; set; }
		public int Games { get; set; }
		public double SelectionProbability { get; set; }
		public double WinRatio
		{
			get
			{
				if (this.Games == 0) return 0;
				return this.Wins / ((double)this.Games);
			}
		}
	}

	public class GenerationWorker
	{
#if JUPITER
		private ManualResetEvent signal;
		private Windows.Foundation.IAsyncAction thread;
#else
		private Thread thread;
#endif
		private Kingdom kingdom;
		private int[] leaderWins;
		private int[] leaderGames;
		private int[] challengerWins;
		private int[] challengerGames;
		private List<BuyListEntry> leaders;
		private List<BuyListEntry> challengers;
		private int startIndex;
		private int count;
		public GenerationWorker(Kingdom kingdom, List<BuyListEntry> leaders, List<BuyListEntry> challengers, int firstChallenger, int count)
		{
			this.kingdom = kingdom;
			this.startIndex = firstChallenger;
			this.count = count;
			this.leaders = leaders;
			this.challengers = challengers;
			this.leaderWins = new int[leaders.Count];
			this.leaderGames = new int[leaders.Count];
			this.challengerWins = new int[count];
			this.challengerGames = new int[count];
#if JUPITER
			this.signal = new ManualResetEvent(false);
#else
			this.thread = new Thread(this.workerStart);
#endif
		}

		public void Go()
		{
#if JUPITER
			this.thread = Windows.System.Threading.ThreadPool.RunAsync(new Windows.System.Threading.WorkItemHandler(this.workerStart));
			this.thread.Completed = new Windows.Foundation.AsyncActionCompletedHandler(completed);
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
			for (int i = 0; i < this.leaders.Count; i++)
			{
				for (int j = 0; j < this.count; j++)
				{
					for (int k = 0; k < BuyListTrainer.GamesPerMatchup; k++)
					{
						KeyValuePair<bool,bool> result = Generation.PlayGame(this.kingdom, this.leaders[i], this.challengers[this.startIndex + j]);
						if (result.Key) this.leaderWins[i]++;
						if (result.Value) this.challengerWins[j]++;

						result = Generation.PlayGame(this.kingdom, this.challengers[this.startIndex + j], this.leaders[i]);
						if (result.Key) this.challengerWins[j]++;
						if (result.Value) this.leaderWins[i]++;

						this.leaderGames[i] += 2;
						this.challengerGames[j] += 2;
					}
				}
			}
		}

		public void WriteResults()
		{
			for (int i = 0; i < this.count; i++)
			{
				this.challengers[this.startIndex + i].Games += this.challengerGames[i];
				this.challengers[this.startIndex + i].Wins += this.challengerWins[i];
			}
			for (int i = 0; i < this.leaders.Count; i++)
			{
				this.leaders[i].Games += this.leaderGames[i];
				this.leaders[i].Wins += this.leaderWins[i];
			}
		}
	}

	public class Generation
	{
		private Kingdom kingdom;
		private bool isFourThreeSplit;
		private List<BuyListEntry> leaders;
		private List<BuyListEntry> challengers;
		private List<BuyListEntry> winners;
		private List<KeyValuePair<CardModel, CardModel>> openers;
		public List<BuyListEntry> Winners {  get { return this.winners; } }

		public Generation(Kingdom kingdom, bool isFourThreeSplit)
		{
			this.kingdom = kingdom;
			this.leaders = new List<BuyListEntry>();
			this.challengers = new List<BuyListEntry>();
			this.winners = new List<BuyListEntry>();
			this.isFourThreeSplit = isFourThreeSplit;
			this.openers = new List<KeyValuePair<CardModel, CardModel>>();
		}

		public void Populate(Generation previous)
		{
			this.openers = previous.openers;
			foreach(BuyListEntry entry in previous.winners)
			{
				BuyListEntry newEntry = new BuyListEntry();
				newEntry.BuyList = entry.BuyList;
				this.leaders.Add(newEntry);
			}
		}

		public void PlayRound()
		{
			int numThreads = Environment.ProcessorCount;
			GenerationWorker[] workers = new GenerationWorker[numThreads];
			int challengersPerWorker = this.challengers.Count / workers.Length;
			int extra = this.challengers.Count % workers.Length;
			int extraCount = extra > 0 ? 1 : 0;
			int currentIndex = 0;
			for (int i = 0; i < workers.Length;i++)
			{
				workers[i] = new GenerationWorker(this.kingdom, this.leaders, this.challengers, currentIndex, challengersPerWorker + extraCount);
				extra--;
				currentIndex += challengersPerWorker + extraCount;
				if (extra == 0)
				{
					extraCount = 0;
				}
				workers[i].Go();
			}
			for (int i = 0; i < workers.Length; i++)
			{
				workers[i].Wait();
			}

			for (int i = 0; i < workers.Length; i++)
			{
				workers[i].WriteResults();
			}

			this.challengers.Sort(new BuyListEntryComparer());

			Dictionary<KeyValuePair<Type, Type>, int> openerCount = new Dictionary<KeyValuePair<Type, Type>, int>();
			int current = this.challengers.Count - 1;
			while(current >= 0 && this.winners.Count < BuyListTrainer.MaxLeaderCount)
			{
				int count;
				KeyValuePair<Type, Type> key = new KeyValuePair<Type, Type>(this.challengers[current].BuyList.OpeningBuy1, this.challengers[current].BuyList.OpeningBuy2);
				if (!openerCount.TryGetValue(key, out count))
				{
					count = 1;
				}
				else
				{
					count++;
				}
				if (count <= BuyListTrainer.MaxLeaderSameOpening)
				{
					this.winners.Add(this.challengers[current]);
				}
				openerCount[key] = count;
				current--;
			}
		}

		// returns whether or not a and b won or lost as a pair, e.g. (true, true) - both won; (true, false) - a won, b lost
		public static KeyValuePair<bool, bool> PlayGame(Kingdom kingdom, BuyListEntry a, BuyListEntry b)
		{
			GameModel model = new GameModel();
			BuyListAIStrategy player1 = new BuyListAIStrategy(model);
			BuyListAIStrategy player2 = new BuyListAIStrategy(model);
			player1.BuyList = a.BuyList;
			player2.BuyList = b.BuyList;
			Player p1 = new Player("player1", player1, model);
			Player p2 = new Player("player2", player2, model);
			model.Players.Add(p1);
			model.Players.Add(p2);

			using (model.TextLog.SuppressLogging())
			{
				model.InitializeGameState(kingdom, 0);
				model.PlayGame(100);
			}

			if (!model.GameOver)
			{	
				// not finished -> loss for both
				return new KeyValuePair<bool, bool>(false, false);
			}
			else if(model.Result.Winners.Count == 2)
			{
				return new KeyValuePair<bool, bool>(true, true);
			}
			else if (model.Result.ResultMap[p1].Won)
			{
				return new KeyValuePair<bool, bool>(true, false);
			}
			else
			{
				return new KeyValuePair<bool, bool>(false, true);
			}
		}

		public void BuildChallengers(bool mutateThresholds)
		{
			foreach (BuyListEntry leader in this.leaders)
			{
				//always add copy of old leaders to challengers
				BuyListEntry leaderClone = new BuyListEntry();
				leaderClone.BuyList = leader.BuyList;
				this.challengers.Add(leaderClone);
			}

			while (this.challengers.Count < BuyListTrainer.MaxChallengerCount)
			{
				BuyListEntry newChallenger;
				if (mutateThresholds)
				{
					newChallenger = this.MutateBuyThresholds(this.leaders[Randomizer.Next(this.leaders.Count)].BuyList);
				}
				else
				{
					newChallenger = this.MutateStrategy(this.leaders[Randomizer.Next(this.leaders.Count)].BuyList);
				}

				bool use = true;
				
				foreach (BuyListEntry existing in this.challengers)
				{
					if (existing.BuyList.Equals(newChallenger.BuyList))// || IsSimilar(existing.BuyList, newChallenger.BuyList))
					{
						use = false;
						break;
					}
				}
				
				if (use)
				{
					this.challengers.Add(newChallenger);
				}
			}
		}

		private bool IsSimilar(BuyList lhs, BuyList rhs)
		{
			if (lhs.OpeningBuy1 != rhs.OpeningBuy1) return false;
			if (lhs.OpeningBuy2 != rhs.OpeningBuy2) return false;
			// for now skip thresholds
			Dictionary<string, int> lhsIndex = new Dictionary<string, int>();
			Dictionary<string, int> rhsIndex = new Dictionary<string, int>();
			HashSet<string> cards = new HashSet<string>();
			for (int i = 0; i < lhs.List.Count; i++)
			{
				lhsIndex[lhs.List[i].Card.Name] = i;
				cards.Add(lhs.List[i].Card.Name);
			}
			for (int i = 0; i < rhs.List.Count; i++)
			{
				rhsIndex[rhs.List[i].Card.Name] = i;
				cards.Add(rhs.List[i].Card.Name);
			}
			if (lhsIndex.Count != rhsIndex.Count || lhsIndex.Count != cards.Count)
			{
				return false;
			}
			int itemDiffCount = 0;
			int locationDiffCount = 0;
			foreach(KeyValuePair<string, int> entry in lhsIndex)
			{
				if (!rhsIndex.ContainsKey(entry.Key)) return false;
				locationDiffCount += Math.Abs(entry.Value - rhsIndex[entry.Key]);
				itemDiffCount += Math.Abs(lhs.List[entry.Value].Count - rhs.List[rhsIndex[entry.Key]].Count);
			}
			return locationDiffCount <= 3 || itemDiffCount <= 3;
		}

		public void PopulateInitial()
		{
			List<BuyListEntry> strategies = new List<BuyListEntry>();

			BuyList buyList;
			BuyListEntry entry = new BuyListEntry();

			List<CardModel> card1Options = this.kingdom.Cards.Where(c => c.GetBaseCost() <= (this.isFourThreeSplit ? 3 : 2) && !c.CostsPotion).ToList();
			if(this.isFourThreeSplit)
			{
				card1Options.Add(new Silver());
			}
			if (!this.isFourThreeSplit)
			{
				card1Options.Add(null);
			}

			List<CardModel> card2Options = this.kingdom.Cards.Where(c => c.GetBaseCost() <= (this.isFourThreeSplit ? 4 : 5) && !c.CostsPotion).ToList();
			card2Options.Add(new Silver());
			if (this.kingdom.Cards.Any(c => c.CostsPotion))
			{
				card2Options.Add(new Potion());
			}

			List<CardModel> potionCosts = new List<CardModel>();
			potionCosts.Add(null);
			potionCosts.AddRange(this.kingdom.Cards.Where(c => c.CostsPotion));
			for (int i = 0; i < card1Options.Count; i++)
			{
				CardModel card1 = card1Options[i];
				
				for (int j = i; j < card2Options.Count; j++)
				{
					CardModel card2 = card2Options[j];

					for (int k = 0; k < potionCosts.Count; k++)
					{
						CardModel potionCard = potionCosts[k];
						if (card2 is Potion && potionCard != null || potionCard == null && !(card2 is Potion))
						{
							buyList = new BuyList();
							buyList.OpeningBuy1 = card1 != null ? card1.GetType() : null;
							buyList.OpeningBuy2 = card2 != null ? card2.GetType() : null;

							if (potionCard != null)
							{
								buyList.List.Add(new BuyListItem(potionCard.GetType(), potionCard.CardPriority.MaxGain));
							}
							if (this.kingdom.UsesColonies)
							{
								buyList.List.Add(new BuyListItem(typeof(Platinum), 1));
								buyList.List.Add(new BuyListItem(typeof(Colony), 99));
								buyList.List.Add(new BuyListItem(typeof(Platinum), 99));
							}
							buyList.List.Add(new BuyListItem(typeof(Gold), 1));
							buyList.List.Add(new BuyListItem(typeof(Province), 99));
							buyList.List.Add(new BuyListItem(typeof(Gold), 99));
							buyList.List.Add(new BuyListItem(typeof(Silver), 99));
							buyList.ProvinceBuyThreshold = 5;
							buyList.DuchyBuyThreshold = 5;
							buyList.EstateBuyThreshold = 2;
							entry = new BuyListEntry();
							entry.BuyList = buyList;
							entry.SelectionProbability = OpeningBook.GetOpeningStrength(card1 != null ? card1.Name : "", card2 != null ? card2.Name : "");
							if (card1 != null) entry.SelectionProbability *= card1.CardPriority.WinRateWith;
							if (card2 != null) entry.SelectionProbability *= card2.CardPriority.WinRateWith;
							strategies.Add(entry);
						}
					}

					this.openers.Add(new KeyValuePair<CardModel, CardModel>(card1, card2));
				}
			}
			strategies = strategies.OrderBy(e => e.SelectionProbability).ToList();
			strategies.Reverse();
			int leaderCount = Math.Min(BuyListTrainer.MaxLeaderCount, strategies.Count);
			this.leaders.AddRange(strategies.Take(leaderCount));
			this.challengers.AddRange(strategies.Skip(leaderCount));
		}

		private BuyListEntry MutateStrategy(BuyList input)
		{
			BuyListEntry e = new BuyListEntry() { BuyList = input.Clone() };
							
			int silverIndex = 99;
			int goldIndex = 99;
			for (int i = 0; i < e.BuyList.List.Count; i++)
			{
				if (e.BuyList.List[i].Card.Equals(typeof(Silver)) && e.BuyList.List[i].Count == 99) silverIndex = i;
			}
			for (int i = 0; i < e.BuyList.List.Count; i++)
			{
				if (e.BuyList.List[i].Card.Equals(typeof(Gold)) && e.BuyList.List[i].Count == 99) goldIndex = i;
			}

			// mutate opener
			if (Randomizer.NextDouble() <= .20)
			{
				int which = Randomizer.Next(this.openers.Count);
				e.BuyList.OpeningBuy1 = this.openers[which].Key != null ? this.openers[which].Key.GetType() : null;
				e.BuyList.OpeningBuy2 = this.openers[which].Value != null ? this.openers[which].Value.GetType() : null;
			}

			// add kingdom card
			for (int k = 0; k < 2; k++)
			{
				if (Randomizer.NextDouble() <= 0.6)
				{
					CardModel card = this.kingdom.Cards[Randomizer.Next(this.kingdom.Cards.Count)];
					List<int> possible = new List<int>();
					for (int i = 0; i <= e.BuyList.List.Count; i++)
					{
						if (i > 0 && e.BuyList.List[i - 1].Card.Equals(card.GetType()) ||
							i < e.BuyList.List.Count && e.BuyList.List[i].Card.Equals(card.GetType()))
						{
							continue;
						}
						if (card.GetBaseCost() >= 6 && i > goldIndex || card.GetBaseCost() >= 3 && i > silverIndex) continue;

						possible.Add(i);
					}
					if (possible.Any())
					{
						int i = possible[Randomizer.Next(possible.Count)];

						possible.Clear();
						for (int j = 0; j < 4; j++)
						{
							//e.SelectionProbability *= card.CardPriority.WinRateWith;
							if (card.CardPriority.MaxGain >= (1 << j))
							{
								possible.Add(1 << j);
							}
						}
						if (possible.Any())
						{
							int kk = Randomizer.Next(possible.Count);
							
							e.BuyList.List.Insert(i, new BuyListItem(card.GetType(), possible[kk]));
							if (card.CostsPotion)
							{
								if (!e.BuyList.OpeningBuy2.Equals(typeof(Potion)) && !e.BuyList.List.Any(li => li.Card.Equals(typeof(Potion))))
								{
									e.BuyList.List.Insert(i, new BuyListItem(typeof(Potion), 1));
								}
							}
						}
					}
				}
			}

			// alter card counts
			if (Randomizer.NextDouble() <= 0.7)
			{
				int i = Randomizer.Next(e.BuyList.List.Count);

				if (e.BuyList.List[i].Count < 99)
				{
					CardModel card = (CardModel)Activator.CreateInstance(e.BuyList.List[i].Card);
					int k = Randomizer.Next(7);
					if (k == 0)
					{
						e.BuyList.List.RemoveAt(i);
					}
					else if (k == 1 && e.BuyList.List[i].Count > 1)
					{
						e.BuyList.List[i].Count = e.BuyList.List[i].Count - 1;
					}
					else if (k == 2 && e.BuyList.List[i].Count < 10 && card.CardPriority.MaxGain > e.BuyList.List[i].Count)
					{
						e.BuyList.List[i].Count = e.BuyList.List[i].Count + 1;
					}
					else if (k > 2)
					{
						if (card.CardPriority.MaxGain >= (1 << (k - 3)))
						{
							e.BuyList.List[i].Count = 1 << (k - 3);
						}
					}
				}
			}

			// swap
			if (Randomizer.NextDouble() <= 0.5)
			{
				for (int tries = 0; tries < 20; tries++)
				{
					int i = Randomizer.Next(e.BuyList.List.Count - 1);
					int j = Randomizer.Next(i + 1, e.BuyList.List.Count);
					CardModel card1 = (CardModel)Activator.CreateInstance(e.BuyList.List[i].Card);

					if (card1.GetBaseCost() >= 6 && j >= goldIndex || card1.GetBaseCost() >= 3 && j >= silverIndex) continue;
					if (e.BuyList.List[i].Count == 99 || e.BuyList.List[j].Count == 99) continue;
					BuyListItem temp = e.BuyList.List[i];
					e.BuyList.List[i] = e.BuyList.List[j];
					e.BuyList.List[j] = temp;
					break;
				}
			}

			Dictionary<Type, int> cardCounts = new Dictionary<Type, int>();
			// Normalize entries
			for (int i = 0; i < e.BuyList.List.Count;i++)
			{
				if (e.BuyList.List[i].Count != 99)
				{
					// merge adjacent
					if (i + 1 < e.BuyList.List.Count && e.BuyList.List[i + 1].Count != 99 && e.BuyList.List[i].Card.Equals(e.BuyList.List[i + 1].Card))
					{
						e.BuyList.List[i].Count += e.BuyList.List[i + 1].Count;
						e.BuyList.List.RemoveAt(i + 1);
					}

					// cap counts at max available.
					int current = 0;
					int max = 10; // todo handle victory, rats
					if (cardCounts.TryGetValue(e.BuyList.List[i].Card, out current))
					{
						if (current == max)
						{
							// delete this
							e.BuyList.List.RemoveAt(i);
							i--;
							continue;
						}
						else if (current + e.BuyList.List[i].Count > max)
						{
							e.BuyList.List[i].Count = max - current;
						}
					}
					current += e.BuyList.List[i].Count;
					cardCounts[e.BuyList.List[i].Card] = current;
				}
			}
			
			return e;
		}

		private BuyListEntry MutateBuyThresholds(BuyList input)
		{
			BuyListEntry e = new BuyListEntry() { BuyList = input.Clone() };

			bool changedSomething = false;
			do
			{
				if (this.kingdom.UsesColonies)
				{
					if (Randomizer.NextDouble() < .3)
					{
						int max = this.kingdom.VictoryCardCount;
						int min = 0;
						e.BuyList.ColonyBuyThreshold = Randomizer.Next(min, max + 1);
						changedSomething = true;
					}
				}

				if (Randomizer.NextDouble() < .3)
				{
					int max = this.kingdom.VictoryCardCount;
					int min = 0;
					e.BuyList.ProvinceBuyThreshold = Randomizer.Next(min, max + 1);
					changedSomething = true;
				}

				if (Randomizer.NextDouble() < .8)
				{
					int max = this.kingdom.VictoryCardCount;
					int min = 0;
					e.BuyList.DuchyBuyThreshold = Randomizer.Next(min, max + 1);
					changedSomething = true;
				}

				if (Randomizer.NextDouble() < .5)
				{
					int max = this.kingdom.VictoryCardCount;
					int min = 0;
					e.BuyList.EstateBuyThreshold = Randomizer.Next(min, max + 1);
					changedSomething = true;
				}
			} while (!changedSomething);
			
			return e;
		}
	}

	public class BuyListTrainer
	{
		public static int MaxLeaderCount = 5;
		public static int MaxChallengerCount = 100;
		public static int GamesPerMatchup = 16;
		public static int MaxLeaderSameOpening = 3;

		private Kingdom kingdom;
		private bool isFourThreeSplit;
		private List<Generation> generations;

		public BuyListTrainer(Kingdom kingdom, bool isFourThreeSplit)
		{
			this.kingdom = kingdom;
			this.isFourThreeSplit = isFourThreeSplit;
			this.generations = new List<Generation>();
		}

		public List<BuyListEntry> Train(int generations)
		{
			return this.Train(generations, null);
		}

		public List<BuyListEntry> TrainThresholds()
		{
			Generation generation = new Generation(this.kingdom, this.isFourThreeSplit);
			if(this.generations.Count == 0)
			{
				generation.PopulateInitial();
			}
			else
			{
				generation.Populate(this.generations[this.generations.Count - 1]);
			}
			
			generation.BuildChallengers(mutateThresholds: true);
			generation.PlayRound();
			this.generations.Add(generation);
			return this.generations[this.generations.Count - 1].Winners; 
		}

		public List<BuyListEntry> Train(int generations, TextWriter logger)
		{			
			for (int iterations = 0; iterations < generations; iterations++)
			{
				Generation generation = new Generation(this.kingdom, this.isFourThreeSplit);
				if (this.generations.Count == 0)
				{
					generation.PopulateInitial();
				}
				else
				{
					generation.Populate(this.generations[this.generations.Count - 1]);
				}

				generation.BuildChallengers(mutateThresholds: false);
				generation.PlayRound();
				this.generations.Add(generation);

				if (logger != null)
				{
					logger.WriteLine("iteration " + iterations);

					foreach (BuyListEntry e in ((IEnumerable<BuyListEntry>)this.generations.Last().Winners).Reverse())
					{
						logger.WriteLine(e.WinRatio);
						logger.WriteLine(e.BuyList.ToString());
						logger.WriteLine();
					}
					if (this.generations.Count > 1 && this.generations[this.generations.Count - 1].Winners.Last().Equals(this.generations[this.generations.Count - 2].Winners.Last()))
					{
						logger.WriteLine("stabilized");
					}
				}			
				
			}

			return this.generations[this.generations.Count - 1].Winners;
		}
	}
}
