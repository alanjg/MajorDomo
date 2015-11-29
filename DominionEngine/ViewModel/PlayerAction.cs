using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
	public enum ActionType
	{
		PlayCard,
		PlayBasicTreasures,
		PlayCoinTokens,
		BuyCard,
		CleanupCard,
		EnterBuyPhase,
		EndTurn,
	}

	public class PlayerAction
	{
		public PlayerAction()
		{
		}

		public PlayerAction(ActionType actionType)
		{
			this.ActionType = actionType;
		}

		public PlayerAction(ActionType actionType, CardModel card)
		{
			this.ActionType = actionType;
			this.Card = card;
		}

		public PlayerAction(ActionType actionType, Pile pile)
		{
			this.ActionType = actionType;
			this.Pile = pile;
		}

		public ActionType ActionType { get; set; }
		public CardModel Card { get; set; }
		public Pile Pile { get; set; }

		public override string ToString()
		{
			StringBuilder result = new StringBuilder();
			result.Append(this.ActionType.ToString());
			if (this.Card != null)
			{
				result.Append(" ");
				result.Append(this.Card.Name);
			}
			if (this.Pile != null)
			{
				result.Append(" ");
				result.Append(this.Pile.Name);
			}
			return result.ToString();
		}

		public override bool Equals(object obj)
		{
			PlayerAction rhs = obj as PlayerAction;
			if(rhs == null)
			{
				return false;
			}
			return this.ActionType == rhs.ActionType &&
						((this.Card == null && rhs.Card == null) || (this.Card != null && rhs.Card != null && this.Card.Name == rhs.Card.Name)) &&
						((this.Pile == null && rhs.Pile == null) || (this.Pile != null && rhs.Pile != null && this.Pile.Name == rhs.Pile.Name));
					
		}

		public override int GetHashCode()
		{
			int hashCode = this.ActionType.GetHashCode();
			if (this.Card != null)
			{
				hashCode ^= this.Card.GetHashCode();
			}
			if (this.Pile != null)
			{
				hashCode ^= this.Pile.GetHashCode();
			}
			return hashCode;
		}
	}
}
