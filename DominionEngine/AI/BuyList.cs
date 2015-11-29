using Dominion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DominionEngine.AI
{
	[DebuggerDisplay("{Card.Name} {Count}")]
	public class BuyListItem
	{
		public BuyListItem(Type card, int count)
		{
			this.Card = card;
			this.Count = count;
		}

		public Type Card { get; set; }
		public int Count { get; set; }
	}

	public class BuyList
	{
		public int ColonyBuyThreshold { get; set; }
		public int ProvinceBuyThreshold { get; set; }
		public int DuchyBuyThreshold { get; set; }
		public int EstateBuyThreshold { get; set; }
		public BuyList()
		{
			this.List = new List<BuyListItem>();
		}

		public Type OpeningBuy1 { get; set; }
		public Type OpeningBuy2 { get; set; }
		public List<BuyListItem> List { get; private set; }
		public BuyList Clone()
		{
			BuyList clone = new BuyList();
			clone.OpeningBuy1 = this.OpeningBuy1;
			clone.OpeningBuy2 = this.OpeningBuy2;
			clone.List = new List<BuyListItem>();
			for (int i = 0; i < this.List.Count; i++)
			{
				clone.List.Add(new BuyListItem(this.List[i].Card, this.List[i].Count));
			}

			clone.ColonyBuyThreshold = this.ColonyBuyThreshold;
			clone.ProvinceBuyThreshold = this.ProvinceBuyThreshold;
			clone.DuchyBuyThreshold = this.DuchyBuyThreshold;
			clone.EstateBuyThreshold = this.EstateBuyThreshold;
			return clone;
		}

		public override bool Equals(object obj)
		{
			BuyList rhs = obj as BuyList;
			if (rhs == null) return false;
			if (!object.Equals(this.OpeningBuy1, rhs.OpeningBuy1)) return false;
			if (!object.Equals(this.OpeningBuy2, rhs.OpeningBuy2)) return false;
			if (this.ColonyBuyThreshold != rhs.ColonyBuyThreshold) return false;
			if (this.ProvinceBuyThreshold != rhs.ProvinceBuyThreshold) return false;
			if (this.DuchyBuyThreshold != rhs.DuchyBuyThreshold) return false;
			if (this.EstateBuyThreshold!= rhs.EstateBuyThreshold) return false;
			if (this.List.Count != rhs.List.Count) return false;
			for (int i = 0; i < this.List.Count; i++)
			{
				if (!this.List[i].Card.Equals(rhs.List[i].Card)) return false;
				if (this.List[i].Count != rhs.List[i].Count) return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.ColonyBuyThreshold.GetHashCode() ^ this.ProvinceBuyThreshold.GetHashCode() ^ this.DuchyBuyThreshold.GetHashCode() ^ this.EstateBuyThreshold.GetHashCode() ^ this.List.GetHashCode();
			foreach(BuyListItem item in this.List)
			{
				hashCode ^= item.Card.GetHashCode() ^ item.Count.GetHashCode();
			}
			if (this.OpeningBuy1 != null)
			{
				hashCode ^= this.OpeningBuy1.GetHashCode();
			}
			if (this.OpeningBuy2 != null)
			{
				hashCode ^= this.OpeningBuy2.GetHashCode();
			}
			return hashCode;
		}

		public string ToString(bool showThresholds)
		{
			StringBuilder result = new StringBuilder();
			if (showThresholds)
			{
				result.Append("C:" + this.ColonyBuyThreshold + ",");
				result.Append("P:" + this.ProvinceBuyThreshold + ",");
				result.Append("D:" + this.DuchyBuyThreshold + ",");
				result.Append("E:" + this.EstateBuyThreshold + ",");
			}
			result.Append(this.OpeningBuy1 != null ? this.OpeningBuy1.Name : "-");
			result.Append("/");
			result.Append(this.OpeningBuy2 != null ? this.OpeningBuy2.Name : "-");
			result.Append(",");
			foreach (BuyListItem item in this.List)
			{
				result.Append(item.Card.Name + ":" + item.Count + ",");
			}
			return result.ToString();
		}

		public override string ToString()
		{
			return this.ToString(false);
		}
	}

}
