using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;
namespace Dominion
{
	[Flags]
	public enum CardType
	{
		Treasure	= 0x0001,
		Victory		= 0x0002,
		Curse		= 0x0004,
		Action		= 0x0008,
		Bonus		= 0x0010,
		Attack		= 0x0020,
		Reaction	= 0x0040,
		Duration	= 0x0080,
		Prize		= 0x0100,
		Looter		= 0x0200,
		Shelter		= 0x0400,
		Ruins		= 0x0800,
		Knight		= 0x1000,
		Traveller	= 0x2000,
		Reserve		= 0x4000,
		Event		= 0x8000
	}

	public abstract class CardModel
	{
		protected int Points
		{
			get;
			set;
		}

		protected int Cost
		{
			get;
			set;
		}

		public CardType Type
		{
			get;
			protected set;
		}

		public bool IsPureVictory
		{
			get
			{
				return this.Is(CardType.Victory) && !this.Is(CardType.Treasure) && !this.Is(CardType.Action);
			}
		}

		public string Name
		{
			get; 
			protected set;
		}

		public virtual string PluralName
		{
			get
			{
				string name = this.Name;
				string pluralName = string.Empty;
				char last = name[name.Length - 1];
				if (last == 'y' && !Log.IsVowel(name[name.Length - 2]))
				{
					pluralName = name.Substring(0, name.Length - 1) + "ies";
				}
				else if(name.EndsWith("tch"))
				{
					pluralName = name + "es";
				}
				else if(name.EndsWith("ss"))
				{
					pluralName = name + "es";
				}
				else if(name.EndsWith("sh"))
				{
					pluralName = name = "es";
				}
				else if (last != 's')
				{
					pluralName = name + "s";
				}
				else
				{
					pluralName = name;
				}
				return pluralName;
			}
		}

		public string ID
		{
			get { return this.Name.Replace(" ", ""); }
		}

		private CardPriority cardPriority;
		public CardPriority CardPriority
		{
			get
			{
				if (this.cardPriority == null)
				{
					bool result = CardPriorityManager.PriorityMap.TryGetValue(this.ID, out this.cardPriority);
					Debug.Assert(result);
				}
				return this.cardPriority;
			}
		}

		private static Dictionary<string, CardInfo> cards;
		public CardInfo CardInfo 
		{ 
			get
			{
				CardModel.EnsureCardInfoLoaded();
				CardInfo cardInfo;
				if (!cards.TryGetValue(this.Name, out cardInfo))
				{
					cardInfo = new CardInfo() { Name = this.Name, Type="", Text="Missing info, please file a bug", Cost="" };
				}
				return cardInfo;
			}
		}

		public static void EnsureCardInfoLoaded()
		{
			if (cards == null)
			{
				cards = new Dictionary<string, Dominion.CardInfo>();
				try
				{
					XDocument document = XDocument.Load("cards.xml");
					XElement cardsElement = document.Element(XName.Get("Cards"));
					foreach (XElement cardElement in cardsElement.Elements())
					{
						string name = cardElement.Attribute(XName.Get("Name")).Value;
						string type = cardElement.Attribute(XName.Get("Type")).Value;
						string cost = cardElement.Attribute(XName.Get("Cost")).Value;
						string text = cardElement.Value;
						cards[name] = new CardInfo() { Cost = cost, Name = name, Text = text, Type = type };
					}
				}
				catch
				{
				}
			}
		}

		public bool CostsPotion
		{
			get;
			protected set;
		}

		public virtual int Cards
		{
			get;
			protected set;
		}

		public int Actions
		{
			get;
			protected set;
		}

		public int Buys
		{
			get;
			protected set;
		}

		public int Coins
		{
			get;
			protected set;
		}

		public int Potions
		{
			get;
			protected set;
		}

		// used for throne room, king's court of duration cards.
		public int DurationPlayMultiplier
		{
			get;
			set;
		}

		public virtual bool CanBuy(Player player)
		{
			return true;
		}

		public int GetBaseCost()
		{
			return this.Cost;
		}

		public virtual int GetVictoryPoints(Player player)
		{
			return this.Points;
		}

		// returns true if the reaction provides immunity
		public virtual bool ReactToAttack(GameModel gameModel, Player targetPlayer)
		{
			return false;
		}

		public ReactionTrigger ReactionTrigger
		{
			get;
			protected set;
		}

		public virtual void BeforePlay(GameModel gameModel)
		{

		}

		public virtual void Play(GameModel gameModel)
		{

		}

		public virtual void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{

		}

		public virtual void PlayPostAttack(GameModel gameModel)
		{

		}

		public virtual void PlayDuration(GameModel gameModel)
		{

		}

		public virtual void Call(GameModel gameModel)
		{

		}

        public virtual void OnBuy(GameModel gameModel)
        {

        }

        public virtual void OnGain(GameModel gameModel, Player player)
        {

        }

		public virtual void OnCleanup(GameModel gameModel)
		{

		}

		public virtual void OnDiscardedFromPlay(GameModel gameModel)
		{

		}

		public virtual void OnRemovedFromPlay(GameModel gameModel)
		{

		}

		public virtual void OnTrash(GameModel gameModel, Player owner)
		{

		}

		public virtual bool HasCleanupEffect(GameModel gameModel)
		{
			return false;
		}

		public virtual void OnGameEnd(GameModel gameModel, Player player)
		{

		}

		public virtual IDisposable ForceMultipleCardPlayChoice()
		{
			return null;
		}

		public bool IsDurationPlay
		{
			get;
			set;
		}

		// If this card is being played by a band of misfits, this is the link to the original card.
		public CardModel PlayedAsBandOfMisfitsSource
		{
			get;
			set;
		}

		// If this card is being played by a prince, this is the link to the original card.
		public CardModel PlayedWithPrinceSource
		{
			get;
			set;
		}

		// If you need to do actions based on trashing this card, use this property instead.
		public CardModel ThisAsTrashTarget
		{
			get
			{
				return this.PlayedAsBandOfMisfitsSource != null ? this.PlayedAsBandOfMisfitsSource : this;
			}
		}

		public bool Is(CardType cardType)
		{
			return (this.Type & cardType) != 0;
		}

		// True if the order for playing this as a treasure matters
		public virtual bool AffectsTreasurePlayOrder
		{
			get { return false; }
		}

		// True if the order you play treasures to buy this matters
		public virtual bool AffectsTreasureBuyOrder
		{
			get { return false; }
		}

		public virtual bool IsKingdomCard { get { return true; } }
		public abstract string Expansion { get; }
		public abstract GameSets GameSet { get; }

		public virtual CardModel Clone()
		{
			return (CardModel)Activator.CreateInstance(this.GetType());
		}
	}
}
