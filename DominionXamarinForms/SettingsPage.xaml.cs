using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Dominion;

namespace DominionXamarinForms
{
    public class DifficultyModel : NotifyingObject
    {
        public DifficultyModel()
        {
            this.difficultyValue = this.ReadDifficulty();
        }

        public int ReadDifficulty()
        {
            /*
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            if (isoStore.FileExists("difficulty.txt"))
            {
                using (IsolatedStorageFileStream stream = isoStore.OpenFile("difficulty.txt", System.IO.FileMode.Open))
                {
                    StreamReader r = new StreamReader(stream);
                    string d = r.ReadLine();
                    int i;
                    if (int.TryParse(d, out i))
                    {
                        return i;
                    }
                }
            }
            */
            return 1;
        }

        public int GetDifficulty()
        {
            return this.difficultyValue;
        }


        public void WriteDifficulty()
        {
            /*
            IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

            using (IsolatedStorageFileStream stream = isoStore.CreateFile("difficulty.txt"))
            {
                using (StreamWriter r = new StreamWriter(stream))
                {
                    r.WriteLine(this.GetDifficulty());
                }
            }
            */
        }

        private int difficultyValue;
        public int DifficultyValue
        {
            get { return this.difficultyValue; }
            set { this.difficultyValue = value; }
        }
    }

    public partial class SettingsPage : ContentPage
    {
        private int DifficultyIndex { get; set; }
        public SettingsPage()
        {
            InitializeComponent();


            //this.GameSetsList.BindingContext = App.SupportedSets;
            this.BindingContext = App.Settings;
            //this.ServerAddressTextBox.BindingContext = App.ServerModel;
            //this.EnableMultiplayerCheckBox.BindingContext = App.ServerModel;
            //this.UserNameTextBox.BindingContext = App.ServerModel;
            //this.ProhibitedCardsTextBox.BindingContext = App.ProhibitedCards;

            this.DifficultyPicker.Items.Add("Easy");
            this.DifficultyPicker.Items.Add("Medium");
            this.DifficultyPicker.Items.Add("Hard");
            this.DifficultyPicker.SelectedIndexChanged += (sender, args) =>
            {
                if (this.DifficultyPicker.SelectedIndex != -1)
                {
                    App.Difficulty.DifficultyValue = this.DifficultyPicker.SelectedIndex;
                }
            };

            this.UseColoniesPicker.SelectedIndexChanged += (sender, args) =>
            {
                if (this.UseColoniesPicker.SelectedIndex != -1)
                {
                    App.Settings.UseColonies = App.Settings.UseColoniesChoices[this.UseColoniesPicker.SelectedIndex];
                }
            };
        }
    }
}
