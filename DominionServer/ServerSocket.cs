using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dominion;
using Dominion.Network;
using System.Net.Sockets;
using System.Threading;

namespace DominionServer
{
	public class ServerSocket : SocketBase
	{
		//private ManualResetEvent resetEvent = new ManualResetEvent(true);
		private Socket socket;
		private Connection connection;
		public ServerSocket(Socket socket, Connection connection)
		{
			this.socket = socket;
			this.connection = connection;
		}

		protected override void HandleMessage(NetworkMessage message)
		{
			this.connection.HandleMessage(message);
		}

		public void Close()
		{
			this.socket.Close();
		}

		protected override void SendBytes(byte[] bytes)
		{
			if (bytes.Length > 0)
			{
				this.socket.Send(bytes);
			}
		}

		public void Listen()
		{
			byte[] buffer = new byte[1024];
			try
			{
				int bytesTransferred = this.socket.Receive(buffer);
				if(Server.EnableLogging)
				{
					Console.WriteLine("Received socket data" + (this.connection.User != null ? "(" + this.connection.User + ")" : "") + ":" + Encoding.ASCII.GetString(buffer, 0, bytesTransferred));
				}
				this.ProcessBytes(buffer, bytesTransferred);
			}
			catch (SocketException e)
			{
				string elog = "Error";
				if (this.connection.User != null && this.connection.User.Name != null)
				{
					elog += " from " + this.connection.User.Name;
				}
				elog += ": ";
				elog += e.SocketErrorCode.ToString();
				Console.WriteLine(elog);
				this.connection.Status = ConnectionStatus.Exiting;
			}
		}
	}
}
