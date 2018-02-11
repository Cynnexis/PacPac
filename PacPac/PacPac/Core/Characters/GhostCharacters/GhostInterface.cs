using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core
{
	public interface GhostInterface
	{
		/// <summary>
		/// Make the ghost think about a strategy.
		/// </summary>
		/// <param name="gameTime">The current game time</param>
		/// <returns>Return the direction the ghost want to go to. If <c>null</c>, it means that the ghost do not
		/// wish to move.</returns>
		Direction? Strategy(GameTime gameTime);
	}
}
