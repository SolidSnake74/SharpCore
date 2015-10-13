using System;
using System.Net;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpCore.Communications;

namespace SharpCore.Communications.Testing
{
	/// <summary>
	/// Verifies the functionality found in SharpCore.Communications.
	/// </summary>
	[TestClass]
	public class UnitTest
	{
		[TestMethod]
		public void CommunicationsUnitTest()
		{
			using (SessionServer server = new SessionServer(new IPEndPoint(IPAddress.Any, IPEndPoint.MaxPort)))
			{
				server.CommuniqueReceived += new CommuniqueReceivedEventHandler(server_CommuniqueReceived);

				IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
				IPEndPoint clientEndPoint = new IPEndPoint(hostEntry.AddressList[0], IPEndPoint.MaxPort);
				using (SessionClient client = new SessionClient(clientEndPoint))
				{
					Communique communique = new Communique("CommunicationsUnitTest");
					client.SendCommunique(communique);
				}

			}
		}

		/// <summary>
		/// Handler for CommuniqueReceived events.
		/// </summary>
		/// <param name="sender">The server that received the communique.</param>
		/// <param name="e">Container for the communique data.</param>
		/// <remarks>This event should fire before the above SessionServer is disposed.</remarks>
		private void server_CommuniqueReceived(object sender, CommuniqueReceviedEventArgs e)
		{
			Communique communique = e.Communique;
			Assert.IsNotNull(communique);
			Assert.IsTrue(communique.ArrivedTime.ToShortDateString().Length > 0);
			Assert.IsTrue(communique.Body.ToString().Length > 0);
			Assert.IsTrue(communique.HostName.Length > 0);
			Assert.IsTrue(communique.SentTime.ToShortDateString().Length > 0);
		}
	}
}
