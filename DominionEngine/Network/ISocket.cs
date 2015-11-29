using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion.Network
{
	public interface ISocket
	{
		void SendMessage(NetworkMessage message);
		void SetServerConnection(IServerConnection connection);
		void Connect(string address, string username);
		void Disconnect();
		bool IsConnected { get; }
	}

	public abstract class SocketBase
	{
		enum MessageState
		{
			WaitingForHeader,
			WaitingForContent
		}

		private MessageState state = MessageState.WaitingForHeader;
		private List<byte[]> pendingBytes = new List<byte[]>();
		private int messageLength;
		private byte[] GetBytes(int length)
		{
			byte[] result;
			if (this.pendingBytes[0].Length == length)
			{
				result = this.pendingBytes[0];
				this.pendingBytes.RemoveAt(0);
				return result;
			}
			else
			{
				result = new byte[length];
				int at = 0;
				while (length > 0 && length >= this.pendingBytes[0].Length)
				{
					Array.Copy(this.pendingBytes[0], 0, result, at, this.pendingBytes[0].Length);
					length -= this.pendingBytes[0].Length;
					at += this.pendingBytes[0].Length;
					this.pendingBytes.RemoveAt(0);
				}

				if (length > 0)
				{
					// partial cleanup
					byte[] leftOver = new byte[this.pendingBytes[0].Length - length];
					Array.Copy(this.pendingBytes[0], 0, result, at, length);
					Array.Copy(this.pendingBytes[0], length, leftOver, 0, this.pendingBytes[0].Length - length);
					this.pendingBytes[0] = leftOver;
				}
				return result;
			}
		}
		public void ProcessBytes(byte[] bytes, int length)
		{
			if (length == bytes.Length)
			{
				this.pendingBytes.Add(bytes);
			}
			else
			{
				byte[] buffer = new byte[length];
				Array.Copy(bytes, 0, buffer, 0, length);
				this.pendingBytes.Add(buffer);
			}

			bool go = true;
			while (go)
			{
				if(this.state == MessageState.WaitingForHeader)
				{
					go = this.TryProcessHeader();
				}
				if(this.state == MessageState.WaitingForContent)
				{
					go = this.TryProcessContent();
				}
			}
		}

		private bool TryProcessHeader()
		{
			if (this.pendingBytes.Sum(ba => ba.Length) >= 8)
			{
				//get header + length
				byte[] bytes = this.GetBytes(8);
				int length = 0;
				if (NetworkMessage.CheckHeaderMatchAndGetLength(bytes, out length))
				{
					this.state = MessageState.WaitingForContent;
					this.messageLength = length;
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool TryProcessContent()
		{
			if (this.pendingBytes.Sum(ba => ba.Length) >= this.messageLength)
			{
				byte[] bytes = this.GetBytes(this.messageLength);
				NetworkMessage message = new NetworkMessage();
				string m = UTF8Encoding.UTF8.GetString(bytes, 0, bytes.Length);
				int spaceIndex = m.IndexOf(' ');
				int pipeIndex = m.IndexOf('|');
				message.MessageCategory = m.Substring(0, pipeIndex);
				message.MessageType = m.Substring(pipeIndex + 1, spaceIndex - pipeIndex - 1);
				message.MessageContent = m.Substring(spaceIndex + 1);
				this.HandleMessage(message);
				
				this.messageLength = 0;
				this.state = MessageState.WaitingForHeader;
				return true;
			}
			else
			{
				return false;
			}
		}

		public SocketBase()
		{
		}

		public void SendMessage(NetworkMessage message)
		{
			SocketBase.Send(message, this.SendBytes);
		}

		public static void Send(NetworkMessage message, Action<byte[]> sendBytesMethod)
		{
			int headerSize = 8;
			byte[] messageCategoryBytes = UTF8Encoding.UTF8.GetBytes(message.MessageCategory);
			byte[] pipeBytes = UTF8Encoding.UTF8.GetBytes("|");
			byte[] messageTypeBytes = UTF8Encoding.UTF8.GetBytes(message.MessageType);
			byte[] messageSpaceBytes = UTF8Encoding.UTF8.GetBytes(" ");
			byte[] messageContentBytes = UTF8Encoding.UTF8.GetBytes(message.MessageContent);
			int length = messageContentBytes.Length + messageSpaceBytes.Length + messageTypeBytes.Length + pipeBytes.Length + messageCategoryBytes.Length;

			byte[] headerBytes = new byte[headerSize];
			headerBytes[0] = NetworkMessage.ProtocolHeader[0];
			headerBytes[1] = NetworkMessage.ProtocolHeader[1];
			headerBytes[2] = NetworkMessage.ProtocolMajorVersion;
			headerBytes[3] = NetworkMessage.ProtocolMinorVersion;
			for (int i = 7; i >= 4; i--)
			{
				headerBytes[i] = (byte)(length % 256);
				length /= 256;
			}

			sendBytesMethod(headerBytes);
			sendBytesMethod(messageCategoryBytes);
			sendBytesMethod(pipeBytes);
			sendBytesMethod(messageTypeBytes);
			sendBytesMethod(messageSpaceBytes);
			sendBytesMethod(messageContentBytes);
		}

		protected abstract void SendBytes(byte[] bytes);
		protected abstract void HandleMessage(NetworkMessage message);
	}
}
