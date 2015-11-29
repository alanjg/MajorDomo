using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Dominion;
using Dominion.Network;

namespace DominionPhone
{
	public class WPSocket : SocketBase, ISocket
	{
		private string serverAddress;
		private string id;
		private EndPoint endpoint;
		private Socket socket = null;
		private IServerConnection connection;
		private SocketAsyncEventArgs pendingReceiveArgs;

		protected override void SendBytes(byte[] bytes)
		{
			if (bytes.Length > 0)
			{
				SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
				sendArgs.SetBuffer(bytes, 0, bytes.Length);
				this.socket.SendAsync(sendArgs);
			}
		}


		public void SetServerConnection(IServerConnection connection)
		{
			this.connection = connection;
		}

		private void ReceiveMessage()
		{
			SocketAsyncEventArgs recArgs = new SocketAsyncEventArgs();
			recArgs.Completed += new EventHandler<SocketAsyncEventArgs>(recArgs_Completed);
			this.pendingReceiveArgs = recArgs;
			byte[] bytes = new byte[256];
			recArgs.SetBuffer(bytes, 0, 256);
			if (!this.socket.ReceiveAsync(recArgs))
			{
				OnReceive(recArgs);
			}
		}

		private void recArgs_Completed(object sender, SocketAsyncEventArgs e)
		{
			OnReceive(e);
		}

		private void OnReceive(SocketAsyncEventArgs e)
		{
			e.Completed -= recArgs_Completed;
			if (e.BytesTransferred == 0)
			{
				this.Disconnect();
				return;
			}
			this.ProcessBytes(e.Buffer, e.BytesTransferred);

			// Cancel if disposed
			if (this.socket != null)
			{
				ReceiveMessage();
			}
		}

		public void Connect(string serverAddress, string username) 
		{
			this.serverAddress = serverAddress;
			int port = 4502;
			this.id = username;
			this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.endpoint = new DnsEndPoint(this.serverAddress, port);
			SocketAsyncEventArgs args = new SocketAsyncEventArgs();
			args.RemoteEndPoint = this.endpoint;
			args.Completed += new EventHandler<SocketAsyncEventArgs>(e_Completed);
			bool async = this.socket.ConnectAsync(args);
			if (!async)
			{
				OnConnect(args);
			}
		}

		public void Disconnect()
		{
			if(this.pendingReceiveArgs != null)
			{
				this.pendingReceiveArgs.Dispose();
			}
			this.socket.Dispose();
			this.socket = null;
		}

		public bool IsConnected { get { return this.socket != null; } }

		private void e_Completed(object sender, SocketAsyncEventArgs e)
		{
			OnConnect(e);
		}

		void OnConnect(SocketAsyncEventArgs e)
		{
			if (e.SocketError != SocketError.Success)
			{
				ViewModelDispatcher.BeginInvoke(new Action(() =>
				{
					MessageBox.Show("Socket error:" + e.SocketError.ToString());
				}));
				return;
			}
			e.Completed -= new EventHandler<SocketAsyncEventArgs>(e_Completed);

			NetworkMessage message = new NetworkMessage();
			message.MessageCategory = SystemMessages.SystemPrefix;
			message.MessageType = SystemMessages.Connect;
			AuthenticationInfo auth = new AuthenticationInfo();
			auth.Username = this.id;
			message.MessageContent = NetworkSerializer.Serialize(auth);
			this.connection.Connected();
			SendMessage(message);
			ReceiveMessage();
		}

		protected override void HandleMessage(NetworkMessage message)
		{
			this.connection.HandleMessage(message);
		}
	}
}
