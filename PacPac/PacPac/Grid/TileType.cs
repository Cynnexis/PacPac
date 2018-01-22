using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Grid
{
	public enum TileType
	{
		EMPTY,
		WALL,
		GHOST_WALL,
		GHOST_GATE,
		BORDER,
		PACDOT,
		FRUIT,
		PAC_STARTUP,
		BLINKY_STARTUP,
		GHOSTS_STARTUP,
		TELEPORTER
	}
}
