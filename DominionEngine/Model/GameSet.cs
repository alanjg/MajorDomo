using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;


namespace Dominion
{
	[Flags]
	public enum GameSets
	{
		None = 0x0,
		Base = 0x1,
		Intrigue = 0x2,
		Seaside = 0x4,
		Alchemy = 0x8,
		Prosperity = 0x10,
		Cornucopia = 0x20,
		Hinterlands = 0x40,
		DarkAges = 0x80,
		Guilds = 0x100,
		Promo = 0x200,
		Adventures = 0x400,
		Any = 0xFFFFFFF,
	}

	public class GameSet : INotifyPropertyChanged
	{
		private bool isSelected;

		public GameSets Set
		{
			get;
			private set;
		}

		public bool Selected
		{
			get { return this.isSelected; }
			set
			{
				if (this.isSelected != value)
				{
					this.isSelected = value;
					this.OnPropertyChanged("Selected");
				}
			}
		}

		public string SetName
		{
			get;
			private set;
		}

		public GameSet(GameSets gameSet, bool selected)
		{
			this.Selected = selected;
			this.Set = gameSet;
			this.SetName = Enum.GetName(typeof(GameSets), gameSet);
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
	}

	public class GameSetCollection : IXmlSerializable
	{
		public ObservableCollection<GameSet> GameSets { get; private set; }
		public GameSets AllowedSets { get { return this.GameSets.Aggregate(Dominion.GameSets.None, (GameSets before, GameSet item) => { return before | (item.Selected ? item.Set : Dominion.GameSets.None); }); } }
		public GameSetCollection()
		{
			this.GameSets = new ObservableCollection<GameSet>();
			foreach (GameSets gameSet in Enum.GetValues(typeof(GameSets)))
			{
				if (gameSet != Dominion.GameSets.Any && gameSet != Dominion.GameSets.None)
				{
					this.GameSets.Add(new GameSet(gameSet, true));
				}
			}
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			foreach (GameSet gameSet in this.GameSets)
			{
				bool supported = true;
				bool.TryParse(reader.GetAttribute(gameSet.SetName), out supported);
				gameSet.Selected = supported;
			}
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			foreach(GameSet gameSet in this.GameSets)
			{
				writer.WriteAttributeString(gameSet.SetName, gameSet.Selected.ToString());
			}
		}

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}
	}

	public class ProhibitedCardsCollection : NotifyingObject, IXmlSerializable
	{
		public override string ToString()
		{
			StringBuilder result = new StringBuilder();
			for(int i=0;i<this.ProhibitedCards.Count;i++)
			{
				if (i != 0)
				{
					result.Append(",");
				}
				result.Append(this.ProhibitedCards[i].ID);
			}
			return result.ToString();
		}

		public void ReadString(string cardString)
		{
			this.ProhibitedCards.Clear();
			string[] cards = cardString.Split(',');
			foreach (string card in cards)
			{
				CardModel c = CardModelFactory.GetCardModel(card.Trim());
				if(c != null)
				{
					this.ProhibitedCards.Add(c);
				}				
			}
		}

		public string ProhibitedCardsString
		{
			get
			{
				return this.ToString();
			}
			set
			{
				this.ReadString(value);
				this.OnPropertyChanged("ProhibitedCardsString");
			}
		}

		public List<CardModel> ProhibitedCards { get; private set; }
		public ProhibitedCardsCollection()
		{
			this.ProhibitedCards = new List<CardModel>();
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			int count = int.Parse(reader.GetAttribute("Count"));
			reader.ReadStartElement();
			for (int i = 0; i < count;i++)
			{
				this.ProhibitedCards.Add(CardModelFactory.GetCardModel(reader.ReadElementContentAsString()));
			}				
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Count", this.ProhibitedCards.Count.ToString());
			foreach (CardModel card in this.ProhibitedCards)
			{				
				writer.WriteElementString("Card", card.ID);
			}
		}

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}
	}
}