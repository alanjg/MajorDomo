using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion.Model.Actions;

namespace Dominion.Model.Actions
{
	public sealed class Copper : BaseCardModel
	{
		public Copper()
		{
			this.Name = "Copper";
			this.Cost = 0;
			this.Coins = 1;
			this.Type = CardType.Treasure;
		}

		public override bool IsKingdomCard { get { return false; } }
		public override CardModel Clone()
		{
			return new Copper();
		}
	}

	public sealed class Silver : BaseCardModel
	{
		public Silver()
		{
			this.Name = "Silver";
			this.Cost = 3;
			this.Coins = 2;
			this.Type = CardType.Treasure;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Silver();
		}
	}

	public sealed class Gold : BaseCardModel
	{
		public Gold()
		{
			this.Name = "Gold";
			this.Cost = 6;
			this.Coins = 3;
			this.Type = CardType.Treasure;
		}

		public override bool IsKingdomCard { get { return false; } }
		public override CardModel Clone()
		{
			return new Gold();
		}
	}

	public sealed class Platinum : ProsperityCardModel
	{
		public Platinum()
		{
			this.Name = "Platinum";
			this.Cost = 9;
			this.Coins = 5;
			this.Type = CardType.Treasure;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Platinum();
		}
	}

	public sealed class Potion : AlchemyCardModel
	{
		public Potion()
		{
			this.Name = "Potion";
			this.Cost = 4;
			this.Coins = 0;
			this.Potions = 1;
			this.Type = CardType.Treasure;
		}

		public override bool IsKingdomCard { get { return false; } }

		public override CardModel Clone()
		{
			return new Potion();
		}
	}
}
