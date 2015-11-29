using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Chooser;

namespace Dominion.Model.Actions
{
	public class BandOfMisfits : DarkAgesCardModel
	{
		private class BandOfMisfitsLockToken : IDisposable
		{
			private BandOfMisfits card;
			public BandOfMisfitsLockToken(BandOfMisfits card)
			{
				this.card = card;
				this.card.lockCount++;
			}
			public void Dispose()
			{
				this.card.lockCount--;
				if (this.card.lockCount == 0)
				{
					this.card.forceMimic = null;
				}
			}
		}
		private const string cardName = "Band of Misfits";
		private CardModel mimic = null; 
		private CardModel forceMimic = null;
		private int lockCount = 0;

		public override CardModel Clone()
		{
			BandOfMisfits clone = (BandOfMisfits)base.Clone();
			clone.mimic = this.mimic != null ? this.mimic.Clone() : null;
			clone.forceMimic = this.forceMimic != null ? this.forceMimic.Clone() : null;
			clone.lockCount = this.lockCount;
			return clone;
		}
		public BandOfMisfits()
		{
			this.Name = BandOfMisfits.cardName;
			this.Type = CardType.Action;
			this.Cost = 5;
		}

		public override IDisposable ForceMultipleCardPlayChoice()
		{
			return new BandOfMisfitsLockToken(this);
		}

		public override void BeforePlay(GameModel gameModel)
		{
			if (this.mimic == null)
			{
				CardModel target = null;
				if (this.forceMimic != null)
				{
					target = this.forceMimic;
				}
				else
				{
					Pile pile = gameModel.CurrentPlayer.Chooser.ChooseOnePile(CardChoiceType.BandOfMisfits, "Play Band of Misfits as...", gameModel.SupplyPiles.Where(p => p.GetCost() < gameModel.GetCost(this) && !p.CostsPotion && p.Count > 0 && p.Card.Is(CardType.Action)));
					if (pile != null)
					{
						target = (CardModel)Activator.CreateInstance(pile.TopCard.GetType());
					}
				}
				if (target != null)
				{
					this.mimic = target;
					if (this.lockCount > 0)
					{
						this.forceMimic = target;
					}
					this.SetMimic();
				}
			}
		}

		private void SetMimic()
		{
			this.mimic.PlayedAsBandOfMisfitsSource = this;
			this.Cost = this.mimic.GetBaseCost();
			this.Actions = this.mimic.Actions;
			this.Coins = this.mimic.Coins;
			this.Type = this.mimic.Type;
			this.Name = this.mimic.Name;
			this.Buys = this.mimic.Buys;
			this.Cards = this.mimic.Cards;
			this.ReactionTrigger = this.mimic.ReactionTrigger;
		}

		public override void Play(GameModel gameModel)
		{
			if (this.mimic != null)
			{
				this.mimic.Play(gameModel);
			}
		}

		public override void OnRemovedFromPlay(GameModel gameModel)
		{
			this.Revert();
		}

		private void Revert()
		{
			this.mimic = null;
			this.Cost = 5;
			this.Actions = 0;
			this.Coins = 0;
			this.Type = CardType.Action;
			this.Name = BandOfMisfits.cardName;
			this.Buys = 0;
			this.Cards = 0;
			this.ReactionTrigger = ReactionTrigger.None;
		}

		public override bool AffectsTreasureBuyOrder
		{
			get
			{
				return this.mimic != null ? this.mimic.AffectsTreasureBuyOrder : false;
			}
		}

		public override bool AffectsTreasurePlayOrder
		{
			get
			{
				return this.mimic != null ? this.mimic.AffectsTreasurePlayOrder : false;
			}
		}

		public override bool HasCleanupEffect(GameModel gameModel)
		{
			return this.mimic != null ? this.mimic.HasCleanupEffect(gameModel) : false;
		}

		public override void OnCleanup(GameModel gameModel)
		{
			if (this.mimic != null)
			{
				this.mimic.OnCleanup(gameModel);
			}
		}

		public override void OnTrash(GameModel gameModel, Player owner)
		{
			if (this.mimic != null)
			{
				this.mimic.OnTrash(gameModel, owner);
			}
		}

		public override void PlayAttack(GameModel gameModel, IEnumerable<Player> attackedPlayers)
		{
			if (this.mimic != null)
			{
				this.mimic.PlayAttack(gameModel, attackedPlayers);
			}
		}

		public override void PlayPostAttack(GameModel gameModel)
		{
			if (this.mimic != null)
			{
				this.mimic.PlayPostAttack(gameModel);
			}
		}

		public override void PlayDuration(GameModel gameModel)
		{
			if (this.mimic != null)
			{
				this.mimic.PlayDuration(gameModel);
			}
		}

		public override void OnDiscardedFromPlay(GameModel gameModel)
		{
			if (this.mimic != null)
			{
				this.mimic.OnDiscardedFromPlay(gameModel);
			}
		}

		public override void OnGameEnd(GameModel gameModel, Player player)
		{
			if (this.mimic != null)
			{
				this.mimic.OnGameEnd(gameModel, player);
			}
		}
	}
}