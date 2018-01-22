using Microsoft.Xna.Framework;
using PacPac.Core.Exceptions;
using PacPac.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Algorithms
{
	public class AStar
	{
		private List<ANode> open;
		private List<ANode> closed;
		private Maze maze;
		private Path path;

		public List<ANode> Open
		{
			get { return open; }
			private set { open = value; }
		}
		public List<ANode> Closed
		{
			get { return closed; }
			private set { closed = value; }
		}
		public Maze Map
		{
			get { return maze; }
			set { maze = value; }
		}
		public Path Path
		{
			get { return path; }
			private set { path = value; }
		}
		
		public AStar(Maze maze)
		{
			if (maze == null)
				throw new ArgumentNullException();

			Map = maze;
			Open = new List<ANode>();
			Closed = new List<ANode>();
			Path = new Path();
		}

		public Path ComputePath(Vector2 start, Vector2 end)
		{
			if (Path == null)
				throw new InvalidOperationException();

			if (/*start.Equals(Path.Origin) && */end.Equals(Path.Goal))
				return Path;

			if (start == null || end == null)
				throw new ArgumentNullException();

			if (start.X < 0 || start.X >= Map.Width || start.Y < 0 || start.Y >= Map.Height ||
				end.X < 0 || end.X >= Map.Width || end.Y < 0 || end.Y >= Map.Height)
				throw new ArgumentException();

			Open = new List<ANode>(1);
			Closed = new List<ANode>();
			Path = new Path();

			// Add the starting point to the path
			Path.Add(start);

			// Add the starting point to the Open nodes
			Open.Add(new ANode(0, ManhattanDistance(start, end), start));

			Dictionary<Vector2, Vector2> map = new Dictionary<Vector2, Vector2>();

			// While Open is not empty
			int rec = 0;
			while (Open.Count > 0)
			{
				int q_index = minF(Open);
				// q is the node which has the minimum F in the Open nodes. This is THIS node which will be evaluated in this iteration.
				ANode q = Open[q_index];

				if (q.Position.Equals(end))
				{
					/*if (!Path.Contains(q.Position))
						Path.Add(q.Position);*/
					return ReconstructPath(map, q.Position);
				}

				// Remove q from the Open nodes
				Open.Remove(q);

				Closed.Add(q);

				// Generate q's 4 successors (neighbors)
				List<ANode> successors = new List<ANode>(4);
				//successors.Add(new ANode(ANode.INFINITY, 0, new Vector2(q.Position.X - 1, q.Position.Y - 1), q));
				successors.Add(new ANode(ANode.INFINITY, 0, new Vector2(q.Position.X, q.Position.Y - 1), q));
				//successors.Add(new ANode(ANode.INFINITY, 0, new Vector2(q.Position.X + 1, q.Position.Y - 1), q));
				successors.Add(new ANode(ANode.INFINITY, 0, new Vector2(q.Position.X + 1, q.Position.Y), q));
				//successors.Add(new ANode(ANode.INFINITY, 0, new Vector2(q.Position.X + 1, q.Position.Y + 1), q));
				successors.Add(new ANode(ANode.INFINITY, 0, new Vector2(q.Position.X, q.Position.Y + 1), q));
				//successors.Add(new ANode(ANode.INFINITY, 0, new Vector2(q.Position.X - 1, q.Position.Y + 1), q));
				successors.Add(new ANode(ANode.INFINITY, 0, new Vector2(q.Position.X - 1, q.Position.Y), q));

				// TODO: Merge the two for-loop in one, and replace the Remove and i-- by 'continue'
				// Check if all successors are correct
				/*
				for (int i = 0; i < successors.Count; i++)
				{
					Vector2 p = successors[i].Position;

					// If the node is in Closed, then the node is already evaluated
					/*if (Closed.Contains(successors[i]))
					{
						successors.RemoveAt(i);
						i--;
					}

					if (i < 0)
						i = 0;*

					// Check if position is correct
					if ((p.X < 0 || p.X >= Map.Width || p.Y < 0 || p.Y >= Map.Height ||
						 p.X < 0 || p.X >= Map.Width || p.Y < 0 || p.Y >= Map.Height) && i <= successors.Count - 1)
					{
						successors.RemoveAt(i);
						i--;
					}

					if (i < 0)
						i = 0;

					// Check if the node in the maze is not a block
					if (Cell.IsTileTypeBlock(Map[p].Tile) && i <= successors.Count - 1)
					{
						successors.RemoveAt(i);
						i--;
					}

					if (i < 0)
						i = 0;

					// Finally, after all those checks, add the successor in Open:
					/*if (i <= successors.Count - 1 && !Open.Contains(successors[i]))
						Open.Add(successors[i]);*
				}*/
				

				// Now that successors contains valid nodes, continue the A* algorithm
				// For each succesor 's', update its variables G and H
				ANode s = null;
				for (int i = 0; i < successors.Count; i++)
				{
					s = successors[i];
					Vector2 p = s.Position;
					// Check if position is correct
					if (p.X < 0 || p.X >= Map.Width || p.Y < 0 || p.Y >= Map.Height)
						continue;

					if (Cell.IsTileTypeBlock(Map[p].Tile))
						continue;

					/*if (s.Equals(end))
						break;*/

					/* mit.edu solution : */
					/*
					//          distance between s and q
					s.G = q.G + ManhattanDistance(s.Position, q.Position);

					//    distance between goal and s
					s.H = ManhattanDistance(end, s.Position);

					// s.F is automatically calculated (see ANode.G and ANode.H properties for more details)
					List<ANode> openUclosed = new List<ANode>(Open.Count + Closed.Count);
					openUclosed.AddRange(Open);
					openUclosed.AddRange(Closed);
					foreach (ANode n in openUclosed)
						if (s.Position.Equals(n.Position) && n.F < s.F)
							continue;
					*/

					/* Wikipedia solution: */
					if (Closed.Contains(s))
						continue;

					if (!Open.Contains(s))
						Open.Add(s);

					//                      distance between s and q
					int tentative_g = q.G + ManhattanDistance(s.Position, q.Position);
					if (tentative_g >= s.G)
						continue;

					if (!Path.Contains(q.Position))
						Path.Add(s.Position);
					map[s.Position] = q.Position;
					s.G = tentative_g;
					//    distance between goal and s
					s.H = ManhattanDistance(end, s.Position);
				}

				// q is now evaluated, it can be stored in Closed
				//Closed.Add(q);

				rec++;
				if (rec >= 100)
					throw new InfiniteLoopException();
			}

			return Path;
		}

		private int minF(List<ANode> nodes)
		{
			int min_i = 0;
			for (int i = 0; i < nodes.Count - 1; i++)
			{
				min_i = i;
				for (int j = i + 1; j < nodes.Count; j++)
					if (nodes[j].F < nodes[min_i].F)
						min_i = j;
			}

			// min_i is the index to the minimal node
			return min_i;
		}

		private int ManhattanDistance(Vector2 start, Vector2 end)
		{
			return (int) Math.Round(Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y));
		}

		private Path ReconstructPath(Dictionary<Vector2, Vector2> map, Vector2 current)
		{
			Path path = new Path(current);
			while (map.Keys.Contains(current))
			{
				current = map[current];
				path.Add(current);
			}
			path.Reverse();
			return path;
		}

		public Direction? WhichDirection(Vector2 current, Path path)
		{
			int idx = path.IndexOf(current);

			if (idx == path.Count - 1)
				return null;

			if (!(0 <= idx && idx <= path.Count - 1))
				idx = 0;

			Vector2 next = path[idx + 1];

			Direction result;

			if (current.X != next.X)
			{
				if (current.X < next.X)
					result = Direction.RIGHT;
				else
					result = Direction.LEFT;
			}
			else
			{
				if (current.Y < next.Y)
					result = Direction.DOWN;
				else
					result = Direction.UP;
			}

			return result;
		}
		public Direction? WhichDirection(Vector2 current)
		{
			return WhichDirection(current, Path);
		}
		public Direction? ReturnDirection(int index, Path path)
		{
			return WhichDirection(path[index], path);
		}
		public Direction? ReturnDirection(int index)
		{
			return WhichDirection(path[index], Path);
		}
	}
}
