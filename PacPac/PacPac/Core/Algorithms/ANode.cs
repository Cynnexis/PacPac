using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Algorithms
{
	/// <summary>
	/// Node for the A* algorithm
	/// </summary>
	/// <seealso cref="AStar"/>
	[Obsolete]
	public class ANode
	{
		public static int INFINITY = int.MaxValue;

		private int g;
		private int h;
		private int f;
		private Vector2 position;
		private ANode parent;

		public int G
		{
			get { return g; }
			set
			{
				int oldValue = g;
				g = value;

				if (oldValue != g)
					F = moveCost();
			}
		}
		public int H
		{
			get { return h; }
			set
			{
				int oldValue = h;
				h = value;

				if (oldValue != h)
					F = moveCost();
			}
		}
		public int F
		{
			get { return f; }
			set { f = value; }
		}
		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}
		public ANode Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		public ANode(int g = 0, int h = 0, Vector2? position = null, ANode parent = null)
		{
			G = g;
			H = h;
			Position = position != null ? (Vector2) position : new Vector2(-1, -1);
			Parent = parent;
		}

		public int moveCost()
		{
			if (G == INFINITY || H == INFINITY || ((long) (((long)G) + ((long)H))) >= INFINITY)
				return INFINITY;
			return G + H;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (!(obj is ANode))
				return false;

			ANode n = (ANode) obj;
			return Position.Equals(n.Position);
		}

		public override string ToString()
		{
			return "ANode{Position={X=" + Position.X + ", Y=" + Position.Y + "}}";
		}
	}
}
