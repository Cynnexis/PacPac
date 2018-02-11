using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Characters.GhostCharacters
{
	/// <summary>
	/// Enumerate the differents possible state of a ghost
	/// </summary>
	public enum GhostState
	{
		/// <summary>
		/// Indicate that the ghost is still in the ghost start area
		/// </summary>
		INITIALIZING,

		/// <summary>
		/// The ghost is moving from the starting area to the maze entrance
		/// </summary>
		MOVING_MAZE,

		/// <summary>
		/// The ghost is running in the maze, using its own strategy
		/// </summary>
		RUNNING,

		/// <summary>
		/// The ghost is now edible, and shall run away from pac
		/// </summary>
		EDIBLE,

		/// <summary>
		/// The ghost has been eaten by pac, and must return to the ghost area to regenerate
		/// </summary>
		EATEN
	}
}
