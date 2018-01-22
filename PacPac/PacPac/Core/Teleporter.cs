using Microsoft.Xna.Framework;
using PacPac.Grid;
using PacPac.Grid.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core
{
	public class Teleporter
	{
		private char name;
		private Vector2 position1;
		private Vector2 position2;
		private bool activated = true;

		public char Name
		{
			get { return name; }
			set { name = value; }
		}

		public Vector2 Position1
		{
			get { return position1; }
			set { position1 = value; }
		}

		public Vector2 Position2
		{
			get { return position2; }
			set { position2 = value; }
		}

		public bool Activated
		{
			get { return activated; }
			set { activated = value; }
		}

		public Teleporter(char name, Vector2 pos1, Vector2 pos2)
		{
			Name = name;
			Position1 = pos1;
			Position2 = pos2;
		}
		public Teleporter(char name, Vector2 pos1)
		{
			Name = name;
			Position1 = pos1;
			Position2 = new Vector2(-1, -1);
		}

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
