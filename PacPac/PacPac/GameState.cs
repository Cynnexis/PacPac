using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac
{
	/// <summary>
	/// Enumerates all game state
	/// </summary>
	public enum GameState
	{
		/// <summary>
		/// The game is in the start menu
		/// </summary>
		StartMenu,

		/// <summary>
		/// The game is playing
		/// </summary>
		Playing,

		/// <summary>
		/// The game is in pause
		/// </summary>
		Pause,

		/// <summary>
		/// The game is stopped
		/// </summary>
		Stop
	}
}
