using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacPac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Grid
{
	public class Cell
	{
		private TileType tile = TileType.EMPTY;
		private BoundingBox dimension = new BoundingBox();

		public TileType Tile
		{
			get { return tile; }
			set { tile = value;	}
		}


		public BoundingBox Dimension
		{
			get { return dimension; }
			set { dimension = value; }
		}

		public Cell(Game game, BoundingBox dimension, TileType type)
		{
			Dimension = dimension;
			Tile = type;
		}

		public static bool IsTileTypeBlock(TileType tile)
		{
			return (tile == TileType.BORDER ||
				tile == TileType.GHOST_GATE ||
				tile == TileType.GHOST_WALL ||
				tile == TileType.WALL);
		}
	}
}
