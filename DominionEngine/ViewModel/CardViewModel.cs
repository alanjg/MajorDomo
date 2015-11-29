using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Dominion;
using Dominion.Model.Actions;

namespace Dominion
{
	public class CardViewModel : NotifyingObject, IDisplayable
	{
		private CardModel card;
		public CardModel CardModel { get { return this.card; } }

		public CardViewModel(CardModel card)
		{
			this.card = card;
			this.order = -1;
		}

		public string Name
		{
			get { return this.card.Name; }
		}

		public string ID
		{
			get
			{
				return this.card.ID;
			}
		}

		public string Image
		{
			get { return this.card.Name.Replace(" ", "").Replace("-","").Replace("'","") + ".jpg"; }
		}


		public CardInfo CardInfo
		{
			get
			{
				return this.card.CardInfo;
			}
		}

		public override string ToString()
		{
			return this.Name;
		}

		public bool Is(CardType cardType)
		{
			return this.CardModel.Is(cardType);
		}

		public CardDisplayModel DisplayModel
		{
			get { return new ImageCardDisplayModel(this.Image); }
		}

		private bool isSelected;
		public bool IsSelected 
		{
			get { return this.isSelected; }
			set { this.isSelected = value; this.OnPropertyChanged("IsSelected"); }
		}

		private bool isSelectable;
		public bool IsSelectable
		{
			get { return this.isSelectable; }
			set { this.isSelectable = value; this.OnPropertyChanged("IsSelectable"); }
		}

		private int order;
		public int Order
		{
			get { return this.order; }
			set { this.order = value; this.OnPropertyChanged("Order"); this.OnPropertyChanged("ShowOrder"); }
		}

		public bool ShowOrder
		{
			get { return this.order != -1; }
		}

		public CardType CardType { get { return this.CardModel.Type; } }
	}
}