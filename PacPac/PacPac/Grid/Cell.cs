using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacPac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Grid
{
	/// <summary>
	/// A cell of the maze. It contains a tiletype (wall, pacdots, ...) and
	/// a dimension.
	/// </summary>
	public class Cell
	{
		private TileType tile = TileType.EMPTY;
		private BoundingBox dimension = new BoundingBox();

		/// <summary>
		/// The type of tile
		/// </summary>
		public TileType Tile
		{
			get { return tile; }
			set { tile = value;	}
		}

		/// <summary>
		/// The dimension of the cell.
		/// </summary>
		public BoundingBox Dimension
		{
			get { return dimension; }
			set { dimension = value; }
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="dimension">The dimension of the cell</param>
		/// <param name="type">The type of cell</param>
		public Cell(BoundingBox dimension, TileType type)
		{
			Dimension = dimension;
			Tile = type;
		}

		/// <summary>
		/// Indicates if <paramref name="tile"/> is passable.
		/// For this method, <see cref="TileType.GHOST_WALL"/> is not passable.
		/// </summary>
		/// <param name="tile">The type of tile to evaluate</param>
		/// <returns>Return <c>true</c> if <paramref name="tile"/> is not
		/// passable, otherwise <c>false</c></returns>
		public static bool IsTileTypeBlock(TileType tile)
		{
			return (tile == TileType.BORDER ||
				tile == TileType.GHOST_GATE ||
				tile == TileType.GHOST_WALL ||
				tile == TileType.WALL);
		}
	}
}
