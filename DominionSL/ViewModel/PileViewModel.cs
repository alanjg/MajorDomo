using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Dominion;
using Dominion.Model.Actions;

namespace DominionSL
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

		void pile_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			App.UIDispatcher.BeginInvoke(new Action(delegate()
			{
				this.OnPropertyChanged(e.PropertyName);
			}));
		}

		public string Name
		{
			get { return this.pile.Name; }
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

		public CardViewModel Card
		{
			get { return new CardViewModel(this.pile.Card); }
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
					this.PileModel.Card.CanBuy(this.gameViewModel.CurrentPlayer.PlayerModel);
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
	}
}