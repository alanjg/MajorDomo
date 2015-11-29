using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Dominion.Model.PileFactories;
using Dominion.Model.Actions;

namespace Dominion
{
	public abstract class CardSet
	{
		protected ObservableCollection<CardModel> cardCollection = new ObservableCollection<CardModel>();

		public ReadOnlyObservableCollection<CardModel> CardCollection
		{
			get { return new ReadOnlyObservableCollection<CardModel>(this.cardCollection); }
		}

		public abstract string CardSetName
		{
			get;
		}

		public abstract GameSets GameSet
		{
			get;
		}

		public CardModel BaneCard
		{
			get;
			set;
		}

		public IList<Pile> CreatePiles(GameModel gameModel)
		{
			return CardSet.CreatePiles(gameModel, this.cardCollection, this.BaneCard);
		}
		public static IList<Pile> CreatePiles(GameModel gameModel, IEnumerable<CardModel> cards, CardModel bane)
		{
			List<Pile> piles = new List<Pile>();

			foreach (CardModel cardModel in cards)
			{
				int count = 10;
				if (cardModel.Is(CardType.Victory))
				{
					if (gameModel.Players.Count < 3)
					{
						count = 8;
					}
					else
					{
						count = 12;
					}
				}
				if (cardModel is Rats)
				{
					count = 20;
				}
				piles.Add(new Pile(count, gameModel, cardModel.GetType()));
			}
			if (bane != null)
			{
				gameModel.Bane = bane;
			}
			return piles;
		}
	}	

	public sealed class RandomAllCardSet : CardSet
	{
		public static CardModel[] RandomCardIDs = new CardModel[]
		{
			new Adventurer(),
			new Alchemist(),
			new Ambassador(),
			new Apothecary(),
			new Apprentice(),
			new Bank(),
			new Bazaar(),
			new Bishop(),
			new Bureaucrat(),
			new Caravan(),
			new Cellar(),
			new Chancellor(),
			new Chapel(),
			new City(),
			new Contraband(),
			new CouncilRoom(),
			new CountingHouse(),
			new Cutpurse(),
			new Embargo(),
			new Expand(),
			new Explorer(),
			new Fairgrounds(),
			new Familiar(),
			new FarmingVillage(),
			new Feast(),
			new Festival(),
			new FishingVillage(),
			new Forge(),
			new FortuneTeller(),
			new Gardens(),
			new GhostShip(),
			new Golem(),
			new Goons(),
			new GrandMarket(),
			new Hamlet(),
			new Harvest(),
			new Haven(),
			new Herbalist(),
			new Hoard(),
			new HornOfPlenty(),
			new HorseTraders(),
			new HuntingParty(),
			new Island(),
			new Jester(),
			new KingsCourt(),
			new Laboratory(),
			new Library(),
			new Lighthouse(),
			new Loan(),
			new Lookout(),
			new Market(),
			new Menagerie(),
			new MerchantShip(),
			new Militia(),
			new Mine(),
			new Mint(),
			new Moat(),
			new Moneylender(),
			new Monument(),
			new Mountebank(),
			new NativeVillage(),
			new Navigator(),
			new Outpost(),
			new PearlDiver(),
			new Peddler(),
			new PhilosophersStone(),
			new PirateShip(),
			new Possession(),
			new Quarry(),
			new Rabble(),
			new Remake(),
			new Remodel(),
			new RoyalSeal(),
			new Salvager(),
			new ScryingPool(),
			new SeaHag(),
			new Smithy(),
			new Smugglers(),
			new Spy(),
			new Tactician(),
			new Talisman(),
			new Thief(),
			new ThroneRoom(),
			new Tournament(),
			new TradeRoute(),
			new Transmute(),
			new TreasureMap(),
			new Treasury(),
			new University(),
			new Vault(),
			new Venture(),
			new Village(),
			new Warehouse(),
			new Watchtower(),
			new Wharf(),
			new Witch(),
			new Woodcutter(),
			new WorkersVillage(),
			new Workshop(),
			new YoungWitch(),
			new Baron(),
			new Bridge(),
			new Conspirator(),
			new Coppersmith(),
			new Courtyard(),
			new Duke(),
			new GreatHall(),
			new Harem(),
			new Ironworks(),
			new Masquerade(),
			new MiningVillage(),
			new Minion(),
			new Nobles(),
			new Pawn(),
			new Saboteur(),
			new Scout(),
			new SecretChamber(),
			new ShantyTown(),
			new Steward(),
			new Swindler(),
			new Torturer(),
			new TradingPost(),
			new Tribute(),
			new Upgrade(),
			new WishingWell(),
			new BorderVillage(),
			new Cache(),
			new Cartographer(),
			new Crossroads(),
			new Develop(),
			new Duchess(),
			new Embassy(),
			new Farmland(),
			new FoolsGold(),
			new Haggler(),
			new Highway(),
			new IllGottenGains(),
			new Inn(),
			new JackOfAllTrades(),
			new Mandarin(),
			new Margrave(),
			new NobleBrigand(),
			new NomadCamp(),
			new Oasis(),
			new Oracle(),
			new Scheme(),
			new SilkRoad(),
			new SpiceMerchant(),
			new Stables(),
			new Trader(),
			new Tunnel(),
			new BlackMarket(),
			new Envoy(),
			new Governor(),
			new Prince(),
			new Stash(),
			new WalledVillage(),
			new Altar(),
			new Armory(),
			new BanditCamp(),
			new BandOfMisfits(),
			new Beggar(),
			new Catacombs(),
			new Count(),
			new Counterfeit(),
			new Cultist(),
			new DeathCart(),
			new Feodum(),
			new Forager(),
			new Fortress(),
			new Graverobber(),
			new Hermit(),
			new HuntingGrounds(),
			new Ironmonger(),
			new JunkDealer(),
			new Knights(),
			new Marauder(),
			new MarketSquare(),
			new Mystic(),
			new PoorHouse(),
			new Procession(),
			new Rats(),
			new Rebuild(),
			new Rogue(),
			new Sage(),
			new Scavenger(),
			new Squire(),
			new Storeroom(),
			new Urchin(),
			new Vagrant(),
			new WanderingMinstrel(),
			//guilds
			new Advisor(),
			new Baker(),
			new Butcher(),
			new CandlestickMaker(),
			new Doctor(),
			new Herald(),
			new Journeyman(),
			new Masterpiece(),
			new MerchantGuild(),
			new Plaza(),
			new Soothsayer(),
			new Stonemason(),
			new Taxman(),
			//adventures
		};

		public RandomAllCardSet(GameSets allowedSets)
		{
			if (allowedSets == GameSets.None || allowedSets == GameSets.Promo)
			{
				allowedSets |= GameSets.Base;
			}

			foreach (CardModel card in RandomCardIDs.OrderBy(card => Randomizer.Next()))
			{
				if((card.GameSet & allowedSets) != 0)
				{
					this.cardCollection.Add(card);
				}
				if (this.cardCollection.Count == 10)
				{
					break;
				}
			}
		}

		public override string CardSetName
		{
			get { return "Random (Any)"; }
		}

		public override GameSets GameSet
		{
			get { return GameSets.Any; }
		}
	}
}
