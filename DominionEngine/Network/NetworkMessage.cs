using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Dominion.Network
{
	public class NetworkMessage
	{
		public NetworkMessage()
		{
			this.MessageContent = string.Empty;
		}
		public static byte[] ProtocolHeader = new byte[] { (byte)'M', (byte)'D' };
		public const byte ProtocolMajorVersion = 1;
		public const byte ProtocolMinorVersion = 4;

		public static bool CheckHeaderMatchAndGetLength(byte[] header, out int length)
		{
			Debug.Assert(header.Length == 8);
			length = 0;
			if (header.Length != 8)
			{
				return false;
			}
			if (header[0] != NetworkMessage.ProtocolHeader[0] || header[1] != NetworkMessage.ProtocolHeader[1] || header[2] != NetworkMessage.ProtocolMajorVersion || header[3] != NetworkMessage.ProtocolMinorVersion)
			{
				return false;
			}

			for (int i = 0; i < 4;i++)
			{
				length *= 256;
				length += header[i + 4];
			}
			return true;
		}

		// System or Game
		public string MessageCategory { get; set; }
		// Message Type(e.g. PlayCard, EnterLobby
		public string MessageType { get; set; }
		// Actual message payload
		public string MessageContent { get; set; }

		public override string ToString()
		{
			return this.MessageCategory + "|" + this.MessageType + " " + this.MessageContent;
		}
	}



	public static class NetworkSerializer
	{
		public static string Serialize(object objectToSerialize)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				DataContractJsonSerializer serializer =
						new DataContractJsonSerializer(objectToSerialize.GetType());

				serializer.WriteObject(ms, objectToSerialize);
				ms.Position = 0;

				using (StreamReader reader = new StreamReader(ms))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public static T Deserialize<T>(string jsonString)
		{
			using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString)))
			{
				DataContractJsonSerializer serializer =
						new DataContractJsonSerializer(typeof(T));

				return (T)serializer.ReadObject(ms);
			}
		}
	}
	
}
