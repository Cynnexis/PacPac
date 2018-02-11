using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacPac.Core;

namespace PacPac.Grid
{
	/// <summary>
	/// Type of tile
	/// </summary>
	/// <seealso cref="Cell"/>
	public enum TileType
	{
		/// <summary>
		/// Empty tile
		/// </summary>
		EMPTY,

		/// <summary>
		/// Wall tile
		/// </summary>
		WALL,

		/// <summary>
		/// Border of the ghosts starting area, represented by a wall
		/// </summary>
		GHOST_WALL,

		/// <summary>
		/// The gate of the ghost starting area
		/// </summary>
		GHOST_GATE,

		/// <summary>
		/// Border of the map, represented by a wall
		/// </summary>
		BORDER,

		/// <summary>
		/// Pac-dot tile. It will be replaced by an <see cref="EMPTY"/> tile
		/// when pac will pass in this tile.
		/// </summary>
		PACDOT,

		/// <summary>
		/// Fruit tile. It will be replaced by an <see cref="EMPTY"/> tile
		/// when pac will pass in this tile.
		/// </summary>
		FRUIT,

		/// <summary>
		/// Starting point for pac
		/// </summary>
		PAC_STARTUP,

		/// <summary>
		/// Ghost leader starting point
		/// </summary>
		BLINKY_STARTUP,

		/// <summary>
		/// Ghosts starting point (describe the ghost starting area)
		/// </summary>
		GHOSTS_STARTUP,

		/// <summary>
		/// Teleporter-tile, associated with another tile
		/// </summary>
		/// <seealso cref="Teleporter"/>
		/// <seealso cref="TeleporterManager>
		TELEPORTER
	}
}
