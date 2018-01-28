using Microsoft.Xna.Framework;
using PacPac.Core.Exceptions;
using PacPac.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Algorithms
{
	public class Dijkstra
	{
		private DNode[,] nodes;
		private Maze maze;

		public DNode[,] Nodes
		{
			get { return nodes; }
			set { nodes = value; }
		}
		public Maze Map
		{
			get { return maze; }
			set { maze = value; }
		}
		public DNode this[int i, int j]
		{
			get { return Nodes[i, j]; }
			set { Nodes[i, j] = value; }
		}
		public DNode this[float i, float j]
		{
			get { return this[(int) i, (int) j]; }
			set { this[(int) i, (int) j] = value; }
		}
		public DNode this[Vector2 indexes]
		{
			get { return this[indexes.X, indexes.Y]; }
			set { this[indexes.X, indexes.Y] = value; }
		}

		public Dijkstra(Maze maze)
		{
			if (maze == null)
				throw new ArgumentNullException();

			Map = maze;
			Nodes = new DNode[Map.Width, Map.Height];

			ResetNodes();
		}

		public void ResetNodes()
		{
			for (int i = 0, maxi = Map.Width; i < maxi; i++)
			{
				for (int j = 0, maxj = Map.Height; j < maxj; j++)
				{
					if (!Cell.IsTileTypeBlock(Map[i, j].Tile) || Map[i, j].Tile == TileType.GHOST_GATE)
						this[i, j] = new DNode(DNode.INFINITY, false);
					else
						this[i, j] = null;
				}
			}
		}

		public Direction? ComputeDirection(Vector2 start, Vector2 end)
		{
			if (start == null || end == null)
				throw new ArgumentNullException();

			if (start.X < 0 || start.X >= Map.Width ||
				start.Y < 0 || start.Y >= Map.Height ||
				end.X < 0 || end.X >= Map.Width ||
				end.Y < 0 || end.Y >= Map.Height)
				throw new ArgumentException();

			if (start.Equals(end))
				return null;

			if (this[(int)end.X, (int)end.Y] == null)
				this[(int)end.X, (int)end.Y] = new DNode(0, false);
			else
				this[(int) end.X, (int) end.Y].Potential = 0;
			Vector2 current = end;

			// Beginning of the algorithm
			int rec = 0;
			while (!current.Equals(start))
			{
				DNode z = this[current];
				z.IsChecked = true;

				// Up
				if (current.Y > 0)
					CheckNode(current, z, new Vector2(current.X, current.Y - 1));

				// Down
				if (current.Y + 1 < Map.Height)
					CheckNode(current, z, new Vector2(current.X, current.Y + 1));

				// Left
				if (current.X > 0)
					CheckNode(current, z, new Vector2(current.X - 1, current.Y));

				// Right
				if (current.X + 1 < Map.Width)
					CheckNode(current, z, new Vector2(current.X + 1, current.Y));

				int min = DNode.INFINITY;
				for (int i = 0, maxi = Map.Width; i < maxi; i++)
				{
					for (int j = 0, maxj = Map.Height; j < maxj; j++)
					{
						if (this[i, j] != null)
						{
							if (!this[i, j].IsChecked && this[i, j].Potential < min)
							{
								min = this[i, j].Potential;
								current = new Vector2(i, j);
							}
						}
					}
				}

				rec++;

				if (rec == 1000)
					throw new InfiniteLoopException(rec);
			}

			Vector2 next = this[current].Previous;
			Direction result;

			if (next.X != start.X)
			{
				if (next.X > start.X)
					result = Direction.RIGHT;
				else
					result = Direction.LEFT;
			}
			else
			{
				if (next.Y > start.Y)
					result = Direction.DOWN;
				else
					result = Direction.UP;
			}

			return result;
		}

		private void CheckNode(Vector2 current, DNode z, Vector2 coordinates)
		{
			if (coordinates.X > 0 && coordinates.X < Map.Width &&
				coordinates.Y > 0 && coordinates.Y < Map.Height &&
				this[coordinates] != null)
			{
				DNode s = this[coordinates];
				if (s.Potential > z.Potential + 1)
				{
					s.Potential = z.Potential + 1;
					s.Previous = current;
				}
			}
		}
	}
}
