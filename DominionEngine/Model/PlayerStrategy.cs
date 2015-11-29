using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Dominion.Model.Chooser;

namespace Dominion
{
	public abstract class PlayerStrategy
	{
		protected GameModel GameModel
		{
			get;
			private set;
		}

		public Player Player
		{
			get;
			set;
		}

		public abstract string Name
		{
			get;
		}

		public IChooser Chooser
		{
			get;
			private set;
		}

		public abstract PlayerAction GetNextAction();

		protected PlayerStrategy(GameModel gameModel, IChooser chooser)
		{
			this.GameModel = gameModel;
			this.Chooser = chooser;
		}

		public virtual void OnGameStart(Kingdom kingdom)
		{
		}

		public virtual void OnTurnStart(Player player)
		{
		}

		public virtual void OnThisPlayerGainedCard(CardModel card)
		{
		}

		public virtual void OnOtherPlayerGainedCard(Player player, CardModel card)
		{
		}
	}
}
