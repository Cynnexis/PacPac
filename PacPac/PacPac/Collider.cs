using Microsoft.Xna.Framework;
using PacPac.Core;
using PacPac.Grid;
using PacPac.Grid.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac
{
	[Obsolete]
	public class Collider
	{
		public static bool CheckCollision(BoundingBox b1, BoundingBox b2)
		{
			CheckBoudingBox(b1);
			CheckBoudingBox(b2);
			
			return
				/*
				// Touch-type collision
				(b1.Min.Y == b2.Max.Y) ||
				(b1.Min.X == b2.Max.X) ||
				(b1.Max.Y == b2.Min.Y) ||
				(b1.Max.X == b2.Min.X) ||
				// Superposition-type collision
				((b2.Min.X <= b1.Max.X && b1.Max.X <= b2.Max.X) && (b2.Min.Y <= b1.Min.Y && b1.Min.Y <= b2.Max.Y)) ||
				((b2.Min.X <= b1.Min.X && b1.Min.X <= b2.Max.X) && (b1.Min.Y <= b2.Min.Y && b2.Min.Y <= b1.Max.Y)) ||*/
				b1.Intersects(b2);
		}
		public static bool CheckCollision(Cell cell, Pac pac)
		{
			// No collision with an empty or food-type cell
			if (!Cell.IsTileTypeBlock(cell.Tile))
				return false;

			return CheckCollision(cell.Dimension, pac.Dimension);
		}
		public static bool CheckCollision(Maze maze, Pac pac)
		{
			bool collision = false;
			for (int i = 0, maxi = maze.Cells.GetLength(0); i < maxi && !collision; i++)
			{
				for (int j = 0, maxj = maze.Cells.GetLength(0); j < maxj && !collision; j++)
				{
					/*BoundingBox currentCellBox = new BoundingBox(new Vector3(i * Maze.SPRITE_DIMENSION, j * Maze.SPRITE_DIMENSION, 0),
						new Vector3((i + 1) * Maze.SPRITE_DIMENSION, (j + 1) * Maze.SPRITE_DIMENSION, 0));*/
					collision = CheckCollision(maze[i, j], pac);
				}
			}

			return collision;
		}

		public static bool LessThan(Vector2 v1, Vector2 v2)
		{
			return v1.X < v2.X && v1.Y < v2.Y;
		}
		public static bool LessOrEqualTo(Vector2 v1, Vector2 v2)
		{
			return v1.X <= v2.X && v1.Y <= v2.Y;
		}

		public static bool GreaterThan(Vector2 v1, Vector2 v2)
		{
			return v1.X > v2.X && v1.Y > v2.Y;
		}

		public static bool GreaterOrEqualTo(Vector2 v1, Vector2 v2)
		{
			return v1.X >= v2.X && v1.Y >= v2.Y;
		}

		public static Vector2 V3ToV2(Vector3 v3)
		{
			return new Vector2(v3.X, v3.Y);
		}

		public static void CheckBoudingBox(BoundingBox box)
		{
			if (GreaterThan(V3ToV2(box.Min), V3ToV2(box.Max)))
				throw new BoundingBoxException();
		}
	}
}
