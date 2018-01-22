using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Algorithms
{
	public class Path : List<Vector2>
	{
		#region Variables & Properties
		public Vector2 Origin
		{
			get { return this.Count > 0 ? this[0] : new Vector2(-1, -1); }
		}
		public Vector2 Goal
		{
			get { return this.Count > 0 ? this[this.Count-1] : new Vector2(-1, -1); }
		}
		#endregion

		#region Constructor
		public Path() { }
		public Path(List<Vector2> list)
		{
			this.AddRange(list);
		}
		public Path(Vector2 position)
		{
			this.Add(position);
		}
		#endregion
	}
}
