using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Model.Chooser
{
	[Flags]
	public enum ChoiceSource
	{
		None =		0x01,
		FromHand =	0x02,
		FromPile =	0x04,
		InPlay =	0x08,
		FromTrash =	0x10
	}

	public interface IChooser
	{
		CardModel ChooseZeroOrOneCard(CardChoiceType choiceType, string choiceText, ChoiceSource source, IEnumerable<CardModel> choices);
		CardModel ChooseZeroOrOneCard(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, IEnumerable<CardModel> choices);
		Pile ChooseZeroOrOnePile(CardChoiceType choiceType, string choiceText, IEnumerable<Pile> choices);

		CardModel ChooseOneCard(CardChoiceType choiceType, string choiceText, ChoiceSource source, IEnumerable<CardModel> choices);
		CardModel ChooseOneCard(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, IEnumerable<CardModel> choices);
		Pile ChooseOnePile(CardChoiceType choiceType, string choiceText, IEnumerable<Pile> choices);
		int ChooseOneEffect(EffectChoiceType choiceType, CardModel cardInfo, string choiceText, string[] choices, string[] choiceDescriptions);
		int ChooseOneEffect(EffectChoiceType choiceType, IEnumerable<CardModel> cardInfo, string choiceText, string[] choices, string[] choiceDescriptions);
		int ChooseOneEffect(EffectChoiceType choiceType, string choiceText, string[] choices, string[] choiceDescriptions);

		IEnumerable<CardModel> ChooseSeveralCards(CardChoiceType choiceType, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices);
		IEnumerable<CardModel> ChooseSeveralCards(CardChoiceType choiceType, CardModel cardInfo, string choiceText, ChoiceSource source, int minChoices, int maxChoices, IEnumerable<CardModel> choices);
		IEnumerable<Pile> ChooseSeveralPiles(CardChoiceType choiceType, string choiceText, int minChoices, int maxChoices, IEnumerable<Pile> choices);
		IEnumerable<int> ChooseSeveralEffects(EffectChoiceType choiceType, CardModel cardInfo, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions);
		IEnumerable<int> ChooseSeveralEffects(EffectChoiceType choiceType, IEnumerable<CardModel> cardInfo, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions);
		IEnumerable<int> ChooseSeveralEffects(EffectChoiceType choiceType, string choiceText, int minChoices, int maxChoices, string[] choices, string[] choiceDescriptions);

		IEnumerable<CardModel> ChooseOrder(CardOrderType choiceType, string choiceText, IEnumerable<CardModel> choices);
	}

	// Used for AI
	public enum CardChoiceType
	{
		Gain, // Player will gain the card
		ForceGain, // Opponent will gain the card
		GainInHand, // Player will gain the card in hand
		GainOnTopOfDeck, // Player will gain the card on top of deck
		GainForHornOfPlenty,
		GainForIronworks,

		DiscardOrPutOnDeck, // Player discards these cards or puts them on deck
		GolemPlayOrder,
		ThroneRoom,
		KingsCourt,
		Counterfeit,
		Procession,
		
		Trash, // Player will trash
		ForceTrash, // Opponent will trash
		TrashFromHand, 

		TrashForTransmute,
		TrashForApprentice, 
		TrashForMine,
		TrashForRemodel,
		TrashForThief,
		TrashForRemake,
		TrashForGovernor,
		TrashForDeathCart,
		TrashForGraverobber,
		TrashForKnight,
		TrashForDevelop,
		TrashForFarmland,
		TrashForNobleBrigand,
		TrashForTrader,
		TrashForUpgrade,
		TrashForBishop,
		TrashForExpand,
		TrashForForge,
		TrashForSalvager,
		TrashForButcher,
		TrashForStonemason,
		TrashForTaxman,

		PutOnDeckFromPlayed, // Player will put a card in Play on deck
		PutOnDeckFromHand, // Player will put the choice on deck from their hand
		PutOnDeckFromDiscard,

		PutInHand, // pick a card to put in hand and discard the rest

		Discard, // Player will discard
		ForceDiscard, // Player will make other player discard
		DiscardForCellar, //Player will discard for cellar(+1 card per card discarded)
		DiscardForCards, //Player will discard for +1 card per card discarded
		DiscardForCoins, //Player will discard for +1 coin per card discarded
		DiscardForArtificer,

		ReactToAttack,
		ReactToGain,

		Ambassador,
		BlackMarket,
		Contraband,
		CountingHouse,
		Embargo,
		FoolsGold,
		Haven,
		Inn,
		Masquerade,
		Island,
		Scheme,
		Tunnel,
		BandOfMisfits,
		Plaza,
		Prince,
		PrincePlayOrder,
		PlayCaravanGuard,
		Disciple,
		Ferry,
		Inheritance,
		LostArts,
		Raze,
		SaveEvent,
		PlayTreasuresForStoryteller,
		TeacherTokenPile,
		ActionTokenPile,

		NameACardToDraw,
		NameACardForRebuild,
		NameACardForDoctor,
		NameACardForJourneyman,
		Other
	}

	public enum CardOrderType
	{
		OrderOnDeck, // Player puts these cards back on deck in the specified order
		OrderInBlackMarket,
		Stash,

		Other
	}

	public enum EffectChoiceType
	{
		TrashForLoan,
		TrashForMercenary,
		TrashForSpiceMerchant,
		TrashMiningVillage,
		TrashUrchin,

		Cultist,
		GainDuchess,
		GainForJester,
		GainForHuntingGrounds,
		
		PutDeckInDiscard,
		DiscardOrPutOnDeck, // Player will put this card on top of deck or will discard it
		DiscardOrPutOnDeckToDraw, // Player will put this card on top of deck(for drawing) or will discard it
		ForceDiscardOrPutOnDeck, // Player will make another player put this card on top of deck or discard it
		
		DiscardForHamletAction,
		DiscardForHamletBuy,
		DiscardBeggar,
		DiscardForStables,
		DiscardForBaron,
		DiscardForMountebank,
		DiscardMarketSquare,

		SetAsideForLibrary,
		RevealForTournament,
		RevealBane,

		TrustySteed,
		CountEffect1, 
		CountEffect2,
		Graverobber,
		Squire,
		IllGottenGains,
		SpiceMerchantEffect,
		Minion,
		Nobles,
		Pawn,
		Steward,
		Torturer,
		Governor,
		Vault,
		Prince,
		AmbassadorCount,
		Explorer,
		NativeVillage,
		PearlDiver,
		PirateShip,
		Hovel,
		Watchtower,
		RoyalSeal,
		Butcher,
		DoctorPay,
		DoctorEffect,
		HeraldPay,
		MasterpiecePay,
		StonemasonPay,
		Amulet,
		Duplicate,
		Guide,
		Miser,
		Ratcatcher,
		RoyalCarriage,
		Teacher,
		Transmogrify,
		TeacherTokenType,
		WineMerchant,
		PlayCoinTokens,

		ExchangePeasantForSoldier,
		ExchangeSoldierForFugitive,
		ExchangeFugitiveForDisiple,
		ExchangeDisipleForTeacher,
		ExchangeHeroForChampion,

		ExchangePageForTreasureHunter,
		ExchangeTreasureHunterForWarrior,
		ExchangeWarriorForHero,

		Other
	}
}