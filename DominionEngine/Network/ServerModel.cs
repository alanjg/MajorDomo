using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dominion.Network
{
	public class ServerModel : IXmlSerializable
	{
		public const string PublicServerAddress = "majordomoserver.cloudapp.net";

		public ServerModel()
		{
			this.ServerAddress = string.Empty;
			this.UserName = string.Empty;
		}

		public string ServerAddress { get; set; }
		public bool IsValidAddress { get; set; }
		public string UserName { get; set; }
		public bool HasEnabledMultiplayer { get; set; }
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		public void ReadXml(System.Xml.XmlReader reader)
		{
			reader.MoveToContent();
			this.ServerAddress = reader.GetAttribute("ServerAddress");
			this.IsValidAddress = bool.Parse(reader.GetAttribute("IsValidAddress"));
			this.UserName = reader.GetAttribute("UserName");
			string has = reader.GetAttribute("HasEnabledMultiplayer");
			if (has != null)
			{
				this.HasEnabledMultiplayer = bool.Parse(has);
			}
		}

		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("ServerAddress", this.ServerAddress.ToString());
			writer.WriteAttributeString("IsValidAddress", this.IsValidAddress.ToString());
			writer.WriteAttributeString("UserName", this.UserName);
			writer.WriteAttributeString("HasEnabledMultiplayer", this.HasEnabledMultiplayer.ToString());
		}
	}
}
