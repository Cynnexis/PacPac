using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Algorithms
{
	public class DNode
	{
		public static int INFINITY = int.MaxValue;

		private int potential;
		private bool isChecked;
		private Vector2 previous;

		public int Potential
		{
			get { return potential; }
			set { potential = value; }
		}
		public bool IsChecked
		{
			get { return isChecked; }
			set { isChecked = value; }
		}
		public Vector2 Previous
		{
			get { return previous; }
			set { previous = value; }
		}

		public DNode() { }
		public DNode(int potential)
		{
			Potential = potential;
		}
		public DNode(bool isChecked)
		{
			IsChecked = isChecked;
		}
		public DNode(Vector2 previous)
		{
			Previous = previous;
		}
		public DNode(int potential, bool isChecked)
		{
			Potential = potential;
			IsChecked = isChecked;
		}
		public DNode(int potential, Vector2 previous)
		{
			Potential = potential;
			IsChecked = isChecked;
			Previous = previous;
		}
		public DNode(bool isChecked, Vector2 previous)
		{
			Potential = potential;
			IsChecked = isChecked;
			Previous = previous;
		}
		public DNode(int potential, bool isChecked, Vector2 previous)
		{
			Potential = potential;
			IsChecked = isChecked;
			Previous = previous;
		}
	}
}
