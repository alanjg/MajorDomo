using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Dominion;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.Windows.Data;
using System.Threading;
using Dominion.Network;
using System.IO;
using System.IO.IsolatedStorage;

namespace DominionSL
{
	public class DifficultyModel : NotifyingObject
	{
		public DifficultyModel()
		{
			switch (this.ReadDifficulty())
			{
				case 0:
					this.Easy = true;
					break;
				case 1:
					this.Medium = true;
					break;
				case 2:
					this.Hard = true;
					break;
			}
		}

		public int ReadDifficulty()
		{
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
			return 1;
		}

		public int GetDifficulty()
		{
			if (this.easy) return 0;
			if (this.medium) return 1;
			if (this.hard) return 2;
			// shouldn't get here
			return 1;
		}
		public void WriteDifficulty()
		{
			IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication();

			using (IsolatedStorageFileStream stream = isoStore.CreateFile("difficulty.txt"))
			{
				using (StreamWriter r = new StreamWriter(stream))
				{
					r.WriteLine(this.GetDifficulty());
				}
			}
		}

		private bool easy;
		public bool Easy
		{
			get
			{
				return this.easy;
			}
			set
			{
				this.easy = value;
				this.OnPropertyChanged("Easy");
			}
		}

		private bool medium;
		public bool Medium
		{
			get
			{
				return this.medium;
			}
			set
			{
				this.medium = value;
				this.OnPropertyChanged("Medium");
			}
		}

		private bool hard;
		public bool Hard
		{
			get
			{
				return this.hard;
			}
			set
			{
				this.hard = value;
				this.OnPropertyChanged("Hard");
			}
		}
	}

	public class SizingHelper : NotifyingObject
	{
		private FrameworkElement source;
		public SizingHelper()
		{
		}

		public void SetSource(FrameworkElement source)
		{
			this.source = source;
			this.source.SizeChanged += source_SizeChanged;
			this.UpdateSize(source.ActualWidth, source.ActualHeight);
		}

		private void source_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.UpdateSize(e.NewSize.Width, e.NewSize.Height);
		}

		private void UpdateSize(double width, double height)
		{
			this.PageWidth = width;
			this.PageHeight = height;
			this.CardHeight = Math.Max(this.PageHeight / 6 - 32, 0);
			this.CardWidth = Math.Max(this.PageWidth / 10 - 8, 0);
			this.CardRowHeight = this.CardHeight + 8;
		}

		private double pageWidth;
		public double PageWidth
		{
			get { return this.pageWidth; }
			set { this.pageWidth = value; this.OnPropertyChanged("PageWidth"); }
		}

		private double pageHeight;
		public double PageHeight
		{
			get { return this.pageHeight; }
			set { this.pageHeight = value; this.OnPropertyChanged("PageHeight"); }
		}

		// Width=130, Height=98 are the right values for Surface, e.g. 1300x768
		private double cardWidth;
		public double CardWidth
		{
			get { return this.cardWidth; }
			set { this.cardWidth = value; this.OnPropertyChanged("CardWidth"); }
		}

		private double cardHeight;
		public double CardHeight
		{
			get { return this.cardHeight; }
			set { this.cardHeight = value; this.OnPropertyChanged("CardHeight"); }
		}

