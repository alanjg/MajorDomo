using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Dominion;
using Dominion.Model.Actions;
using Dominion.Model.Chooser;

namespace DominionEngine.AI
{
	public enum GameSegment
	{
		Early, // opening few rounds, no province buying power
		Middle, // middle of the game; big money is going for provinces, engines have not fully ramped up.
		Endgame // end of game; both players are maximizing points, engines are fully functional
	}

	public class AICardPlayComparer : Comparer<CardModel>
	{
		public override int Compare(CardModel x, CardModel y)
		{
			return AIPileComparer.Compare(x, y);
		}
	}

	// used to decide which cards to buy
	public class AIPileComparer : Comparer<Pile>
	{
		static AIPileComparer()
		{
			AIPileComparer.cardOrder = new Dictionary<string, int>();

			for (int i = 0; i < ranks.Length; i++)
			{
				for (int j = 0; j < ranks[i].Length; j++)
				{
					AIPileComparer.cardOrder[((CardModel)Activator.CreateInstance(ranks[i][j])).Name] = 100 * i + j;
				}
			}
		}

		private GameSegment gameSegment;
		private BaseAIChooser chooser;
		public AIPileComparer(GameSegment gameSegment, BaseAIChooser chooser)
		{
			this.gameSegment = gameSegment;
			this.chooser = chooser;
		}

		public override int Compare(Pile x, Pile y)
		{
			return DiscardCardComparer.Compare(x.TopCard, y.TopCard, false, true, this.chooser.Player, this.gameSegment);
			//return AIPileComparer.Compare(x.TopCard, y.TopCard);
		}

		public static int Compare(CardModel x, CardModel y)
		{
			if (x.Name == y.Name) return 0;

			int xCost = x.GetBaseCost();
			int yCost = y.GetBaseCost();
			if (x.CostsPotion && !y.CostsPotion)
			{
				return 1;
			}
			else if (y.CostsPotion && !x.CostsPotion)
			{
				return -1;
			}
			if (xCost > yCost)
			{
				return 1;
			}
			else if (yCost > xCost)
			{
				return -1;
			}
			int xi = -1, yi = -1;
			AIPileComparer.cardOrder.TryGetValue(x.Name, out xi);
			AIPileComparer.cardOrder.TryGetValue(y.Name, out yi);
			return xi - yi;
		}

		private static Type[] rank7 = new Type[] { typeof(Expand), typeof(Forge), typeof(Bank), typeof(KingsCourt) };
		private static Type[] rank6 = new Type[] { typeof(Adventurer), typeof(Farmland), typeof(Harem), typeof(Fairgrounds), typeof(Nobles), typeof(Hoard), typeof(BorderVillage), typeof(GrandMarket), typeof(Goons) };
		private static Type[] rank5 = new Type[] { typeof(CountingHouse), typeof(Saboteur), typeof(Stash), typeof(Explorer), typeof(Contraband), typeof(Cache), typeof(Mine), typeof(Mandarin), typeof(Outpost), typeof(Harvest), typeof(Tribute), typeof(RoyalSeal), typeof(Mint), typeof(CouncilRoom), typeof(Library), typeof(Treasury), typeof(Market), typeof(Cartographer), typeof(Inn), typeof(TradingPost), typeof(HornOfPlenty), typeof(City), typeof(Rabble), typeof(Venture), typeof(MerchantShip), typeof(Upgrade), typeof(Jester), typeof(Bazaar), typeof(Highway), typeof(Festival), typeof(Duke), typeof(Stables), typeof(Haggler), typeof(Laboratory), typeof(Vault), typeof(Embassy), typeof(Margrave), typeof(Apprentice), typeof(Tactician), typeof(Torturer), typeof(Minion), typeof(GhostShip), typeof(Governor), typeof(HuntingParty), typeof(IllGottenGains), typeof(Wharf), typeof(Witch), typeof(Mountebank) };
		private static Type[] rank4 = new Type[] { typeof(Scout), typeof(Thief), typeof(Coppersmith), typeof(Talisman), typeof(Spy), typeof(TreasureMap), typeof(PirateShip), typeof(Navigator), typeof(Feast), typeof(Bureaucrat), typeof(NomadCamp), typeof(WalledVillage), typeof(NobleBrigand), typeof(Remodel), typeof(Ironworks), typeof(SpiceMerchant), typeof(Trader), typeof(Quarry), typeof(Island), typeof(Baron), typeof(Cutpurse), typeof(FarmingVillage), typeof(ThroneRoom), typeof(Moneylender), typeof(MiningVillage), typeof(HorseTraders), typeof(Conspirator), typeof(WorkersVillage), typeof(Smithy), typeof(Gardens), typeof(SilkRoad), typeof(Envoy), typeof(Bridge), typeof(Caravan), typeof(Salvager), typeof(Militia), typeof(Bishop), typeof(Monument), typeof(YoungWitch), typeof(Remake), typeof(Tournament), typeof(JackOfAllTrades), typeof(SeaHag) };
		private static Type[] rank3 = new Type[] { typeof(Chancellor), typeof(Develop), typeof(Woodcutter), typeof(FortuneTeller), typeof(Workshop), typeof(GreatHall), typeof(Smugglers), typeof(ShantyTown), typeof(Loan), typeof(TradeRoute), typeof(WishingWell), typeof(BlackMarket), typeof(Oasis), typeof(Lookout), typeof(Oracle), typeof(Village), typeof(Watchtower), typeof(Tunnel), typeof(Scheme), typeof(Warehouse), typeof(Swindler), typeof(Steward), typeof(Menagerie), typeof(FishingVillage), typeof(Masquerade), typeof(Ambassador) };
		private static Type[] rank2 = new Type[] { typeof(SecretChamber), typeof(Duchess), typeof(PearlDiver), typeof(Herbalist), typeof(Moat), typeof(Cellar), typeof(Embargo), typeof(Pawn), typeof(NativeVillage), typeof(Haven), typeof(Crossroads), typeof(FoolsGold), typeof(Lighthouse), typeof(Courtyard), typeof(Hamlet), typeof(Chapel) };
		private static Type[] rank1 = new Type[] { };
		private static Type[] rankPotion = new Type[] { typeof(Transmute), typeof(PhilosophersStone), typeof(Possession), typeof(Alchemist), typeof(University), typeof(Golem), typeof(Apothecary), typeof(Vineyard), typeof(ScryingPool), typeof(Familiar) };
		private static Type[][] ranks = new Type[][] { rank1, rank2, rank3, rank4, rank5, rank6, rank7, rankPotion };
		private static Dictionary<string, int> cardOrder = null;
	}

	public class BaseAIChooser : ChooserBase
	{
		private bool hasUsedMoatThisTurn;
		private bool hasUsedSecretChamberThisTurn;
		private bool isSecretChamberActive;
		private CardModel attackTriggeringSecretChamber;

		private GameModel gameModel;
		private BaseAIStrategy strategy;
		private Player player;
		public Player Player
		{
			get
			{
				if (this.player == null)
				{
					this.player = strategy.Player;
				}
				return this.player;
			}
		}

		public BaseAIChooser(GameModel gameModel)
		{
			this.gameModel = gameModel;
		}

		public void SetStrategy(BaseAIStrategy strategy)
		{
			this.strategy = strategy;
		}

		public void OnTurnStart(Player player)
		{
			this.hasUsedSecretChamberThisTurn = false;
			this.hasUsedMoatThisTurn = false;
			this.isSecretChamberActive = false;
			this.attackTriggeringSecretChamber = null;
		}

		protected virtual Comparer<CardModel> GetCardGainComparer(GameSegment gameSegment)
		{
			return new GainCardComparer(this, gameSegment);
		}

		protected virtual Comparer<CardModel> GetCardDiscardComparer(GameSegment gameSegment)
		{
			return new DiscardCardComparer(this, gameSegment);
		}
		protected virtual Comparer<CardModel> GetCardPlayComparer(GameSegment gameSegment)
		{
			return new AICardPlayComparer();
		}

		private GameSegment GetGameSegment()
		{
			return this.strategy.GetGameSegment();
		}

