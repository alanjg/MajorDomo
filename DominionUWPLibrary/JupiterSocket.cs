using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using Dominion;
using Dominion.Network;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace DominionUWP
{
	public class JupiterSocket : SocketBase, ISocket
	{
		private string serverAddress;
		private string id;
		private StreamSocket socket = null;
		private IServerConnection connection;
		private DataWriter writer;
		private DataReader reader;

		protected override void SendBytes(byte[] bytes)
		{
			this.writer.WriteBytes(bytes);
			this.writer.StoreAsync();
		}

		public void SetServerConnection(IServerConnection connection)
		{
			this.connection = connection;
		}

		public bool IsConnected { get { return this.socket != null; } }

		public void Connect(string serverAddress, string username)
		{
			this.serverAddress = serverAddress;
			int port = 4502;
			this.id = username;
			HostName hostName = new HostName(this.serverAddress);
			this.socket = new StreamSocket();
			EndpointPair endpointPair = new EndpointPair(null, string.Empty, hostName, port.ToString());
			StreamSocket s = this.socket;
			IAsyncAction action = s.ConnectAsync(endpointPair);
			action.Completed = new AsyncActionCompletedHandler(this.OnConnected);
		}

		public void Disconnect()
		{
			this.socket.Dispose();
			this.socket = null;
		}

		private void OnConnected(IAsyncAction action, AsyncStatus target)
		{
			if (target == AsyncStatus.Completed)
			{
				this.connection.Connected();

				this.writer = new DataWriter(this.socket.OutputStream);
				this.reader = new DataReader(this.socket.InputStream);
				NetworkMessage m = new NetworkMessage();
				m.MessageCategory = SystemMessages.SystemPrefix;
				m.MessageType = SystemMessages.Connect;
				AuthenticationInfo auth = new AuthenticationInfo();
				auth.Username = this.id;
				m.MessageContent = NetworkSerializer.Serialize(auth);
				this.SendMessage(m);
				this.reader.InputStreamOptions = InputStreamOptions.Partial;
				DataReaderLoadOperation operation = this.reader.LoadAsync(256);
				operation.Completed = new AsyncOperationCompletedHandler<uint>(this.ReaderHasData);
			}
		}

		private void ReaderHasData(IAsyncOperation<uint> operation, AsyncStatus target)
		{
			uint len = operation.GetResults();
			byte[] buffer = new byte[len];
			this.reader.ReadBytes(buffer);

			this.ProcessBytes(buffer, (int)len);
			DataReaderLoadOperation operation2 = this.reader.LoadAsync(256);
			operation2.Completed = new AsyncOperationCompletedHandler<uint>(this.ReaderHasData);
		}

		protected override void HandleMessage(NetworkMessage message)
		{
			this.connection.HandleMessage(message);
		}
	}
}
