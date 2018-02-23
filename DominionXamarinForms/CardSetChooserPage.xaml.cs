using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Dominion;

namespace DominionXamarinForms
{
    public partial class CardSetChooserPage : ContentPage
    {
        public CardSetsModel Model { get; private set; }
        private void CardSetTap(object sender, EventArgs e)
        {
            this.Model.SelectedCardSet = (CardSetViewModel)((View)sender).BindingContext;
        }

        public CardSetChooserPage()
        {
            this.Model = App.CardSetsModel;
            this.Model.SelectedCardSet = this.Model.CardSetGroups.First().CardSets.First();
            this.BindingContext = this.Model;
            InitializeComponent();
            foreach (CardSetGroup group in this.Model.CardSetGroups)
            {
                ScrollView scrollView = new ScrollView();
                scrollView.Orientation = ScrollOrientation.Vertical;
                this.GroupsLayout.Children.Add(scrollView);

                StackLayout setLayout = new StackLayout();
                setLayout.Orientation = StackOrientation.Vertical;
                scrollView.Content = setLayout;

                Label l = new Label();
                l.Text = group.GameSet.SetName;
                setLayout.Children.Add(l);
                foreach(CardSetViewModel cardSet in group.CardSets)
                {
                    Button button = new Button();
                    button.BindingContext = cardSet;
                    button.Text = cardSet.CardSetName;
                    button.Clicked += CardSet_Clicked;
                    setLayout.Children.Add(button);
                }
            }
        }

        private void CardSet_Clicked(object sender, EventArgs e)
        {
            this.Model.SelectedCardSet = (CardSetViewModel)((Button)sender).BindingContext;
            this.SelectedCardSetLayout.Children.Clear();
            foreach(CardModel card in this.Model.SelectedCardSet.CardSet.CardCollection)
            {
                Label l = new Label();
                l.Text = card.Name;
                this.SelectedCardSetLayout.Children.Add(l);
            }
        }

        private void Click_OK(object sender, EventArgs e)
        {
            if (App.CurrentGame != null)
            {
                App.CurrentGame.CancelGame();
            }

            Game game = new LocalGame();
            App.CurrentGame = game;
            IEnumerable<CardModel> cards;
            CardModel bane = null;
            if (this.Model.SelectedCardSet != null)
            {
                cards = this.Model.SelectedCardSet.CardSet.CardCollection;
                bane = this.Model.SelectedCardSet.CardSet.BaneCard;
            }
            else
            {
                cards = new RandomAllCardSet(App.SupportedSets.AllowedSets).CardCollection;
            }

            Kingdom kingdom = new Kingdom(cards.ToList(), App.ProhibitedCards.ProhibitedCards, bane, App.SupportedSets.AllowedSets, 2, App.Settings.UseColonies, App.Settings.UseShelters, App.Settings.StartingHandType, App.Settings.UseRandomCardsFromChosenSetsOnly);

            game.GamePageModel.Kingdom = kingdom;
            game.PlayGame();

           App.Instance.MainPage = new DominionXamarinForms.GamePage();
        }
    }
}
