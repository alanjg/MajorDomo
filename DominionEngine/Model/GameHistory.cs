using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dominion
{
	public class GameHistory : NotifyingObject, IXmlSerializable
	{
		public GameHistory()
		{
			this.GameRecords = new ObservableCollection<GameRecord>();
		}

		private int wins;
		public int Wins
		{
			get { return this.wins; }
			set { this.wins = value; this.OnPropertyChanged("Wins"); this.OnPropertyChanged("WinRatio"); }
		}
		private int losses;
		public int Losses
		{
			get { return this.losses; }
			set { this.losses = value; this.OnPropertyChanged("Losses"); this.OnPropertyChanged("WinRatio"); }
		}

		public double WinRatio
		{
			get
			{
				if (this.Wins + this.Losses == 0)
				{
					return 1.0;
				}
				else
				{
					return Math.Round((double)this.Wins / (double)(this.Wins + this.Losses), 2);
				}
			}
		}

		public ObservableCollection<GameRecord> GameRecords
		{
			get;
			private set;
		}

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			this.Wins = int.Parse(reader.GetAttribute("Wins"));
			this.Losses = int.Parse(reader.GetAttribute("Losses"));
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Wins", this.Wins.ToString());
			writer.WriteAttributeString("Losses", this.Losses.ToString());
		}
	}
}
