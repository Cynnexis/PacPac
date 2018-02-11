using Microsoft.Xna.Framework;
using PacPac.Grid;
using PacPac.Grid.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core
{
	/// <summary>
	/// Class to describe a teleporter
	/// </summary>
	public class Teleporter
	{
		private char name;
		private Vector2 position1;
		private Vector2 position2;
		private bool activated;

		/// <summary>
		/// Name of the teleporter. It is commonly a lower-case letter such as
		/// 'a', 'b', ..., 'z'.
		/// </summary>
		public char Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// The first position (in pixel-base) of the teleporter. If pac is at
		/// this position, then it is teleported to <see cref="Position2"/>
		/// </summary>
		/// <seealso cref="Position2"/>
		public Vector2 Position1
		{
			get { return position1; }
			set { position1 = value; }
		}

		/// <summary>
		/// The second position (in pixel-base) of the teleporter. If pac is at
		/// this position, then it is teleported to <see cref="Position1"/>
		/// </summary>
		/// <seealso cref="Position1"/>
		public Vector2 Position2
		{
			get { return position2; }
			set { position2 = value; }
		}

		/// <summary>
		/// Tell if the teleporter is activated or not
		/// </summary>
		public bool Activated
		{
			get { return activated; }
			set { activated = value; }
		}

		/// <summary>
		/// Constructor for Teleporter
		/// </summary>
		/// <param name="name">The name of the teleporter</param>
		/// <param name="pos1">The first position of the teleporter</param>
		/// <param name="pos2">The second position of the teleporter</param>
		/// <param name="activated">Is the teleported activated? Default value is <c>true</c></param>
		/// <seealso cref="Name"/>
		/// <seealso cref="Position1"/>
		/// <seealso cref="Position2"/>
		/// <seealso cref="Activated"/>
		public Teleporter(char name, Vector2 pos1, Vector2 pos2, bool activated = true)
		{
			Name = name;
			Position1 = pos1;
			Position2 = pos2;
			Activated = activated;
		}
		/// <summary>
		/// Constructor for Teleporter
		/// </summary>
		/// <param name="name">The name of the teleporter</param>
		/// <param name="pos1">The first position of the teleporter</param>
		/// <param name="activated">Is the teleported activated? Default value is <c>true</c></param>
		/// <seealso cref="Name"/>
		/// <seealso cref="Position1"/>
		/// <seealso cref="Activated"/>
		public Teleporter(char name, Vector2 pos1, bool activated = true)
		{
			Name = name;
			Position1 = pos1;
			Position2 = new Vector2(-1, -1);
			Activated = activated;
		}

		/// <summary>
		/// Teleport <paramref name="pac"/> from <see cref="Position1"/> to
		/// <see cref="Position2"/> is pac's position if <see cref="Position1"/>
		/// or vice-versa.
		/// </summary>
		/// <param name="pac">The pac to teleport</param>
		/// <returns>Return <c>true</c> if pac did teleport, <c>false</c> otherwise</returns>
		/// <exception cref="TeleporterException">Thrown when the teleporte has not <see cref="Position2"/> set.</exception>
		public bool Teleport(Pac pac)
		{
			bool result = false;

			if (Activated)
			{
				if (Position2.Equals(new Vector2(-1, -1)))
					throw new TeleporterException("Teleporter destination has not been configured yet. Where does pac will go without a destination? Nowhere!");

				if (pac.ConvertPositionToTileIndexes().Equals(Position1))
				{
					pac.Position = new Vector2(Position2.X * Maze.SPRITE_DIMENSION, Position2.Y * Maze.SPRITE_DIMENSION);
					result = true;
				}
				else if (pac.ConvertPositionToTileIndexes().Equals(Position2))
				{
					pac.Position = new Vector2(Position1.X * Maze.SPRITE_DIMENSION, Position1.Y * Maze.SPRITE_DIMENSION);
					result = true;
				}
			}

			// If pacman has been teleported, disable the teleporter until pac is away from the teleporter point
			// (this method prevent pac from teleporting in an infinite loop from one spot to another)
			if (result)
			{
				Activated = false;
				Vector2 currentPosition = pac.Position;
				pac.OnPositionChangedAction = (Vector2 position) =>
				{
					if (!pac.ConvertPositionToTileIndexes().Equals(pac.ConvertPositionToTileIndexes(currentPosition)))
					{
						Activated = true;

						// Delete the delegate
						pac.OnPositionChangedAction = (Vector2 pos) => { };
					}
				};
			}

			return result;
		}
	}
}