		public override IEnumerable<CardModel> ChooseCards(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices)
		{
			GameSegment gameSegment = this.GetGameSegment();
			IEnumerable<CardModel> cardsBadToGoodDiscard = choices.OrderBy(c => c, this.GetCardDiscardComparer(gameSegment));
			IEnumerable<CardModel> cardsGoodToBadDiscard = cardsBadToGoodDiscard.Reverse();
			IEnumerable<CardModel> badCardsDiscard = choices.Where(c => c.Is(CardType.Curse) || c.Is(CardType.Ruins) || c is Estate || c.Is(CardType.Shelter));
			IEnumerable<CardModel> sortedBadDiscard = badCardsDiscard.OrderBy(c => c, this.GetCardDiscardComparer(gameSegment));
			IEnumerable<CardModel> goodCardsDiscard = choices.Except(badCardsDiscard);
			IEnumerable<CardModel> sortedGoodDiscard = goodCardsDiscard.OrderBy(c => c, this.GetCardDiscardComparer(gameSegment)).Reverse();

			IEnumerable<CardModel> cardsBadToGoodGain = choices.OrderBy(c => c, this.GetCardGainComparer(gameSegment));
			IEnumerable<CardModel> cardsGoodToBadGain = cardsBadToGoodGain.Reverse();
			IEnumerable<CardModel> badCardsGain = choices.Where(c => c.Is(CardType.Curse) || c.Is(CardType.Ruins) || c is Estate || c.Is(CardType.Shelter));
			IEnumerable<CardModel> coppers = choices.Where(c => c is Copper);
			IEnumerable<CardModel> sortedBadGain = badCardsGain.OrderBy(c => c, this.GetCardGainComparer(gameSegment));
			IEnumerable<CardModel> goodCardsGain = choices.Except(badCardsGain);
			IEnumerable<CardModel> goodCardsGainNoCopper = choices.Except(badCardsGain).Except(coppers);
			IEnumerable<CardModel> sortedGoodGain = goodCardsGain.OrderBy(c => c, this.GetCardGainComparer(gameSegment)).Reverse();

			IEnumerable<CardModel> cardsGoodToPlay = choices.OrderBy(c => c, this.GetCardPlayComparer(gameSegment)).Reverse();

			switch (choiceType)
			{
				case CardChoiceType.Gain:
				case CardChoiceType.GainInHand:
				case CardChoiceType.GainOnTopOfDeck:
				case CardChoiceType.GainForHornOfPlenty:
				case CardChoiceType.GainForIronworks:
					return cardsGoodToBadGain.Take(maxChoices);
				case CardChoiceType.ForceGain:
					return cardsBadToGoodGain.Take(maxChoices);

				case CardChoiceType.Trash:
				case CardChoiceType.TrashFromHand:

					// keep at least 3 coins worth of money
					int money = this.Player.AllCardsInDeck.Sum(c => c.Coins);
					int cushion = money - 3;
					if (cushion > 0)
					{
						IEnumerable<CardModel> skip = coppers.Take(cushion);
						badCardsGain = badCardsGain.Concat(skip);
						sortedGoodGain = sortedGoodGain.Except(skip);
					}
					if (badCardsGain.Count() >= minChoices) return badCardsGain.Take(Math.Min(maxChoices, badCardsGain.Count()));
					return badCardsGain.Concat(sortedGoodGain.Reverse()).Take(minChoices);

				case CardChoiceType.ForceTrash:
					if (goodCardsGain.Count() >= minChoices) return sortedGoodGain.Take(Math.Min(maxChoices, goodCardsGain.Count()));
					return goodCardsGain.Concat(sortedBadGain.Reverse()).Take(minChoices);

				case CardChoiceType.Discard:
					return cardsBadToGoodDiscard.Take(minChoices);

				case CardChoiceType.ForceDiscard:
					return cardsGoodToBadDiscard.Take(maxChoices);

				case CardChoiceType.DiscardOrPutOnDeck:
					if (minChoices > badCardsDiscard.Count() )
					{
						return cardsBadToGoodDiscard.Take(minChoices);
					}
					else if (maxChoices <= badCardsDiscard.Count())
					{
						return cardsBadToGoodDiscard.Take(maxChoices);
					}
					else
					{
						return cardsBadToGoodDiscard.Take(minChoices);
					}
				case CardChoiceType.GolemPlayOrder:
					return cardsGoodToPlay.Take(maxChoices);

				case CardChoiceType.ThroneRoom:
				case CardChoiceType.KingsCourt:
				case CardChoiceType.Procession:
					CardModel bestCard = null;
					int bestVal = -1000;
					foreach (CardModel card in choices)
					{
						if (card.Is(CardType.Action))
						{
							// encourage playing procession where we'll gain a card
							int val = card.CardPriority.MultiplePlayPriority;
							if (choiceType == CardChoiceType.Procession && !strategy.CardToPlayHasNUpgradeTargets(choices, card, 1) && !strategy.CardHasOnTrashBenefit(card))
							{
								val -= 100;
							}
							
							if (val > bestVal && strategy.ShouldPlayAction(choices, card))
							{
								bestVal = card.CardPriority.MultiplePlayPriority;
								bestCard = card;
							}
						}
					}
					if (bestCard != null)
					{
						return new CardModel[] { bestCard };
					}
					else
					{
						return choices.Take(minChoices);
					}
				case CardChoiceType.Counterfeit:
					return cardsGoodToPlay.Take(maxChoices);

				case CardChoiceType.TrashForApprentice:
				case CardChoiceType.TrashForBishop:
					{
						CardModel target = choices.FirstOrDefault(c => c is Curse || c.Is(CardType.Ruins));
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => c is Estate || c.Is(CardType.Shelter));
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => c is Copper);
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => !c.Is(CardType.Victory));
						if (target != null) return new CardModel[] { target };
						return new CardModel[] { choices.First() };
					}
				case CardChoiceType.TrashForDeathCart:
					{
						CardModel target = choices.FirstOrDefault(c => c.Is(CardType.Ruins));
						if (target != null) return new CardModel[] { target };
						int minCost = choices.Min(c => c.GetBaseCost());
						return choices.Where(c => c.GetBaseCost() == minCost).Take(1);
					}
				case CardChoiceType.TrashForDevelop:
					{
						CardModel target = choices.FirstOrDefault(c => this.gameModel.GetCost(c) == 7);
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => c.Is(CardType.Ruins) || c is Curse);
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => c is Estate || c.Is(CardType.Shelter));
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => c is Copper );
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => !c.Is(CardType.Victory));
						if (target != null) return new CardModel[] { target };
						return choices.Take(1);
					}
				case CardChoiceType.TrashForExpand:
					{
						CardModel target = choices.FirstOrDefault(c => this.gameModel.GetCost(c) >= 8 && !(c is Province) && !(c is Colony));
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => this.gameModel.GetCost(c) >= 5 && !(c is Province) && !(c is Colony));
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => this.gameModel.GetCost(c) >= 8 && !(c is Colony));
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => c is Estate);
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => c.Is(CardType.Ruins) || c.Is(CardType.Shelter) || c is Curse);
						if (target != null) return new CardModel[] { target };
						return choices.Take(1);
					}
				case CardChoiceType.TrashForFarmland:
					{
						CardModel target = choices.FirstOrDefault(c => this.gameModel.GetCost(c) == 9); 
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => this.gameModel.GetCost(c) == 6);
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => c is Estate);
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => c.Is(CardType.Ruins) || c.Is(CardType.Shelter) || c is Curse);
						if (target != null) return new CardModel[] { target };
						target = choices.FirstOrDefault(c => !c.Is(CardType.Victory));
						if (target != null) return new CardModel[] { target };
						return choices.Take(1);
					}
				case CardChoiceType.TrashForForge:
					{
						IEnumerable<CardModel> bad = choices.Where(c => c is Copper || c is Curse || c.Is(CardType.Ruins) || c is Estate || c.Is(CardType.Shelter));
						IEnumerable<CardModel> other = choices.Except(bad);
						int sum = bad.Sum(c => this.gameModel.GetCost(c));
						int gold = this.gameModel.PileMap[typeof(Gold)].GetCost();
						int province = this.gameModel.PileMap[typeof(Province)].GetCost();
						if (sum == gold || sum == province) { return bad; }
						foreach (CardModel card in other)
						{
							int c = this.gameModel.GetCost(card);
							if (sum + c == gold || sum + c == province)
							{
								return bad.Union(new CardModel[] { card });
							}
						}

						foreach (CardModel card in other)
						{
							foreach (CardModel card2 in other)
							{
								if (card != card2)
								{
									int c = this.gameModel.GetCost(card) + this.gameModel.GetCost(card2);
									if (sum + c == gold || sum + c == province)
									{
										return bad.Union(new CardModel[] { card, card2 });
									}
								}
							}
						}

						foreach (CardModel card in choices)
						{
							foreach (CardModel card2 in choices)
							{
								if (card != card2)
								{
									int c = this.gameModel.GetCost(card) + this.gameModel.GetCost(card2);
									if (sum + c == gold || sum + c == province)
									{
										return new CardModel[] { card, card2 };
									}
								}
							}
						}
						return bad;
					}
				case CardChoiceType.TrashForGraverobber:
					{
						int province = this.gameModel.PileMap[typeof(Province)].GetCost();
						CardModel highest = null;
						int highestCost = -1;
						foreach (CardModel card in choices)
						{
							int cost = this.gameModel.GetCost(card);
							if (cost + 3 >= province)
							{
								return new CardModel[] { card };
							}
							else if (cost > highestCost)
							{
								highest = card;
								highestCost = cost;
							}
						}
						return new CardModel[] { highest };
					}
				case CardChoiceType.TrashForKnight:
					{
						CardModel knight = choices.FirstOrDefault(c => c.Is(CardType.Knight));
						if (knight != null)
						{
							return new CardModel[] { knight };
						}
						CardModel nonVictory = choices.FirstOrDefault(c => !c.Is(CardType.Victory));
						if (nonVictory != null)
						{
							return new CardModel[] { nonVictory };
						}
						return choices.OrderByDescending(c => this.gameModel.GetCost(c)).Take(1);
					}
				case CardChoiceType.TrashForMine:
					{
						CardModel gold = choices.FirstOrDefault(c => c is Gold);
						Pile platinum;
						if (gold != null && this.gameModel.PileMap.TryGetValue(typeof(Platinum), out platinum) && platinum.Count > 0)
						{
							return new CardModel[] { gold };
						}
						CardModel silver = choices.FirstOrDefault(c => c is Silver);
						if (silver != null)
						{
							return new CardModel[] { silver };
						}
						CardModel copper = choices.FirstOrDefault(c => c is Copper);
						if (copper != null)
						{
							return new CardModel[] { copper };
						}
						return choices.Take(1);
					}
				case CardChoiceType.TrashForNobleBrigand:
					{
						CardModel gold = choices.FirstOrDefault(c => c is Gold);
						if (gold != null)
						{
							return new CardModel[] { gold };
						}
						return choices.Take(1);
					}
				case CardChoiceType.TrashForUpgrade:
				case CardChoiceType.TrashForRemake:
					{
						int pcost = this.gameModel.PileMap[typeof(Province)].GetCost();
						CardModel toProvince = choices.FirstOrDefault(c => this.gameModel.GetCost(c) + 1 == pcost);
						if (toProvince != null)
						{
							return new CardModel[] { toProvince };
						}
						CardModel estate = choices.FirstOrDefault(c => c is Estate);
						if (estate != null)
						{
							return new CardModel[] { estate };
						}
						CardModel bad = choices.FirstOrDefault(c => c is Curse || c.Is(CardType.Ruins));
						if (bad != null)
						{
							return new CardModel[] { bad };
						}
						CardModel copper = choices.FirstOrDefault(c => c is Copper);
						if (copper != null)
						{
							return new CardModel[] { copper };
						}
						return choices.Take(1);
					}
				case CardChoiceType.TrashForGovernor:
					{						
						Pile colony = null;
						if (this.gameModel.PileMap.TryGetValue(typeof(Colony), out colony))
						{
							int ccost = colony.GetCost();
							CardModel toColony = choices.FirstOrDefault(c => this.gameModel.GetCost(c) + 2 == ccost);
							if (toColony != null)
							{
								return new CardModel[] { toColony };
							}
						}
						int pcost = this.gameModel.PileMap[typeof(Province)].GetCost();
						CardModel toProvince = choices.FirstOrDefault(c => this.gameModel.GetCost(c) + 2 == pcost);
						if (toProvince != null)
						{
							return new CardModel[] { toProvince };
						}
						CardModel estate = choices.FirstOrDefault(c => c is Estate);
						if (estate != null)
						{
							return new CardModel[] { estate };
						}
						CardModel bad = choices.FirstOrDefault(c => c is Curse || c.Is(CardType.Ruins));
						if (bad != null)
						{
							return new CardModel[] { bad };
						}
						CardModel copper = choices.FirstOrDefault(c => c is Copper);
						if (copper != null)
						{
							return new CardModel[] { copper };
						}

						return choices.Take(1);
					}
				case CardChoiceType.TrashForRemodel:
					{
						Pile colony = null;
						if (this.gameModel.PileMap.TryGetValue(typeof(Colony), out colony))
						{
							int ccost = colony.GetCost();
							CardModel toColony = choices.FirstOrDefault(c => this.gameModel.GetCost(c) + 2 >= ccost && !(c is Colony));
							if (toColony != null)
							{
								return new CardModel[] { toColony };
							}
						}
						int pcost = this.gameModel.PileMap[typeof(Province)].GetCost();
						CardModel toProvince = choices.FirstOrDefault(c => this.gameModel.GetCost(c) + 2 >= pcost && !(c is Province) && !(c is Colony));
						if (toProvince != null)
						{
							return new CardModel[] { toProvince };
						}
						CardModel estate = choices.FirstOrDefault(c => c is Estate);
						if (estate != null)
						{
							return new CardModel[] { estate };
						}
						CardModel bad = choices.FirstOrDefault(c => c is Curse || c.Is(CardType.Ruins));
						if (bad != null)
						{
							return new CardModel[] { bad };
						}
						return choices.Take(1);
					}
				case CardChoiceType.TrashForSalvager:
					{
						CardModel estate = choices.FirstOrDefault(c => c is Estate);
						if (estate != null)
						{
							return new CardModel[] { estate };
						}
						CardModel bad = choices.FirstOrDefault(c => c is Curse || c.Is(CardType.Ruins) || c.Is(CardType.Shelter));
						if (bad != null)
						{
							return new CardModel[] { bad };
						}
						return choices.Where(c => !c.Is(CardType.Victory)).OrderByDescending(c => this.gameModel.GetCost(c)).Take(1);
					}
				case CardChoiceType.TrashForThief:
					{
						return choices.OrderByDescending(c => c.GetVictoryPoints(this.Player)).ThenByDescending(c => this.gameModel.GetCost(c)).Take(1);
					}
				case CardChoiceType.TrashForTrader:
					{
						CardModel estate = choices.FirstOrDefault(c => c is Estate);
						if (estate != null)
						{
							return new CardModel[] { estate };
						}
						CardModel bad = choices.FirstOrDefault(c => c is Curse || c.Is(CardType.Ruins) || c.Is(CardType.Shelter));
						if (bad != null)
						{
							return new CardModel[] { bad };
						}
						CardModel copper = choices.FirstOrDefault(c => c is Copper);
						if (copper != null)
						{
							return new CardModel[] { copper };
						}
						return choices.OrderBy(c => c.GetVictoryPoints(this.Player)).ThenByDescending(c => this.gameModel.GetCost(c)).Take(1);
					}
				case CardChoiceType.TrashForTransmute:
					{
						IEnumerable<CardModel> targets = choices.Where(c => c is Estate);
						if (targets.Count() > 0)
						{
							return targets.Take(1);
						}
						targets = choices.Where(c => c.Is(CardType.Shelter));
						if (targets.Count() > 0)
						{
							return targets.Take(1);
						}
						targets = choices.Where(c => c is GreatHall);
						if (targets.Count() > 0)
						{
							return targets.Take(1);
						}
						targets = choices.Where(c => c is Copper);
						if (targets.Count() > 0)
						{
							return targets.Take(1);
						}
						targets = choices.Where(c => c is Transmute);
						if (targets.Count() > 0)
						{
							return targets.Take(1);
						}
						return cardsBadToGoodGain.Take(minChoices);
					}
				case CardChoiceType.PutOnDeckFromPlayed:
					if (this.Player.Played.Any(c => c is Alchemist))
					{
						IEnumerable<CardModel> potions = choices.Where(c => c is Potion);
						if (potions.Any())
						{
							return potions.Take(1);
						}
					}
					IEnumerable<CardModel> goodTreasures = choices.Where(c => !(c is Copper));
					if (goodTreasures.Any())
					{
						return goodTreasures.Take(1);
					}
					return choices.Take(0);
				case CardChoiceType.PutOnDeckFromHand:
					if (this.isSecretChamberActive && cardInfo is SecretChamber)
					{
						List<CardModel> mustKeep = new List<CardModel>();
						this.isSecretChamberActive = false;
						if (this.attackTriggeringSecretChamber is Ambassador ||
							this.attackTriggeringSecretChamber is Familiar ||
							this.attackTriggeringSecretChamber is Soothsayer ||
							this.attackTriggeringSecretChamber is Marauder ||
							this.attackTriggeringSecretChamber is Torturer || 
							this.attackTriggeringSecretChamber is Margrave ||
							this.attackTriggeringSecretChamber is Witch ||
							this.attackTriggeringSecretChamber is YoungWitch ||
							this.attackTriggeringSecretChamber is Cultist)
						{
							CardModel keep = null;
							if (!this.hasUsedMoatThisTurn)
							{
								CardModel moat = choices.FirstOrDefault(c => c is Moat);
								if (moat != null)
								{
									keep = moat;
								}
								else
								{
									if (this.attackTriggeringSecretChamber is YoungWitch)
									{
										CardModel bane = choices.FirstOrDefault(card => card.GetType() == gameModel.Bane.GetType());
										if (bane != null)
										{
											keep = bane;
										}
									}
								}
							}
							return this.PickCardsToPutOnDeck(choices, minChoices, keep != null ? new CardModel[] { keep } : null, null);
						}

						// manipulate top
						if (this.attackTriggeringSecretChamber is Jester)
						{
							CardModel top = null;
							if(this.gameModel.PileMap[typeof(Curse)].Count == 0)
							{
								top = choices.FirstOrDefault(c => c is Curse);
								if(top == null)
								{
									top = choices.FirstOrDefault(c => c.Is(CardType.Victory) && !c.Is(CardType.Action) && !c.Is(CardType.Treasure));
								}
							}
							if(top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Shelter));
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c is Copper);
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Action) && c.GetBaseCost() <= 3 && !c.Is(CardType.Victory));
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c is Silver);
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => !c.Is(CardType.Curse) && !c.Is(CardType.Victory));
							}
							return this.PickCardsToPutOnDeck(choices, minChoices, null, top);							
						}
						if (this.attackTriggeringSecretChamber is SeaHag)
						{
							// put a shitty card
							CardModel top = null;
							top = choices.FirstOrDefault(c => c is Curse);
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Victory) && !c.Is(CardType.Action) && !c.Is(CardType.Treasure));
							}							
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Shelter));
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c is Copper);
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Action) && c.GetBaseCost() <= 3 && !c.Is(CardType.Victory));
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c is Silver);
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => !c.Is(CardType.Curse) && !c.Is(CardType.Victory));
							}
							return this.PickCardsToPutOnDeck(choices, minChoices, null, top);	
						}

						if (this.attackTriggeringSecretChamber is Swindler)
						{
							// put a shitty card
							CardModel top = null;
							top = choices.FirstOrDefault(c => c is Curse);
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Victory) && !c.Is(CardType.Action) && !c.Is(CardType.Treasure));
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Shelter));
							}
							
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Action) && c.GetBaseCost() <= 3 && !c.Is(CardType.Victory));
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c is Silver);
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c is Copper);
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => !c.Is(CardType.Curse) && !c.Is(CardType.Victory));
							}
							return this.PickCardsToPutOnDeck(choices, minChoices, null, top);	
						}

						if(this.attackTriggeringSecretChamber is Saboteur)
						{
							CardModel top = null;
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Action) && c.GetBaseCost() == 3 && !c.Is(CardType.Victory));
							}
							
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c is Silver);
							}
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Action) && c.GetBaseCost() >= 3 && c.GetBaseCost() < 6 && !c.Is(CardType.Victory));
							}
							if(top == null)
							{
								top = choices.FirstOrDefault(c => !c.Is(CardType.Victory));
							}
							return this.PickCardsToPutOnDeck(choices, minChoices, null, top);	
						}

						if(this.attackTriggeringSecretChamber is PirateShip)
						{
							CardModel top = null;
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Curse) || c.Is(CardType.Victory) && !c.Is(CardType.Action) && !c.Is(CardType.Treasure));
							}

							IEnumerable<CardModel> keep = choices.Where(c => c.Is(CardType.Treasure));
							return this.PickCardsToPutOnDeck(choices, minChoices, keep, top);
						}

						if (this.attackTriggeringSecretChamber is Thief ||
							this.attackTriggeringSecretChamber is NobleBrigand)
						{
							CardModel top = choices.FirstOrDefault(c => c is Copper);
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Curse) || c.Is(CardType.Victory) && !c.Is(CardType.Action) && !c.Is(CardType.Treasure));
							}

							IEnumerable<CardModel> keep = choices.Where(c => c.Is(CardType.Treasure) && !(c is Copper));
							return this.PickCardsToPutOnDeck(choices, minChoices, keep, top);
						}

						if(this.attackTriggeringSecretChamber is FortuneTeller)
						{
							CardModel top = choices.FirstOrDefault(c => c.Is(CardType.Victory) || c is Curse);
							return this.PickCardsToPutOnDeck(choices, minChoices, null, top);
						}
						if (this.attackTriggeringSecretChamber is ScryingPool ||
							this.attackTriggeringSecretChamber is Spy)
						{
							return this.PickCardsToPutOnDeck(choices, minChoices, null, null);
						}
						
						if(this.attackTriggeringSecretChamber is Rogue ||
							this.attackTriggeringSecretChamber is Knights)
						{
							CardModel top = choices.FirstOrDefault(c => c is Copper);
							if (top == null)
							{
								top = choices.FirstOrDefault(c => c.Is(CardType.Curse) || c.Is(CardType.Victory) && !c.Is(CardType.Action) && !c.Is(CardType.Treasure));
							}

							IEnumerable<CardModel> keep = choices.Where(c => c.GetBaseCost() >= 3 && c.GetBaseCost() <= 6);
							return this.PickCardsToPutOnDeck(choices, minChoices, keep, top);
						}

						if (this.attackTriggeringSecretChamber is Oracle)
						{
							return this.PickCardsToPutOnDeck(choices, minChoices, null, null);
						}
						if (this.attackTriggeringSecretChamber is Rabble)
						{
							IEnumerable<CardModel> keep = choices.Where(c => c.Is(CardType.Victory));
							
							return this.PickCardsToPutOnDeck(choices, minChoices, keep, null);
						}

						if(this.attackTriggeringSecretChamber is Bureaucrat)
						{
							if(choices.Count(c => c.Is(CardType.Victory)) == 2)
							{
								return choices.Where(c => c.Is(CardType.Victory));
							}
							CardModel top = choices.FirstOrDefault(c => c.Is(CardType.Victory));
							return this.PickCardsToPutOnDeck(choices, minChoices, null, top);
						}
						
						if (this.attackTriggeringSecretChamber is Cutpurse)
						{
							return this.PickCardsToPutOnDeck(choices, minChoices, null, null);
						}						
						
						if (this.attackTriggeringSecretChamber is Taxman)
						{
							return this.PickCardsToPutOnDeck(choices, minChoices, null, null);
						}

						if(this.attackTriggeringSecretChamber is Mountebank)
						{
							IEnumerable<CardModel> curses = choices.Where(c => c is Curse);
							if(curses.Count() > 1)
							{
								curses = curses.Take(1);
							}
							return this.PickCardsToPutOnDeck(choices, minChoices, curses, null);
						}
						if (this.attackTriggeringSecretChamber is Urchin)
						{
							CardModel badCard = choices.FirstOrDefault(c => c is Curse || c.IsPureVictory || c.Is(CardType.Ruins) || c.Is(CardType.Shelter));
							if(badCard == null)
							{
								badCard = choices.FirstOrDefault(c => c is Copper);
							}
							return this.PickCardsToPutOnDeck(choices, minChoices, new CardModel[] {badCard}, null);
						}
						if(this.attackTriggeringSecretChamber is GhostShip ||
						this.attackTriggeringSecretChamber is Goons ||
						this.attackTriggeringSecretChamber is Militia)
						{
							IEnumerable<CardModel> badCards = choices.Where(c => c is Curse || c.IsPureVictory || c.Is(CardType.Ruins) || c.Is(CardType.Shelter));
							if(badCards.Count() > 2)
							{
								badCards = badCards.Take(2);
							}
							else
							{
								badCards = badCards.Concat(choices.Where(c => c is Copper));
							}
							if (badCards.Count() > 2)
							{
								badCards = badCards.Take(2);
							}
							return this.PickCardsToPutOnDeck(choices, minChoices,badCards, null);
						}

						if (this.attackTriggeringSecretChamber is Minion)
						{
							if(cardsGoodToPlay.Count() >= 2)
							{
								return cardsGoodToPlay.Take(2);
							}
							else
							{
								return cardsGoodToBadGain.Take(2);
							}
						}						 
					}
					return choices.Take(minChoices);
				case CardChoiceType.PutOnDeckFromDiscard:
					{
						IEnumerable<CardModel> cards = choices.Where(c => c.Is(CardType.Action) || c.Is(CardType.Treasure)).OrderByDescending(c => c.GetBaseCost());
						if (cards.Any())
						{
							return cards.Take(1);
						}
						else
						{
							return choices.Take(1);
						}
					}
				case CardChoiceType.DiscardForCellar:
					{
						bool takeCopper = this.Player.AllCardsInDeck.Count(c => c is Silver || c is Gold || c is Platinum) > 3;
						return choices.Where(c => !c.Is(CardType.Action) && !c.Is(CardType.Treasure) || takeCopper && c is Copper);
					}
				case CardChoiceType.DiscardForCards:
					{
						bool takeCopper = this.Player.AllCardsInDeck.Count(c => c is Silver || c is Gold || c is Platinum) > 3;
						bool takeActions = this.Player.Actions > 0;
						return choices.Where(c => !c.Is(CardType.Action) && !c.Is(CardType.Treasure) || takeCopper && c is Copper || takeActions && c.Is(CardType.Action));
					}
				case CardChoiceType.DiscardForCoins:
					{
						bool takeActions = this.Player.Actions > 0;
						return choices.Where(c => !c.Is(CardType.Action) && !c.Is(CardType.Treasure) || c is Copper || takeActions && c.Is(CardType.Action));
					}
				case CardChoiceType.ReactToAttack:
					{
						// possibilities are Moat, SecretChamber, HorseTraders, Beggar

						// always use moat, beggar, horse traders
						CardModel always = choices.FirstOrDefault(c => c is Moat || c is Beggar || c is HorseTraders);
						if (always != null)
						{
							if (always is Moat)
							{
								this.hasUsedMoatThisTurn = true;
							}
							return new CardModel[] { always };
						}

						CardModel secretChamber = choices.FirstOrDefault(c => c is SecretChamber);
						if (secretChamber != null)
						{
							if (!this.hasUsedSecretChamberThisTurn)
							{
								this.hasUsedSecretChamberThisTurn = true;
								this.isSecretChamberActive = true;
								this.attackTriggeringSecretChamber = cardInfo;
								return new CardModel[] { secretChamber };
							}
						}

						// nothing left but secret chambers.
						Debug.Assert(choices.All(c => c is SecretChamber));
						return choices.Take(0);
					}
				case CardChoiceType.ReactToGain:
					{
						// need to know what is the gain
						return choices.Take(1);
					}
				case CardChoiceType.NameACardForRebuild:
					{
						return choices.Where(c => c is Province).Take(1);
					}
				case CardChoiceType.NameACardToDraw:
					{
						return choices.Where(c => c is Copper).Take(1);
					}

				case CardChoiceType.Ambassador:
					{
						IEnumerable<CardModel> targets = choices.Where(c => c is Curse);
						if (targets.Count() > 0)
						{
							return targets.Take(1);
						}
						targets = choices.Where(c => c is Estate);
						if (targets.Count() > 0)
						{
							return targets.Take(1);
						}
						targets = choices.Where(c => c is Copper);
						if (targets.Count() > 0)
						{
							return targets.Take(1);
						}
						targets = choices.Where(c => !c.Is(CardType.Victory)).OrderBy(c => c.GetBaseCost());
						if (targets.Count() > 0)
						{
							return targets.Take(1);
						}
						return choices.OrderBy(c => c.GetBaseCost()).Take(1);
					}
				case CardChoiceType.BlackMarket:
					{
						foreach (CardModel card in choices)
						{
							if (this.Player.Hand.Contains(card))
							{
								return new CardModel[] { card };
							}
						}
						Comparer<CardModel> comp = this.GetCardGainComparer(gameSegment);
						return choices.OrderBy(c => c, comp).Reverse().Take(1);
					}
				case CardChoiceType.Contraband:
					return choices.Where(c => this.GetGameSegment() == GameSegment.Endgame ? c is Province : c is Gold).Take(1);
				case CardChoiceType.CountingHouse:
					return choices;
				case CardChoiceType.Embargo:
					return choices.OrderBy(c => Randomizer.Next()).Take(1);
				case CardChoiceType.FoolsGold:
					{
						int foolsGoldCount = this.Player.Hand.Count(c => c is FoolsGold);
						int coin = this.Player.Hand.Sum(c => c.Is(CardType.Treasure) ? c.Coins : 0);
						int oldFoolsGoldCoin = 1 + (foolsGoldCount - 1) * 4;
						if (coin + oldFoolsGoldCoin >= 8)
						{
							for(int i=0;i<=foolsGoldCount;i++)
							{
								int newFoolsGoldCoin = i > 0 ? 1 + (i - 1) * 4 : 0;
								if(coin + newFoolsGoldCoin >= 8)
								{
									return choices.Take(choices.Count() - i);
								}
							}
						}
						return choices;
					}
				case CardChoiceType.Masquerade:
					return cardsBadToGoodGain.Take(1);
				case CardChoiceType.Inn:
					return choices.Take(maxChoices);
				case CardChoiceType.Island:
					{
						CardModel vp = choices.FirstOrDefault(c => c.Is(CardType.Victory) && !c.Is(CardType.Action) && !c.Is(CardType.Treasure));
						if (vp != null) return new CardModel[] { vp };
						CardModel bad = choices.FirstOrDefault(c => c.Is(CardType.Curse) || c.Is(CardType.Ruins) || c.Is(CardType.Shelter));
						if (bad != null) return new CardModel[] { bad };
						return cardsBadToGoodGain.Take(1);
					}
				case CardChoiceType.Tunnel:
					return choices.Take(maxChoices);

				case CardChoiceType.BandOfMisfits:
					return cardsGoodToPlay.Take(1);
				case CardChoiceType.Haven:
					return cardsGoodToBadGain.Take(1);
				case CardChoiceType.Scheme:
					CardModel notYetSchemed = cardsGoodToPlay.FirstOrDefault(c => !this.Player.Schemed.Contains(c));
					if (notYetSchemed != null) return new CardModel[] { notYetSchemed };
					return choices.Take(1);
				case CardChoiceType.Prince:
					{
						IEnumerable<CardModel> targets = this.Player.Hand.Where(c => this.gameModel.GetCost(c) <= 4 && !c.CostsPotion && c.Is(CardType.Action) && !c.Is(CardType.Duration));
						if (targets.Any())
						{
							return targets.OrderByDescending(c => c.CardPriority.MultiplePlayPriority).Take(1);
						}
						else
						{
							return targets.Take(0);
						}
					}
				case CardChoiceType.PrincePlayOrder:
					return choices.OrderByDescending(c => c.CardPriority.PlayPriority).Take(1);
				default:
					//Debug.Assert(false, "Unhandled choice type in ChooseCards: " + choiceType.ToString());
					return choices.Take(minChoices);
			}
		}

		public override IEnumerable<Pile> ChoosePiles(CardChoiceType choiceType, string choiceText, int minChoices, int maxChoices, IEnumerable<Pile> choices)
		{
			return this.ChooseCards(choiceType, null, choiceText, ChoiceSource.FromPile, minChoices, maxChoices, choices.Select(p => p.TopCard)).Select(c => choices.First(p => p.TopCard == c));
		}

		public override IEnumerable<int> ChooseEffects(EffectChoiceType choiceType, IEnumerable<CardModel> cardInfo, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions)
		{
			int singleChoice;
			switch (choiceType)
			{
				case EffectChoiceType.TrashForLoan:
					singleChoice = cardInfo.First() is Copper ?  0 : 1;
					break;
					 
				case EffectChoiceType.TrashForMercenary:
					singleChoice = 1;
					if (this.Player.Hand.Count(c => c is Copper || c is Curse || c.Is(CardType.Ruins) || c is Estate || c.Is(CardType.Shelter)) >= 2)
					{
						singleChoice = 0;
					}
					break;
				case EffectChoiceType.TrashForSpiceMerchant:
					if (this.Player.Hand.Any(c => c is Copper || c is Loan))
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.TrashMiningVillage:
					int coinSum = this.Player.Hand.Sum(c => c.Is(CardType.Treasure) ? c.Coins : 0);
					if (coinSum >= 6 && coinSum < 8)
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.TrashUrchin:
					singleChoice = 0;
					break;
				case EffectChoiceType.GainDuchess:
					singleChoice = 0;
					break;
				case EffectChoiceType.GainForJester:
					CardModel jestedCard = cardInfo.First();
					if (jestedCard is Curse || jestedCard is Copper || jestedCard.Is(CardType.Ruins))
					{
						singleChoice = 1;
					}
					else
					{
						singleChoice = 0;
					}
					break;
				case EffectChoiceType.GainForHuntingGrounds:
					if (this.gameModel.PileMap[typeof(Duchy)].Count == 0)
					{
						singleChoice = 1;
					}
					else
					{
						singleChoice = 0;
					}
					break;
				case EffectChoiceType.Cultist:
					singleChoice = 0;
					break;
				case EffectChoiceType.PutDeckInDiscard:
					singleChoice = 0;
					break;
				case EffectChoiceType.DiscardOrPutOnDeck:
					singleChoice = KeepOrDiscard(cardInfo, treatActionsAsDeadCards: false) ? 0 : 1;
					break;
				case EffectChoiceType.DiscardOrPutOnDeckToDraw:
					singleChoice = KeepOrDiscard(cardInfo, treatActionsAsDeadCards: this.Player.Actions == 0) ? 0 : 1;
					break;
				case EffectChoiceType.ForceDiscardOrPutOnDeck:
					singleChoice = KeepOrDiscard(cardInfo, treatActionsAsDeadCards: false) ? 1 : 0;
					break;
				case EffectChoiceType.DiscardForHamletAction:
					if (this.Player.Hand.Any(c => !c.Is(CardType.Treasure) && !c.Is(CardType.Action)))
					{
						singleChoice = 0;
					}
					else if (this.Player.Actions == 1)
					{
						int total = this.Player.Hand.Sum(c => c.Is(CardType.Action) ? c.Actions - 1 : 0);
						if (total < -1)
						{
							singleChoice = 0;
						}
						else if (total <= 0 && this.Player.Hand.Any(c => c.Is(CardType.Action) && c.Actions == 0 && c.Cards > 0))
						{
							singleChoice = 0;
						}
						else
						{
							singleChoice = 1;
						}
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.DiscardForHamletBuy:
					if(this.Player.Hand.Any(c => !c.Is(CardType.Treasure) && !c.Is(CardType.Action)))
					{
						singleChoice = 0;
					}
					else if (this.Player.Coin + this.TreasureCoin(this.Player.Hand) > 12 && this.Player.Buys == 1)
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.DiscardBeggar:
					singleChoice = 0;
					break;
				case EffectChoiceType.DiscardForStables:
					singleChoice = this.Player.Hand.Any(c => c is Copper || c is Silver) ? 0 : 1;
					break;
				case EffectChoiceType.DiscardForBaron:
					singleChoice = 0;
					break;
				case EffectChoiceType.DiscardForMountebank:
					if (this.gameModel.PileMap[typeof(Curse)].Count == 0 && this.Player.Hand.Any(c => c is Ambassador) ||
						this.Player.Hand.Any(c => c is Trader || c is Watchtower))
					{
						singleChoice = 1;
					}
					else
					{
						singleChoice = 0;
					}
					break;
				case EffectChoiceType.DiscardMarketSquare:
					singleChoice = 0;
					break;
				case EffectChoiceType.SetAsideForLibrary:
					if (this.Player.Actions == 0)
					{
						singleChoice = 1;
					}
					else
					{
						singleChoice = 0;
					}
					break;
				case EffectChoiceType.RevealForTournament:
					singleChoice = 0;
					break;
				case EffectChoiceType.RevealBane:
					singleChoice = 0;
					break;
				case EffectChoiceType.TrustySteed:
					if (this.Player.Actions <= 1)
					{
						return new int[] { 0, 2 };
					}
					else
					{
						return new int[] { 0, 1 };
					}
				case EffectChoiceType.CountEffect1:
					if (this.Player.Hand.All(c => c.Is(CardType.Ruins) || c is Curse || c is Copper || c is Estate || c.Is(CardType.Shelter)))
					{
						singleChoice = 1;
					}
					else if (this.Player.Hand.Count(c => c.Is(CardType.Ruins) || c is Curse || c is Copper || c is Estate || c.Is(CardType.Shelter)) >= 2)
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 2;
					}
					break;
				case EffectChoiceType.CountEffect2:
					if (this.Player.Hand.All(c => c.Is(CardType.Ruins) || c is Curse || c is Copper || c is Estate || c.Is(CardType.Shelter)))
					{
						singleChoice = 1;
					}
					int coin = this.Player.Coin + this.TreasureCoin(this.Player.Hand);
					if (coin >= 5 && coin < 8)
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 2;
					}
					break;
				case EffectChoiceType.Graverobber:
					if (this.Player.Hand.Any(c => c.Is(CardType.Action) && this.gameModel.GetCost(c) >= 5))
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.Squire:
					if (this.Player.Actions == 0 && this.Player.Hand.Any(c => c.Is(CardType.Action)))
					{
						singleChoice = 0;
					}
					else if (this.Player.Buys == 1 && this.Player.Coin + this.TreasureCoin(this.Player.Hand) > 8)
					{
						singleChoice = 1;
					}
					else
					{
						singleChoice = 2;
					}
					break;
				case EffectChoiceType.IllGottenGains:
					if (this.Player.Coin + this.TreasureCoin(this.Player.Hand) + this.Player.Hand.Count(c => c is IllGottenGains) == 7)
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.SpiceMerchantEffect:
					if (this.Player.Hand.Any(c => c.Is(CardType.Action)) && this.Player.Actions == 0)
					{
						singleChoice = 0;
					}
					else if (this.Player.Coin + this.TreasureCoin(this.Player.Hand) > 5)
					{
						singleChoice = 1;
					}
					else
					{
						singleChoice = 0;
					}
					break;
				case EffectChoiceType.Minion:
					if (this.Player.Hand.Any(c => c is Minion))
					{
						singleChoice = 0;
					}
					else
					{
						int coinTotal = this.Player.Coin + this.TreasureCoin(this.Player.Hand);
						if ((coinTotal == 6 || coinTotal == 7) && !this.Player.Hand.Any(c => c.Is(CardType.Action)))
						{
							singleChoice = 0;
						}
						else
						{
							singleChoice = 1;
						}
					}
					break;
				case EffectChoiceType.Nobles:
					if (this.Player.Actions == 0 && this.Player.Hand.Any(c => c.Is(CardType.Action)))
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.Pawn:
					bool wantAction = this.Player.Actions == 0 && this.Player.Hand.Any(c => c.Is(CardType.Action));
					bool wantBuy = this.Player.Coin + this.TreasureCoin(this.Player.Hand) > 8;
					bool wantCard = this.Player.Actions == 0 && !wantAction && this.Player.Deck.Count(c => c.Is(CardType.Action)) * 2 < this.Player.Deck.Count;
					if (wantAction && wantBuy)
					{
						return new int[] { 2, 3 };
					}
					else if (wantAction && wantCard)
					{
						return new int[] { 0, 2 };
					}
					else if (wantBuy && wantCard)
					{
						return new int[] { 0, 3 };
					}
					else if (wantAction)
					{
						return new int[] { 1, 2 };
					}
					else if (wantBuy)
					{
						return new int[] { 1, 3 };
					}
					else if (wantCard)
					{
						return new int[] { 0, 1 };
					}
					else
					{
						return new int[] { 0, 2 };
					}
				case EffectChoiceType.Steward:
					if (this.Player.Hand.Count(c => c.Is(CardType.Ruins) || c is Curse || c is Estate || c.Is(CardType.Shelter)) >= 1 &&
						this.Player.Hand.Count(c => c.Is(CardType.Ruins) || c is Curse || c is Estate || c.Is(CardType.Shelter) || c is Copper) >= 2 && 
						this.GetGameSegment() != GameSegment.Endgame)
					{
						singleChoice = 2;
					}
					else if (this.Player.Actions > 0 && this.Player.Deck.Count > 0)
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.Torturer:
					if (this.gameModel.PileMap[typeof(Curse)].Count == 0 || this.Player.Hand.Any(c => this.IsInHandTrasher(c)))
					{
						singleChoice = 1;
					}
					else if (this.gameModel.LeftOfCurrentPlayer.Actions == 0 && this.Player.Hand.Count == 5)
					{
						singleChoice = 0;
					}
					else if (this.Player.Hand.Count(c => !c.Is(CardType.Action) && !c.Is(CardType.Treasure)) >= 2)
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.Governor:
					if (this.Player.Hand.Any(c => this.gameModel.GetCost(c) == 6))
					{
						singleChoice = 2;
					}
					else if (this.Player.AllCardsInDeck.Count(c => c is Gold) < 4)
					{
						singleChoice = 1;
					}
					else
					{
						singleChoice = 0;
					}
					break;
				case EffectChoiceType.Vault:
					int badCardCount = this.Player.Hand.Count(c => !c.Is(CardType.Treasure) && !c.Is(CardType.Action));
					bool tunnels = this.Player.Hand.Any(c => c is Tunnel);
					int coppers = this.Player.Hand.Count(c => c is Copper);
					if (tunnels || badCardCount >= 2 || badCardCount >= 1 && coppers >= 1)
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.Prince:
					IEnumerable<CardModel> targets = this.Player.Hand.Where(c => this.gameModel.GetCost(c) <= 4 && !c.CostsPotion && c.Is(CardType.Action) && !c.Is(CardType.Duration));
					if(targets.Any())
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.AmbassadorCount:
					CardModel ambassadoredCard = cardInfo.Single();
					if (ambassadoredCard is Estate || ambassadoredCard is Curse)
					{
						singleChoice = choices.Length - 1;
					}
					int tCoin = this.TreasureCoin(this.Player.AllCardsInDeck);
					if(ambassadoredCard is Copper)
					{
						if (tCoin == 3)
						{
							singleChoice = 0;
						}
						else if (tCoin == 4)
						{
							singleChoice = 1;
						}
						else
						{
							singleChoice = choices.Length - 1;
						}
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.Explorer:
					singleChoice = 0;
					break;
				case EffectChoiceType.NativeVillage:
					if (this.Player.NativeVillageMat.Count < 3 || this.Player.Hand.Any(c => c is NativeVillage))
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.PearlDiver:
					CardModel bottomCard = cardInfo.Single();
					if (!bottomCard.Is(CardType.Action) && !bottomCard.Is(CardType.Treasure))
					{
						singleChoice = 1;
					}
					else if (bottomCard is Copper)
					{
						singleChoice = 1;
					}
					else
					{
						singleChoice = 0;
					}
					break;
				case EffectChoiceType.PirateShip:
					if (this.Player.PirateShipTokens < 4)
					{
						singleChoice = 0;
					}
					else
					{
						singleChoice = 1;
					}
					break;
				case EffectChoiceType.Hovel:
					singleChoice = 0;
					break;
				case EffectChoiceType.Watchtower:
				case EffectChoiceType.RoyalSeal:
					CardModel gainedCard = cardInfo.Single();
					if (!gainedCard.Is(CardType.Action) && !gainedCard.Is(CardType.Treasure))
					{
						singleChoice = 1;
					}
					else if (gainedCard is Copper)
					{
						singleChoice = 1;
					}
					else
					{
						singleChoice = 0;
					}
					break;
				case EffectChoiceType.MasterpiecePay:
					singleChoice = choices.Count() - 1;
					break;
				case EffectChoiceType.HeraldPay:
					singleChoice = 0;
					break;
				case EffectChoiceType.DoctorPay:
					singleChoice = Math.Min(3, choices.Count());
					break;
				case EffectChoiceType.DoctorEffect:
					CardModel doctorTarget = cardInfo.First();
					if (doctorTarget.Is(CardType.Ruins) || doctorTarget is Curse || doctorTarget is Estate || doctorTarget.Is(CardType.Shelter) || doctorTarget is Copper)
					{ 
						// trash
						singleChoice = 0;
					}
					else if (!doctorTarget.Is(CardType.Treasure) && !doctorTarget.Is(CardType.Action))
					{
						// discard
						singleChoice = 1;
					}
					else
					{
						singleChoice = 2;
					}
					break;
				case EffectChoiceType.StonemasonPay:
					// gain two action cards each costing the amount you overpaid
					singleChoice = 0;
					break;
				case EffectChoiceType.PlayCoinTokens:
					singleChoice = choices.Length - 1;
					break;
				default:
					Debug.Assert(false, "Unhandled choice type in ChooseEffects: " + choiceType.ToString());					
					List<int> ret = new List<int>();
					for (int i = 0; i < minChoices; i++)
					{
						ret.Add(i);
					}
					return ret;
			}
			return new int[] { singleChoice };
		}

		private IEnumerable<CardModel> PickCardsToPutOnDeck(IEnumerable<CardModel> cards, int countToPutBack, IEnumerable<CardModel> keep, CardModel top)
		{
			IEnumerable<CardModel> putBack = cards.Where(c => (keep == null || !keep.Contains(c)) && top == c);
			putBack = putBack.Concat(cards.Where(c => (keep == null || !keep.Contains(c)) && top != c));
			if (putBack.Count() >= countToPutBack)
			{
				return putBack.Take(countToPutBack);
			}
			int need = countToPutBack - putBack.Count();
			IEnumerable<CardModel> next = putBack.Concat(cards.Where(c => !putBack.Contains(c)));
			if(need > next.Count())
			{
				IEnumerable<CardModel> final = next.Concat(cards.Where(c => !next.Contains(c)));
				return final.Take(countToPutBack);
			}
			else return next.Take(countToPutBack);
		}

		private bool IsInHandTrasher(CardModel cardModel)
		{
			return cardModel is Ambassador || cardModel is Chapel || cardModel is Trader || cardModel is Watchtower || cardModel is Apprentice ||
				cardModel is Transmute || cardModel is Remake || cardModel is Altar || cardModel is JunkDealer || cardModel is Forager || cardModel is Mercenary ||
				cardModel is Rats || cardModel is Develop || cardModel is Remodel || cardModel is JackOfAllTrades || cardModel is Masquerade || cardModel is TradingPost ||
				cardModel is Upgrade || cardModel is Bishop || cardModel is Forge || cardModel is Expand || cardModel is Salvager;
		}

		private int TreasureCoin(IEnumerable<CardModel> hand)
		{
			return hand.Sum(c => c.Is(CardType.Treasure) ? c.Coins : 0);
		}

		private bool KeepOrDiscard(IEnumerable<CardModel> cards, bool treatActionsAsDeadCards)
		{
			int good = 0;
			int bad = 0;
			int ok = 0;
			foreach (CardModel card in cards)
			{
				if (card is Curse || card.Is(CardType.Ruins) || (card.Is(CardType.Victory) && !card.Is(CardType.Action) && !card.Is(CardType.Treasure)))
				{
					bad++;
				}
				else if(card.Is(CardType.Action) && treatActionsAsDeadCards)
				{
					bad++;
				}
				else if (card is Copper)
				{
					ok++;
				}
				else
				{
					good++;
				}
			}
			if (good == 0 && bad > 0) { return false; }
			if (good == 0 && bad == 0) return true;
			return good + ok > bad;			
		}

		public override IEnumerable<CardModel> Order(CardOrderType choiceType, string choiceText, IEnumerable<CardModel> choices)
		{
			switch (choiceType)
			{
				case CardOrderType.OrderOnDeck:
				case CardOrderType.OrderInBlackMarket:
					return choices;
				case CardOrderType.Stash:
					return choices.OrderBy(c => c is Stash ? 0 : 1);
				default:
					//Debug.Assert(false, "Unhandled choice type in Order: " + choiceType.ToString());
					return choices;
			}
		}
	}

	public abstract class BaseAIStrategy : PlayerStrategy
	{
		private BaseAIChooser BaseAIChooser { get { return ((BaseAIChooser)this.Chooser); } }

		public BaseAIStrategy(GameModel gameModel, IChooser chooser)
			: base(gameModel, chooser)
		{
			this.BaseAIChooser.SetStrategy(this);
		}
 
		public virtual GameSegment GetGameSegment()
		{
			int buysToEndGame = 0;
			foreach (Pile pile in this.GameModel.SupplyPiles.OrderBy(p => p.Count).Take(3))
			{
				buysToEndGame += pile.Count;
			}
			Pile province;
			if (this.GameModel.PileMap.TryGetValue(typeof(Province), out province))
			{
				buysToEndGame = Math.Min(buysToEndGame, province.Count);
			}

			Pile colony;
			if (this.GameModel.PileMap.TryGetValue(typeof(Colony), out colony))
			{
				buysToEndGame = Math.Min(buysToEndGame, colony.Count);
			}

			int maxActionCount = 0;
			int maxCoins = 0;
			foreach (Player player in this.GameModel.Players)
			{
				maxActionCount = Math.Max(maxActionCount, player.AllCardsInDeck.Count(c => c.Is(CardType.Action)));
				maxCoins = Math.Max(maxCoins, player.AllCardsInDeck.Where(c => c.Is(CardType.Treasure)).Sum(c => c.Coins));
			}
			if (buysToEndGame < 4 || maxActionCount > 15 || maxCoins > 30)
			{
				return GameSegment.Endgame;
			}
			else if (buysToEndGame < 6 || maxActionCount > 9 || maxCoins > 24)
			{
				return GameSegment.Middle;
			}
			else
			{
				return GameSegment.Early;
			}
		}

		public override PlayerAction GetNextAction()
		{
			Player tempPlayer = this.Player;
			if (this.Player.GameModel.CurrentPlayer.IsPossessionTurn)
			{
				this.Player = this.Player.GameModel.CurrentPlayer;
			}
			try
			{
				switch (this.GameModel.CurrentPhase)
				{
					case GamePhase.Action:
						return this.ActionPhase();

					case GamePhase.Buy:
						return this.BuyPhase();

					case GamePhase.CleanUp:
						CardModel cleanup = this.Player.Cleanup.FirstOrDefault();
						if (cleanup != null)
						{
							return new PlayerAction() { Card = cleanup, ActionType = ActionType.CleanupCard };
						}
						return new PlayerAction() { ActionType = ActionType.EndTurn };
				}
				return new PlayerAction();
			}
			finally
			{
				this.Player = tempPlayer;
			}
		}

		public override string Name
		{
			get { return "Basic AI"; }
		}

		protected bool TryBuyCard(Type cardType, out PlayerAction playerAction)
		{
			Pile pile = null;
			if (this.GameModel.PileMap.TryGetValue(cardType, out pile))
			{
				if (this.Player.Coin >= pile.GetCost() && (this.Player.Potions > 0 || !pile.CostsPotion) && pile.Count > 0 && !pile.Contrabanded && pile.TopCard.CanBuy(this.Player))
				{
					playerAction = new PlayerAction() { ActionType = ActionType.BuyCard, Pile = pile };
					return true;
				}
			}
			playerAction = null;
			return false;
		}

		public bool CardToPlayHasNUpgradeTargets(IEnumerable<CardModel> hand, CardModel card, int targetCount)
		{			
			foreach (CardModel potentialUpgradeTarget in hand)
			{
				if (potentialUpgradeTarget != card)
				{
					if (potentialUpgradeTarget.GetBaseCost() < 3 ||
						this.GameModel.SupplyPiles.Any(p => p.GetCost() == this.GameModel.GetCost(potentialUpgradeTarget) + 1 && p.CostsPotion == potentialUpgradeTarget.CostsPotion && p.Count > 0))
					{
						// don't upgrade quality VP
						if (!(potentialUpgradeTarget is Province || potentialUpgradeTarget is Colony))
						{
							targetCount--;
							if (targetCount == 0)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public bool CardHasOnTrashBenefit(CardModel card)
		{
			return card is Fortress || card is Rats || card is Catacombs || card is Cultist || card is Feodum || card is HuntingGrounds || card is OvergrownEstate;
		}

		public bool CardToPlayHasNTrashTargets(IEnumerable<CardModel> hand, CardModel card, int targetCount)
		{
			foreach (CardModel potentialTrashTarget in hand)
			{
				if (potentialTrashTarget != card)
				{
					if (potentialTrashTarget is Estate || potentialTrashTarget.Is(CardType.Ruins) || potentialTrashTarget.Is(CardType.Shelter) || potentialTrashTarget is Copper || potentialTrashTarget is Curse || potentialTrashTarget is Rats && !(card is Rats) || potentialTrashTarget is Fortress)
					{
						targetCount--;
						if (targetCount == 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool ShouldPlayAction(IEnumerable<CardModel> hand, CardModel card)
		{
			// only play treasure map if we have another in hand
			if (card is TreasureMap)
			{
				if (hand.Count(c => c is TreasureMap) < 2)
				{
					return false;
				}
				else
				{
					return true;
				}
			}

			// don't play remake if we don't have two good targets
			if (card is Remake)
			{
				return CardToPlayHasNUpgradeTargets(hand, card, 2);
			}

			if (card is Upgrade)
			{
				return CardToPlayHasNUpgradeTargets(hand, card, 1);
			}

			// we'll assume chapel is worth playing even with only 1 trash target
			if (card is Chapel)
			{
				return CardToPlayHasNTrashTargets(hand, card, 1);
			}

			if (card is Altar || card is Forager || card is JunkDealer || card is Rats || card is TradeRoute)
			{
				//make sure we have a good trash target
				return CardToPlayHasNTrashTargets(hand, card, 1);
			}

			if (card is TradingPost)
			{
				return CardToPlayHasNTrashTargets(hand, card, 2);
			}

			// don't mint copper
			if (card is Mint)
			{
				return hand.Any(c => c.Is(CardType.Treasure) && !(c is Copper) && !(c is Loan));
			}

			if (card is Smugglers)
			{
				return Smugglers.SmugglerChoices(this.GameModel).Any();
			}

			// don't bother playing attacks with no effects
			if (card is SeaHag)
			{
				return this.GameModel.PileMap[typeof(Curse)].Count > 0;
			}

			if (card is Library)
			{
				return hand.Count() <= 7;
			}

			if (card is Watchtower)
			{
				return hand.Count() <= 6;
			}

			if (card is Moneylender)
			{
				return hand.Any(c => c is Copper);
			}

			if (card is PoorHouse)
			{
				return hand.Count(c => c.Is(CardType.Treasure)) < 4;
			}

			if (card is Crossroads)
			{
				return !this.GameModel.CurrentPlayer.Played.Any(c => c.Name == Crossroads.CrossroadsName) || hand.Any(c => c.Is(CardType.Victory));
			}

			if (card is SpiceMerchant)
			{
				return hand.Any(c => c is Copper || c is Loan || c is Silver);
			}

			if (card is Stables)
			{
				return hand.Any(c => c.Is(CardType.Treasure));
			}

			if (card is Outpost)
			{
				return !this.GameModel.CurrentPlayer.IsOutpostTurn;
			}

			if (card is Tactician)
			{
				return hand.Count() < 8 || hand.Sum(c => c.Is(CardType.Treasure) ? c.Coins : 0) < 8;
			}

			return true;
		}

		protected abstract PlayerAction BuyPhase();

		protected virtual PlayerAction ActionPhase()
		{
			CardModel bestCard = null;
			int bestVal = -100;
			foreach (CardModel card in this.Player.Hand)
			{
				if (card.Is(CardType.Action))
				{
					if (card.CardPriority.PlayPriority > bestVal && ShouldPlayAction(this.Player.Hand, card))
					{
						bestVal = card.CardPriority.PlayPriority;
						bestCard = card;
					}
				}
			}
			if (bestCard != null)
			{
				return new PlayerAction()
				{
					ActionType = ActionType.PlayCard,
					Card = bestCard
				};
			}

			return new PlayerAction() { ActionType = ActionType.EnterBuyPhase };
		}

		public override void OnTurnStart(Player player)
		{
			this.BaseAIChooser.OnTurnStart(player);
		}
	}
}
