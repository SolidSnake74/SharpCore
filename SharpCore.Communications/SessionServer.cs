using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

using SharpCore.Collections.Generic;

namespace SharpCore.Communications
{
	/// <summary>
	/// Listens for client connections from <see cref="SharpCore.Communications.SessionClient"/> instances.
	/// </summary>
	public sealed class SessionServer : IDisposable
	{
		/// <summary>
		/// Indicates if the current SessionClient has been disposed.
		/// </summary>
		private bool disposed = false;

		/// <summary>
		/// Listens for client connections.
		/// </summary>
		private TcpListener listener;

		/// <summary>
		/// Provides asynchronous functionality for listening for incoming client connections.
		/// </summary>
		private Thread listenThread;

		/// <summary>
		/// Indicates if the SessionServer should continue listening for client communications.
		/// </summary>
		private bool listen;

		/// <summary>
		/// Represents a first-in, first-out collection of remote clients.
		/// </summary>
		private SynchronizedQueue<TcpClient> clientQueue;

		/// <summary>
		/// Occurs when a <see cref="SharpCore.Communications.Communique"/> has been received.
		/// </summary>
		public event CommuniqueReceivedEventHandler CommuniqueReceived;

		/// <summary>
		/// Initializes a new instance of the SessionServer class with the specified <see cref="System.Net.IPAddress"/> and port.
		/// </summary>
		/// <param name="address">The IP address to listen on.</param>
		/// <param name="port">The port to listen on.</param>
		public SessionServer(IPAddress address, int port)
		{
			IPEndPoint endPoint = new IPEndPoint(address, port);
			Initialize(endPoint);
		}

		/// <summary>
		/// Initializes a new instance of the SessionServer class with the specified <see cref="System.Net.IPEndPoint"/>.
		/// </summary>
		/// <param name="endPoint">The IP end point to listen on.</param>
		public SessionServer(IPEndPoint endPoint)
		{
			Initialize(endPoint);
		}

		/// <summary>
		/// Initializes a new instance of the SessionServer class with the specified host name and port.
		/// </summary>
		/// <param name="hostName">The host name that should be resolved to an IP address to listen on.</param>
		/// <param name="port">The port to listen on.</param>
		public SessionServer(string hostName, int port)
		{
			IPEndPoint endPoint = new IPEndPoint(Dns.GetHostEntry(hostName).AddressList[0], port);
			Initialize(endPoint);
		}

		private void Initialize(IPEndPoint endPoint)
		{
			listener = new TcpListener(endPoint);

			clientQueue = new SynchronizedQueue<TcpClient>();

			listenThread = new Thread(new ThreadStart(Listen));
			listenThread.IsBackground = true;
			listenThread.Start();
		}

		/// <summary>
		/// Listens for an incoming client connection and starts a new thread for client communication when a client connects.
		/// </summary>
		private void Listen()
		{
			SynchronizedList<Thread> threadList = new SynchronizedList<Thread>();

			listener.Start();
			listen = true;

			while (listen)
			{
				if (listener.Pending())
				{
					TcpClient client = listener.AcceptTcpClient();

					clientQueue.Enqueue(client);

					Thread thread = new Thread(new ThreadStart(ServiceClient));
					thread.IsBackground = true;
					thread.Start();

					threadList.Add(thread);
				}
				else
				{
					Thread.Sleep(TimeSpan.Zero);
				}
			}

			listener.Stop();

			foreach (Thread thread in threadList)
			{
				thread.Join();
			}
		}

		/// <summary>
		/// Handles communication with a connected client.
		/// </summary>
		private void ServiceClient()
		{
			using (TcpClient client = (TcpClient) clientQueue.Dequeue())
			{
				using (NetworkStream stream = client.GetStream())
				{
					while (listen)
					{
						if (stream.DataAvailable)
						{
							BinaryFormatter formatter = new BinaryFormatter();
							Communique communique = (Communique) formatter.Deserialize(stream);

							CommuniqueReceived(this, new CommuniqueReceviedEventArgs(communique));
						}
						else
						{
							Thread.Sleep(TimeSpan.Zero);
						}
					}
				}
			}
		}

		/// <summary>
		/// Stops the server and prevents additional clients from connecting.
		/// </summary>
		public void Close()
		{
			Dispose(true);
		}

		/// <summary>
		/// Stops the server and prevents additional clients from connecting.
		/// </summary>
		/// <param name="disposing"><b>true</b> if explicit cleanup should be performed, otherwise <b>false</b> (called during garbage collection).</param>
		private void Dispose(bool disposing)
		{
			if (disposed) return;

			lock (this)
			{
				if (disposed == false)
				{
					listen = false;
					if (disposing)
					{
						if (listenThread.IsAlive)
						{
							listenThread.Join();
						}
						System.GC.SuppressFinalize(this);
					}
					disposed = true;
				}
			}
		}

		/// <summary>
		/// Closes all <see cref="SharpCore.Communications.SessionClient"/> connections.
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Allows the server to close all the <see cref="SharpCore.Communications.SessionClient"/> connections.
		/// </summary>
		~SessionServer()
		{
			Dispose(false);
		}
	}
}
