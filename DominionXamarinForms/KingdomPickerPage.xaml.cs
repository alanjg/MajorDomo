using Dominion;
using Dominion.Model.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace DominionXamarinForms
{
    public partial class KingdomPickerPage : ContentPage
    {
        private bool pickingBane = false;
        public CardCollectionModel Model { get; private set; }

        public KingdomPickerPage()
        {
            this.Model = App.CardCollectionModel;
            this.BindingContext = this.Model;

            InitializeComponent();
            foreach (CardCollection collection in this.Model.CardCollections)
            {
                ScrollView scrollView = new ScrollView();
                scrollView.Orientation = ScrollOrientation.Vertical;
                this.CardCollectionsLayout.Children.Add(scrollView);

                StackLayout setLayout = new StackLayout();
                setLayout.Orientation = StackOrientation.Vertical;
                scrollView.Content = setLayout;

                Label l = new Label();
                l.Text = collection.Name;
                setLayout.Children.Add(l);
                foreach (CardViewModel card in collection.Cards)
                {
                    Button button = new Button();
                    button.BindingContext = card;
                    button.Text = card.Name;
                    button.Clicked += Card_Clicked;
                    setLayout.Children.Add(button);
                }
            }
        }

		private void RedrawCards()
		{
			this.BaneControl.Text = this.Model.BaneCard != null ? this.Model.BaneCard.Name : string.Empty;
			this.SelectedCardsLayout.Children.Clear();
			foreach (CardViewModel selectedCard in this.Model.SelectedCards)
			{
				Label l = new Label();
				l.Text = selectedCard.Name;
				this.SelectedCardsLayout.Children.Add(l);
			}
		}

		private void Card_Clicked(object sender, EventArgs e)
		{
			CardViewModel card = (CardViewModel)((Button)sender).BindingContext;
			if (!this.Model.SelectedCards.Any(c => c.Name == card.Name) && (this.Model.BaneCard == null || this.Model.BaneCard.Name != card.Name))
			{
				if (this.pickingBane)
				{
					if ((card.CardModel.GetBaseCost() == 2 || card.CardModel.GetBaseCost() == 3) && !card.CardModel.CostsPotion)
					{
						this.Model.BaneCard = card;
						this.pickingBane = false;
						this.BaneButton.IsToggled = false;
					}
				}
				else if (this.Model.SelectedCards.Count < 10)
				{
					this.Model.SelectedCards.Add(card);
					if (card.CardModel is YoungWitch)
					{
						this.BaneButton.IsVisible = true;
						this.BaneText.IsVisible = true;
						this.BaneControl.IsVisible = true;
						this.BaneButton.IsToggled = false;
					}
				}
			}

			this.RedrawCards();
		}

        private void SelectedCardClicked(object sender, EventArgs e)
        {
            CardViewModel card = (CardViewModel)((Button)sender).BindingContext;
            if (this.Model.BaneCard == card)
            {
                this.Model.BaneCard = null;
			}
            else
            {
                this.Model.SelectedCards.Remove(card);
            }
			this.RedrawCards();
            if (card.CardModel is YoungWitch)
            {
				this.BaneButton.IsVisible = false;
                this.BaneControl.IsVisible = false;
                this.BaneButton.IsToggled = false;
				this.BaneText.IsVisible = false;
				this.Model.BaneCard = null;
                this.pickingBane = false;
            }
        }
		/*
        private void Card_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            FrameworkElement feSender = (FrameworkElement)sender;
            CardViewModel card = (CardViewModel)feSender.DataContext;
            this.CardDetailsPopup.DataContext = card.CardInfo;
            this.CardDetailsPopup.Visibility = Visibility.Visible;
        }
		
        private void CardDetailsClick(object sender, RoutedEventArgs e)
        {
            this.CardDetailsPopup.Visibility = System.Windows.Visibility.Collapsed;
        }
		*/
        private void Click_OK(object sender, EventArgs e)
        {
            if (this.Model.SelectedCards.Count == 10)
            {
                if (App.CurrentGame != null)
                {
                    App.CurrentGame.CancelGame();
                }

                Game game = new LocalGame();
                App.CurrentGame = game;
                IList<CardModel> cards = this.Model.SelectedCards.Select(c => c.CardModel).ToList();
                CardModel bane = this.Model.BaneCard != null ? this.Model.BaneCard.CardModel : null;
                Kingdom kingdom = new Kingdom(cards, App.ProhibitedCards.ProhibitedCards, bane, App.SupportedSets.AllowedSets, 2, App.Settings.UseColonies, App.Settings.UseShelters, App.Settings.StartingHandType, App.Settings.UseRandomCardsFromChosenSetsOnly);
                game.GamePageModel.Kingdom = kingdom;
                game.PlayGame();

				App.Instance.MainPage = new DominionXamarinForms.GamePage();
			}
        }

        private void Click_Bane(object sender, EventArgs e)
        {
            this.pickingBane = true;
        }

        private void Click_Fill(object sender, EventArgs e)
        {
            CardCollection collection = this.Model.CardCollections.First(c => c.Name == "All");
            while (this.Model.SelectedCards.Count < 10)
            {
                CardViewModel card = collection.Cards[Randomizer.Next(collection.Cards.Count)];
                if (!this.Model.SelectedCards.Any(c => c.Name == card.Name) && (this.Model.BaneCard == null || this.Model.BaneCard.Name != card.Name))
                {
                    this.Model.SelectedCards.Add(card);
                }
            }
			this.RedrawCards();
        }

		/*
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = (Pivot)sender;
            CardCollection c = (CardCollection)p.SelectedItem;
            if (c != null)
            {
                if (c.DelayLoadedCards.Count == 0)
                {
                    c.DelayLoadedCards.AddRange(c.Cards);
                }
            }
        }
		*/
    }
}
