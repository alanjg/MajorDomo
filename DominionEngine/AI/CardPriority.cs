using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
	public class CardPriority
	{
		public int PlayPriority { get; set; }
		public int MultiplePlayPriority { get; set; }
		public int DiscardPriority { get; set; }
		public int TrashPriority { get; set; }
		public int TrashForBenefitPriority { get; set; }
		public int MaxGain { get; set; }
		public double WinRateWith { get; set; }
	}
	public static class CardPriorityManager
	{
		private static object priorityMapLock = new object();
		public static Dictionary<string, CardPriority> PriorityMap
		{
			get
			{
				if (priorityMap == null)
				{
					lock (priorityMapLock)
					{
						if (priorityMap == null)
						{
							Initialize();
						}
					}
				}
				return priorityMap;
			}
		}

		private static Dictionary<string, CardPriority> priorityMap;

		private static void Initialize()
		{
			Dictionary<string, CardPriority> priorityMap = new Dictionary<string, CardPriority>();
			string[] lines = priorityText.Split('\n');
			foreach(string line in lines)
			{
				string[] items = line.Split(',');
				CardPriority priority = new CardPriority();
				
				int playPriority;
				if (!int.TryParse(items[1], out playPriority)) playPriority = 0;
				priority.PlayPriority = playPriority;

				int multiplePlayPriority;
				if (!int.TryParse(items[2], out multiplePlayPriority)) multiplePlayPriority = 0;
				priority.MultiplePlayPriority = multiplePlayPriority;

				int discardPriority;
				if (!int.TryParse(items[3], out discardPriority)) discardPriority = 0;
				priority.DiscardPriority = discardPriority;

				int trashPriority;
				if (!int.TryParse(items[4], out trashPriority)) trashPriority = 0;
				priority.TrashPriority = trashPriority;

				int trashForBenefitPriority;
				if (!int.TryParse(items[5], out trashForBenefitPriority)) trashForBenefitPriority = 0;
				priority.TrashForBenefitPriority = trashForBenefitPriority;

				int maxGain;
				if (!int.TryParse(items[6], out maxGain)) maxGain = 10;
				priority.MaxGain = maxGain;

				double winRateWith;
				if (!double.TryParse(items[7], out winRateWith)) winRateWith = 1.0;
				priority.WinRateWith = winRateWith;

				priorityMap[items[0]] = priority;
			}
			CardPriorityManager.priorityMap = priorityMap;
		}

		//Card,Play,PlayKingsCourt,Discard,Trash,TrashForBenefit,MaxToGain,Win Rate with
		private static string priorityText =
@"AbandonedMine,0,1,,,,0,
Adventurer,16,16,,,,4,0.93
Advisor,39,39,,,,10,
Alchemist,39,39,0,0,0,10,0.99
Altar,16,16,,,,4,
Ambassador,21,41,,,,2,1.03
Apothecary,39,9,,,,5,0.96
Apprentice,30,10,,,,10,1.02
Armory,3,3,,,,10,
BagofGold,30,30,,,,1,1.39
Baker,35,15,,,,10,
BanditCamp,40,30,,,,10,
BandofMisfits,50,50,,,,10,
Bank,1,1,,,,10,1.01
Baron,13,13,,,,4,0.95
Bazaar,40,10,,,,10,1
Beggar,1,1,,,,10,
Bishop,15,35,,,,10,1
BlackMarket,5,15,,,,10,0.95
BorderVillage,40,0,,,,10,1.01
Bridge,18,38,,,,10,0.97
Bureaucrat,20,20,,,,4,0.84
Butcher,13,13,,,,4,
Cache,10,10,,,,10,0.89
CandlestickMaker,31,11,,,,10,
Caravan,35,35,,,,10,1
Cartographer,39,19,,,,10,1
Catacombs,15,30,,,,10,
Cellar,30,0,,,,5,0.97
Chancellor,3,3,,,,4,0.87
Chapel,17,0,,,,2,1.01
City,40,50,,,,10,0.99
Colony,0,0,,,,12,1.1
Conspirator,10,30,,,,10,1
Contraband,2,2,,,,2,0.87
Copper,10,10,,,,0,0.96
Coppersmith,10,30,,,,4,0.81
CouncilRoom,15,35,,,,10,0.96
Count,10,1,,,,10,
Counterfeit,20,20,,,,10,
CountingHouse,10,0,,,,10,0.76
Courtyard,13,33,,,,10,0.98
Crossroads,40,0,,,,10,1
Cultist,23,43,,,,10,
Curse,0,0,,,,0,0.91
Cutpurse,20,20,,,,4,1
DameAnna,21,41,,,,1,
DameJosephine,21,41,,,,1,
DameMolly,21,41,,,,1,
DameNatalie,21,41,,,,1,
DameSylvia,21,41,,,,1,
DeathCart,10,20,,,,10,
Develop,4,4,,,,4,0.92
Diadem,10,10,,,,1,1.43
Doctor,1,1,,,,2,
Duchess,2,2,,,,10,0.93
Duchy,0,0,,,,12,0.99
Duke,0,0,,,,12,0.95
Embargo,3,3,,,,10,0.97
Embassy,14,34,,,,10,1.02
Envoy,15,35,,,,10,0.99
Estate,0,0,,,,12,1.01
Expand,17,37,,,,10,0.99
Explorer,8,8,,,,10,0.95
Fairgrounds,0,0,,,,12,0.99
Familiar,35,45,,,,3,1.04
FarmingVillage,40,0,,,,10,0.99
Farmland,0,0,,,,12,0.99
Feast,10,30,,,,10,0.94
Feodum,0,0,,,,12,
Festival,40,0,,,,10,0.99
FishingVillage,40,0,,,,10,1
Followers,26,26,,,,1,1.55
Fool'sGold,10,10,,,,10,0.98
Forager,30,0,,,,3,
Forge,17,17,,,,3,0.96
Fortress,40,0,,,,10,
FortuneTeller,20,20,,,,6,0.95
Gardens,0,0,,,,12,1
GhostShip,25,25,,,,10,1.04
Gold,10,10,,,,100,1
Golem,51,51,,,,10,0.98
Goons,28,48,,,,10,1.04
Governor,34,40,,,,10,1.02
GrandMarket,35,39,,,,10,1.04
Graverobber,13,23,,,,5,
GreatHall,35,15,,,,12,0.98
Haggler,15,15,,,,10,1
Hamlet,40,0,,,,10,1
Harem,10,10,,,,12,1
Harvest,15,25,,,,10,0.94
Haven,35,15,,,,10,0.99
Herald,39,19,,,,10,
Herbalist,1,1,,,,5,0.91
Hermit,5,15,,,,10,
Highway,35,15,,,,10,0.98
Hoard,10,10,,,,10,1.01
HornofPlenty,0,0,,,,10,0.96
HorseTraders,12,12,,,,10,0.96
Hovel,0,0,,,,0,
HuntingGrounds,16,26,,,,10,
HuntingParty,39,39,,,,10,1.05
Ill-GottenGains,10,10,,,,10,1.05
Inn,40,0,,,,10,0.98
Ironmonger,39,15,,,,10,
Ironworks,10,20,,,,10,0.95
Island,9,9,,,,12,1
JackofAllTrades,15,0,,,,2,1.02
Jester,20,20,,,,10,0.99
Journeyman,15,25,,,,10,
JunkDealer,30,0,,,,2,
King'sCourt,55,55,,,,10,1.02
Knights,20,40,,,,10,
Laboratory,39,39,,,,10,1.01
Library,10,0,,,,5,0.98
Lighthouse,30,10,,,,10,0.98
Loan,10,10,,,,1,1.01
Lookout,30,0,,,,2,0.97
Madman,45,0,,,,10,
Mandarin,10,20,,,,10,0.88
Marauder,20,40,,,,2,
Margrave,20,20,,,,10,1.02
Market,35,15,,,,10,0.99
MarketSquare,30,0,,,,10,
Masquerade,12,12,,,,2,1.05
Masterpiece,10,10,,,,10,
Menagerie,39,0,,,,10,1.03
Mercenary,22,22,,,,10,
MerchantGuild,13,13,,,,10,
MerchantShip,17,27,,,,10,0.97
Militia,22,20,,,,10,0.99
Mine,15,25,,,,2,0.93
MiningVillage,40,0,,,,10,0.99
Minion,30,20,,,,10,1.02
Mint,13,13,,,,4,1.01
Moat,3,3,,,,10,0.94
Moneylender,13,0,,,,1,0.98
Monument,15,35,,,,10,1
Mountebank,26,46,,,,10,1.08
Mystic,31,0,,,,10,
NativeVillage,40,0,,,,10,0.95
Navigator,3,3,,,,10,0.93
Necropolis,40,0,,,,0,
NobleBrigand,20,20,,,,10,0.88
Nobles,40,55,,,,12,1.02
NomadCamp,3,3,,,,10,0.94
Oasis,32,0,,,,10,0.98
Oracle,20,20,,,,10,0.97
Outpost,15,0,,,,10,0.92
OvergrownEstate,0,0,,,,0,
Pawn,35,15,,,,10,0.98
PearlDiver,39,0,,,,10,0.96
Peddler,35,10,,,,10,1.03
Philosopher'sStone,10,10,,,,10,0.87
Pillage,24,44,,,,10,
PirateShip,21,41,,,,10,0.84
Platinum,10,10,,,,100,1.07
Plaza,40,0,,,,10,
PoorHouse,3,3,,,,10,
Possession,29,49,,,,10,1
Potion,10,10,,,,2,0.94
Prince,28,0,,,,10,1.0
Princess,18,18,,,,1,1.41
Procession,52,52,,,,10,
Province,0,0,,,,12,1.03
Quarry,10,10,,,,10,0.95
Rabble,20,40,,,,10,1
Rats,32,0,,,,20,
Rebuild,30,30,,,,10,
Remake,16,16,,,,10,0.99
Remodel,13,23,,,,10,0.95
Rogue,21,41,,,,10,
RoyalSeal,10,10,,,,10,0.95
RuinedLibrary,0,1,,,,0,
RuinedMarket,0,1,,,,0,
RuinedVillage,0,1,,,,0,
Saboteur,21,41,,,,10,0.84
Sage,35,35,,,,10,
Salvager,13,23,,,,10,1
Scavenger,8,8,,,,10,
Scheme,35,35,,,,10,1
Scout,35,0,,,,2,0.89
ScryingPool,39,39,,,,10,1.02
SeaHag,25,45,,,,1,1.04
SecretChamber,1,1,,,,2,0.88
ShantyTown,40,0,,,,10,0.98
SilkRoad,0,0,,,,12,1
Silver,10,10,,,,100,0.98
SirBailey,21,31,,,,1,
SirDestry,21,31,,,,1,
SirMartin,21,31,,,,1,
SirMichael,21,31,,,,1,
SirVander,21,31,,,,1,
Smithy,17,27,,,,10,0.98
Smugglers,10,10,,,,10,0.91
Soothsayer,20,30,,,,4,
SpiceMerchant,15,15,,,,2,1
Spoils,10,10,,,,15,
Spy,35,15,,,,10,0.95
Squire,7,7,,,,10,
Stables,35,35,,,,10,1.02
Stash,10,10,,,,10,0.93
Steward,15,25,,,,10,0.99
Stonemason,7,7,,,,5,
Storeroom,2,2,,,,4,
Survivors,0,0,,,,0,
Swindler,21,32,,,,10,1
Tactician,18,0,,,,2,1.01
Talisman,10,10,,,,10,0.87
Taxman,20,0,,,,1,
Thief,2,2,,,,2,0.72
ThroneRoom,53,53,,,,10,0.96
Torturer,22,49,,,,10,1.03
Tournament,32,36,,,,10,1.03
Trader,13,1,,,,2,0.93
TradeRoute,13,1,,,,4,0.98
TradingPost,13,0,,,,1,0.99
Transmute,5,5,,,,5,0.77
TreasureMap,19,0,,,,10,0.91
Treasury,35,35,,,,10,0.99
Tribute,15,35,,,,10,0.92
TrustySteed,49,49,,,,1,1.49
Tunnel,0,0,,,,12,0.98
University,40,30,,,,10,0.92
Upgrade,33,13,,,,10,1
Urchin,32,12,,,,10,
Vagrant,39,19,,,,10,
Vault,11,0,,,,3,1.03
Venture,10,10,,,,10,1.03
Village,40,0,,,,10,0.97
Vineyard,0,0,,,,12,1.02
WalledVillage,40,0,,,,10,0.97
WanderingMinstrel,40,0,,,,10,
Warehouse,35,0,,,,10,1.02
Watchtower,15,0,,,,10,0.95
Wharf,19,39,,,,10,1.04
WishingWell,39,19,,,,10,0.98
Witch,27,47,,,,2,1.07
Woodcutter,3,3,,,,6,0.92
Worker'sVillage,40,0,,,,10,0.99
Workshop,3,3,,,,10,0.87
YoungWitch,25,25,,,,2,1.03";
	}
}
