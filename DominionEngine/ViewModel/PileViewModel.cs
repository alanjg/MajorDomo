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
	public class PileViewModel : INotifyPropertyChanged, IDisplayable
	{
		private Pile pile;
		public Pile PileModel { get { return this.pile; } }
		public PileViewModel(Pile pile, GameViewModel gameViewModel)
		{
			this.pile = pile;
			this.pile.PropertyChanged += new PropertyChangedEventHandler(pile_PropertyChanged);
			this.gameViewModel = gameViewModel;
		}

		private void pile_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			ViewModelDispatcher.BeginInvoke(new Action(delegate()
			{
				this.OnPropertyChanged(e.PropertyName);
			}));
		}

		public string Name
		{
			get { return this.pile.Name; }
		}

		public string ID
		{
			get { return this.pile.ID; }
		}

		public int Count
		{
			get { return this.pile.Count; }
		}

		public int EmbargoCount
		{
			get { return this.pile.EmbargoCount; }
		}

		public int TradeRouteCount
		{
			get { return this.pile.TradeRouteCount; }
		}

		public bool CostsPotion
		{
			get { return this.pile.CostsPotion; }
		}

		public int Cost
		{
			get { return this.pile.GetCost(); }
		}

		public CardViewModel Card
		{
			get { return new CardViewModel(this.pile.Card); }
		}

		public CardViewModel TopCard
		{
			get { return new CardViewModel(this.pile.TopCard); }
		}

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		private GameViewModel gameViewModel;
		public bool IsBanePile
		{
			get { return this.gameViewModel.Bane.Name == this.PileModel.Name; }
		}

		public bool CanBuy
		{
			get
			{
				return this.gameViewModel.CurrentPhase == GamePhase.Buy &&
					this.PileModel.Count > 0 &&
					this.gameViewModel.GameModel.GetCost(this.PileModel) <= this.gameViewModel.CurrentPlayer.Coin &&
					(!this.PileModel.CostsPotion || this.gameViewModel.CurrentPlayer.Potions > 0) &&
					this.gameViewModel.CurrentPlayer.Buys > 0 &&
					!this.PileModel.Contrabanded &&
					this.PileModel.TopCard.CanBuy(this.gameViewModel.CurrentPlayer.PlayerModel);
			}
		}

		public void Update()
		{
			this.OnPropertyChanged("CanBuy");
		}

		public CardDisplayModel DisplayModel
		{
			get { return new ImageCardDisplayModel(this.Card.Image); }
		}

		public bool IsSelected { get; set; }
		private bool isSelectable;
		public bool IsSelectable
		{
			get { return this.isSelectable; }
			set { this.isSelectable = value; this.OnPropertyChanged("IsSelectable"); }
		}

		public CardType CardType { get { return this.pile.Card.Type; } }
	}
}