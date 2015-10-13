using System;
using System.Net;

namespace SharpCore.Communications
{
	/// <summary>
	/// Represents a message to be sent to a SessionServer.
	/// </summary>
	[Serializable]
	public sealed class Communique
	{
		/// <summary>
		/// The body of the Communique.
		/// </summary>
		private object body;

		/// <summary>
		/// The host name of the originating server.
		/// </summary>
		private string hostName;

		/// <summary>
		/// The date and time the communique was sent.
		/// </summary>
		private DateTime sentTime;

		/// <summary>
		/// The date and time the communique was received by the client.
		/// </summary>
		private DateTime arrivedTime;

		/// <summary>
		/// Intializes a new instance of the Communique class with an empty communique body.
		/// </summary>
		public Communique()
			: this(null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the Communique class with the specified communique body.
		/// </summary>
		/// <param name="body">The body of the Communique.</param>
		public Communique(object body)
			: this(body, Dns.GetHostName(), DateTime.MinValue, DateTime.MinValue)
		{
		}

		/// <summary>
		/// Initializes a new isntance of the Commuique class.
		/// </summary>
		/// <param name="body">The body of the Communique.</param>
		/// <param name="hostName">The host name of the originating server.</param>
		/// <param name="sentTime">The date and time the communique was sent.</param>
		/// <param name="arrivedTime">The date and time the communique was received by the client.</param>
		internal Communique(object body, string hostName, DateTime sentTime, DateTime arrivedTime)
		{
			this.body = body;
			this.hostName = hostName;
			this.sentTime = sentTime;
			this.arrivedTime = arrivedTime;
		}

		/// <summary>
		/// The body of the Communique.
		/// </summary>
		public object Body
		{
			get { return body; }
			set { body = value; }
		}

		/// <summary>
		/// The host name of the originating server.
		/// </summary>
		public string HostName
		{
			get { return hostName; }
		}

		/// <summary>
		/// The date and time the communique was sent.
		/// </summary>
		public DateTime SentTime
		{
			get { return sentTime; }
		}

		/// <summary>
		/// The date and time the communique was received by the client.
		/// </summary>
		public DateTime ArrivedTime
		{
			get { return arrivedTime; }
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return body.GetHashCode() ^ hostName.GetHashCode() ^ sentTime.GetHashCode() ^ arrivedTime.GetHashCode();
		}

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns><b>true</b> if <i>obj</i> and this instance are the same type and represent the same value; otherwise, <b>false</b>.</returns>
		public override bool Equals(object obj)
		{
			if (obj is Communique)
			{
				if (obj.GetHashCode() == GetHashCode())
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}
}
