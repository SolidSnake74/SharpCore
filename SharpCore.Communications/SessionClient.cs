using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharpCore.Communications
{
	/// <summary>
	/// Represents a client session with a <see cref="SharpCore.Communications.SessionServer"/>.
	/// </summary>
	public sealed class SessionClient : IDisposable
	{
		/// <summary>
		/// Indicates if the current SessionClient has been disposed.
		/// </summary>
		private bool disposed = false;

		/// <summary>
		/// The <see cref="System.Net.Sockets.TcpClient"/> used to connect to the <see cref="SharpCore.Communications.SessionServer"/>.
		/// </summary>
		private TcpClient client;

		/// <summary>
		/// The <see cref="System.Net.Sockets.NetworkStream"/> used to stream data to and from the <see cref="SharpCore.Communications.SessionServer"/>.
		/// </summary>
		private NetworkStream stream;

		/// <summary>
		/// Serializes and deserializes <see cref="SharpCore.Communications.Communique"/> instances in binary format.
		/// </summary>
		private BinaryFormatter formatter;

		/// <summary>
		/// Initializes a new instance of the SessionClient class with the specified <see cref="System.Net.IPAddress"/> and port.
		/// </summary>
		/// <param name="serverAddress">The IP address of the server to connect to.</param>
		/// <param name="serverPort">The port on which a connection to the server should be made.</param>
		public SessionClient(IPAddress serverAddress, int serverPort)
		{
			IPEndPoint serverEndPoint = new IPEndPoint(serverAddress, serverPort);
			Initialize(serverEndPoint);
		}

		/// <summary>
		/// Initializes a new instance of the SessionClient class with the specified <see cref="System.Net.IPEndPoint"/>.
		/// </summary>
		/// <param name="serverEndPoint">The IP end point of the server to connect to.</param>
		public SessionClient(IPEndPoint serverEndPoint)
		{
			Initialize(serverEndPoint);
		}

		/// <summary>
		/// Initalizes a new instance of the SessionClient class with the specified host name and port.
		/// </summary>
		/// <param name="serverHostName">The host name of the server to connect to.</param>
		/// <param name="serverPort">The port on which a connection to the server should be made.</param>
		public SessionClient(string serverHostName, int serverPort)
		{
			IPEndPoint serverEndPoint = new IPEndPoint(Dns.GetHostEntry(serverHostName).AddressList[0], serverPort);
			Initialize(serverEndPoint);
		}

		/// <summary>
		/// Initializes a new instance of the SessionClient class with the specified <see cref="System.Net.IPEndPoint"/>.
		/// </summary>
		/// <param name="serverEndPoint">The IP end point of the server to connect to.</param>
		private void Initialize(IPEndPoint serverEndPoint)
		{
			client = new TcpClient();
			client.Connect(serverEndPoint);
			stream = client.GetStream();
			formatter = new BinaryFormatter();
		}

		/// <summary>
		/// Sends a Communique to a remote <see cref="SharpCore.Communications.SessionServer"/>.
		/// </summary>
		public void SendCommunique(Communique communique)
		{
			if (disposed) throw new ObjectDisposedException("SessionClient");
			formatter.Serialize(stream, new Communique(communique.Body, communique.HostName, DateTime.Now, DateTime.MinValue));
		}

		/// <summary>
		/// Disconnects the client from the remote <see cref="SharpCore.Communications.SessionServer"/>.
		/// </summary>
		public void Close()
		{
			Dispose(true);
		}

		/// <summary>
		/// Disconnects the client from the remote <see cref="SharpCore.Communications.SessionServer"/>.
		/// </summary>
		/// <param name="disposing"><b>true</b> if explicit cleanup should be performed, otherwise <b>false</b> (called during garbage collection).</param>
		private void Dispose(bool disposing)
		{
			if (disposed) return;

			if (disposing)
			{
				if (stream != null) stream.Close();
				if (client != null) client.Close();
				System.GC.SuppressFinalize(this);
				disposed = true;
			}
		}

		/// <summary>
		/// Disconnects the client from the remote <see cref="SharpCore.Communications.SessionServer"/>.
		/// </summary>
		void IDisposable.Dispose()
		{
			Close();
		}

		/// <summary>
		/// Allows the client to disconnect from the remote <see cref="SharpCore.Communications.SessionServer"/>.
		/// </summary>
		~SessionClient()
		{
			Dispose(false);
		}
	}
}
