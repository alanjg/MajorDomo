using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dominion
{
	public class PlayerRecord
	{
		public string Name { get; set; }
		public int Score { get; set; }
		public string Deck { get; set; }
	}

	public class GameRecord : IXmlSerializable
	{
		public GameRecord()
		{
			this.Players = new List<PlayerRecord>();
			this.Log = new List<string>();
		}
		public string Name { get; set; }
		public bool Won { get; set; }
		
		public List<PlayerRecord> Players { get; private set; }
		public List<string> Log { get; private set; }

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			this.Name = reader.GetAttribute("Name");
			
			this.Won = bool.Parse(reader.GetAttribute("Won"));
			
			reader.ReadStartElement();
			// players
			int playerCount = int.Parse(reader.GetAttribute("Count"));
			for (int i = 0; i < playerCount; i++)
			{
				// opponent
				if (i == 0)
				{
					reader.ReadStartElement();
				}
				PlayerRecord player = new PlayerRecord();
				this.Players.Add(player);
				
				player.Name = reader.GetAttribute("Name");
				player.Score = int.Parse(reader.GetAttribute("Score"));
				
				// deck
				reader.ReadStartElement();
				player.Deck = reader.ReadElementContentAsString();
				reader.ReadEndElement();
			}
			reader.ReadEndElement();
			
			// turns
			int turnCount = int.Parse(reader.GetAttribute("Count"));
			reader.ReadStartElement();
			for (int i = 0; i < turnCount; i++)
			{
				this.Log.Add(reader.ReadElementContentAsString());
			}
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Name", this.Name.ToString());
			writer.WriteAttributeString("Won", this.Won.ToString());
			
			writer.WriteStartElement("Players");
			writer.WriteAttributeString("Count", this.Players.Count.ToString());
			for (int i = 0; i < Players.Count; i++)
			{
				writer.WriteStartElement("Player");
				writer.WriteAttributeString("Name", this.Players[i].Name);
				writer.WriteAttributeString("Score", this.Players[i].Score.ToString());
				writer.WriteElementString("Deck", this.Players[i].Deck);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteStartElement("Log");
			writer.WriteAttributeString("Count", this.Log.Count.ToString());
			for (int i = 0; i < this.Log.Count;i++)
			{
				writer.WriteElementString("Turn", this.Log[i]);
			}
			writer.WriteEndElement();
		}
	}
}
