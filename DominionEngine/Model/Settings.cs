using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dominion
{
	public class Settings : IXmlSerializable
	{
		private const string showPlayerScoreAttributeString = "ShowPlayerScore";
		public bool ShowPlayerScore { get; set; }

		private const string startingHandTypeAttributeString = "StartingHandType";
		
		public StartingHandType StartingHandType { get; set; }
		public List<StartingHandType> StartingHandTypeChoices { get; private set; }

		private const string useColoniesAttributeString = "UseColonies";
		public CardUseType UseColonies { get; set; }
		public List<CardUseType> UseColoniesChoices { get; private set; }

		private const string useSheltersAttributeString = "UseShelters";
		public CardUseType UseShelters { get; set; }
		public List<CardUseType> UseSheltersChoices { get; private set; }

		private const string useRandomCardsFromChosenSetsOnlyAttributeString = "UseRandomCardsFromChosenSetsOnly";
		public bool UseRandomCardsFromChosenSetsOnly { get; set; }

		public Settings()
		{
			this.StartingHandTypeChoices = new List<StartingHandType>(Enum.GetValues(typeof(StartingHandType)).Cast<StartingHandType>());
			this.UseColoniesChoices = new List<CardUseType>(Enum.GetValues(typeof(CardUseType)).Cast<CardUseType>());
			this.UseSheltersChoices = new List<CardUseType>(Enum.GetValues(typeof(CardUseType)).Cast<CardUseType>());
			this.ShowPlayerScore = true;
			this.StartingHandType = StartingHandType.Random;
			this.UseColonies = CardUseType.RandomByCardsFromSet;
			this.UseShelters = CardUseType.RandomByCardsFromSet;
			this.UseRandomCardsFromChosenSetsOnly = false;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			this.ShowPlayerScore = true;
			this.StartingHandType = StartingHandType.Random;
			this.UseColonies = CardUseType.RandomByCardsFromSet;
			this.UseShelters = CardUseType.RandomByCardsFromSet;

			string showPlayerScore = reader.GetAttribute(showPlayerScoreAttributeString);
			if (showPlayerScore != null)
			{
				this.ShowPlayerScore = bool.Parse(showPlayerScore);
			}

			string startingHandType = reader.GetAttribute(startingHandTypeAttributeString);
			if (startingHandType != null)
			{
				this.StartingHandType = (StartingHandType)Enum.Parse(typeof(StartingHandType), startingHandType, false);
			}

			string useColonies = reader.GetAttribute(useColoniesAttributeString);
			if (useColonies != null)
			{
				this.UseColonies = (CardUseType)Enum.Parse(typeof(CardUseType), useColonies, false);
			}

			string useShelters = reader.GetAttribute(useSheltersAttributeString);
			if (useShelters != null)
			{
				this.UseShelters = (CardUseType)Enum.Parse(typeof(CardUseType), useShelters, false);
			}
			
			string useRandomCards = reader.GetAttribute(useRandomCardsFromChosenSetsOnlyAttributeString);
			if (useRandomCards != null)
			{
				this.UseRandomCardsFromChosenSetsOnly = bool.Parse(useRandomCards);
			}
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString(showPlayerScoreAttributeString, this.ShowPlayerScore.ToString());
			writer.WriteAttributeString(startingHandTypeAttributeString, this.StartingHandType.ToString());
			writer.WriteAttributeString(useColoniesAttributeString, this.UseColonies.ToString());
			writer.WriteAttributeString(useSheltersAttributeString, this.UseShelters.ToString());
			writer.WriteAttributeString(useRandomCardsFromChosenSetsOnlyAttributeString, this.UseRandomCardsFromChosenSetsOnly.ToString());
		}

		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}
	}
}
