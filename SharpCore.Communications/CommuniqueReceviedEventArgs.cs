using System;

namespace SharpCore.Communications
{
	/// <summary>
	/// Provides data for the <see cref="SharpCore.Communications.CommuniqueReceivedEventHandler"/> event.
	/// </summary>
	public sealed class CommuniqueReceviedEventArgs : EventArgs
	{
		/// <summary>
		/// The Communique that was received.
		/// </summary>
		private Communique communique;

		/// <summary>
		/// Intitializes a new instance of the CommuniqueReceivedEventArgs class.
		/// </summary>
		/// <param name="communique">The Communique that was recevied.</param>
		internal CommuniqueReceviedEventArgs(Communique communique)
		{
			this.communique = communique;
		}

		/// <summary>
		/// The Communique that was received.
		/// </summary>
		public Communique Communique
		{
			get { return communique; }
		}
	}
}