		private double cardRowHeight;
		public double CardRowHeight
		{
			get { return this.cardRowHeight; }
			set { this.cardRowHeight = value; this.OnPropertyChanged("CardRowHeight"); }
		}
	}

	public partial class MainPage : UserControl
	{
		private bool forceLocal = false;
		private bool forceMultiplayer = true;
		private GameViewModel gameViewModel;
		private GamePageModel gamePageModel;

		public SizingHelper SizingHelper { get; private set; }

		public MainPage()
		{
			InitializeComponent();
			this.SizingHelper = (SizingHelper)this.Resources["SizingHelper"];
			this.SizingHelper.SetSource(this);
			this.gamePageGrid.Visibility = System.Windows.Visibility.Collapsed;
			if (forceLocal)
			{
				this.StartLocalGame();
			}
			else
			{
				this.Loaded += MainPage_Loaded;
			}			
		}

		

		private void DoMultiplayer(string serverAddress, string username)
		{
			this.MultiplayerGameLobby.Visibility = System.Windows.Visibility.Visible;
			string address = serverAddress;
			this.StartNetworkGame(address, username);
		}

		private void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			if (forceMultiplayer)
			{
				UsernameDialog dialog = new UsernameDialog();
				dialog.Closed += dialog_Closed;
				dialog.Show();
			}
			else
			{
				DoPickGameType();
			}
		}

		void dialog_Closed(object sender, EventArgs e)
		{
			UsernameDialog dialog = (UsernameDialog)sender;
			DoMultiplayer(ServerModel.PublicServerAddress, dialog.Username.Text);
		}

		private void DoPickGameType()
		{
			GameTypePicker picker = new GameTypePicker();
			picker.Closed += picker_Closed;
			picker.AddressTextBox.Text = "168.62.216.215";
			picker.Show();
		}

		private void picker_Closed(object sender, EventArgs e)
		{
			GameTypePicker picker = (GameTypePicker)sender;
			if (picker.IsServerGame)
			{
				this.DoMultiplayer(picker.ServerAddress, picker.Username);
			}
			else
			{
				this.StartLocalGame();
			}
			picker.Visibility = Visibility.Collapsed;
		}

		private void StartNetworkGame(string address, string username)
		{
			App.ServerConnection.Connect(address, username);
			this.MultiplayerGameLobby.Model.ServerGameStarted += Model_ServerGameStarted;
		}

		private void Model_ServerGameStarted(object sender, EventArgs e)
		{
			this.gamePageModel = App.CurrentGame.GamePageModel;
			this.gameViewModel = this.gamePageModel.GameViewModel;
			this.LayoutRoot.DataContext = App.CurrentGame.GamePageModel;
			this.gamePageModel.GameViewModel.GameModel.GameInitialized += GameModel_GameInitialized;
			this.gamePageGrid.Visibility = System.Windows.Visibility.Visible;
		}

		private void StartLocalGame()
		{
			this.gamePageModel = new LocalGamePageModel();
			this.gamePageModel.Kingdom = new Kingdom(null, null, GameSets.Any, 2);
			this.gamePageModel.SetupGame();
			this.gameViewModel = this.gamePageModel.GameViewModel;
			((App)App.Current).GameViewModel = this.gameViewModel;
			((App)App.Current).GamePageModel = this.gamePageModel;
			this.LayoutRoot.DataContext = this.gamePageModel;
			this.gamePageModel.GameViewModel.GameModel.GameInitialized += GameModel_GameInitialized;
			Thread thread = new Thread(new ThreadStart(this.gamePageModel.PlayGame));
			thread.Start();
		}

		void GameModel_GameInitialized(object sender, EventArgs e)
		{
			this.Dispatcher.BeginInvoke(new Action(() =>
			{
				this.MultiplayerGameLobby.Visibility = System.Windows.Visibility.Collapsed;


				this.gamePageModel.GameViewModel.GameModel.GameInitialized -= GameModel_GameInitialized;

				this.gamePageModel.GameViewModel.TextLog.PropertyChanged += TextLog_PropertyChanged;
				this.gamePageModel.GameException += gamePageModel_GameException;
				if (this.gamePageModel.GameViewModel.GameModel.ExtraPiles.Count == 0 && this.gamePageModel.GameViewModel.KingdomPiles.Count == 10)
				{
					Grid.SetRow(this.CardScrollViewer, 2);
					Grid.SetRowSpan(this.CardScrollViewer, 4);
				}
				else
				{
					Grid.SetRow(this.CardScrollViewer, 3);
					Grid.SetRowSpan(this.CardScrollViewer, 3);
				}
			}));
		}

		private void gamePageModel_GameException(Exception e)
		{
			this.LogScrollViewer.Content = new TextBlock() { Text = e.StackTrace };
			this.StatusText.Text = e.Message;
		}

		void TextLog_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Text")
			{
				this.Dispatcher.BeginInvoke(new Action(() =>
				{
					this.LogScrollViewer.ScrollToVerticalOffset(((FrameworkElement)this.LogScrollViewer.Content).ActualHeight + this.LogScrollViewer.ActualHeight);
				}));
			}
		}

		private void gameModeChooserControl_Go(object sender, EventArgs e)
		{
			Thread thread = new Thread(new ThreadStart(this.gameViewModel.GameModel.PlayGame));
			thread.Start();
		}

		

		private void Card_Tap(object sender, RoutedEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			CardViewModel card = (CardViewModel)feSender.DataContext;
			this.gamePageModel.InvokeCard(card);
		}

		private void Pile_Tap(object sender, RoutedEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			PileViewModel pile = (PileViewModel)feSender.DataContext;
			this.gamePageModel.InvokePile(pile);
		}

		private void Effect_Tap(object sender, RoutedEventArgs e)
		{
			FrameworkElement feSender = (FrameworkElement)sender;
			EffectViewModel effect = (EffectViewModel)feSender.DataContext;
			this.gamePageModel.InvokeEffect(effect);
		}

		private void BuyPhase_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.EnterBuyPhase();
		}

		private void PlayAll_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.PlayAll();
		}

		private void PlayCoins_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.PlayCoinTokens();
		}

		private void EndTurn_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.EndTurn();
		}

		private void OK_Click(object sender, RoutedEventArgs e)
		{
			this.gamePageModel.MakeChoice();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			App.CurrentGame.ExitGame();
			App.CurrentGame = null;

			this.gamePageGrid.Visibility = System.Windows.Visibility.Collapsed;
			this.MultiplayerGameLobby.Visibility = System.Windows.Visibility.Visible;

		}
	}
}
