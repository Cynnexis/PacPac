using Microsoft.Xna.Framework;
using PacPac.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Characters.GhostCharacters
{
	/// <summary>
	/// Manage the ghosts of the maze by giving them directives such as "Get out of the ghost startup point", "You are now edible", and so on...
	/// </summary>
	public class GhostManager
	{
		private static GhostManager instance = new GhostManager();

		/// <summary>
		/// Each <c>COUNTDOWN_RELEASE_GHOST</c>, <c>GhostManager</c> releases a ghost from the startup point to the maze.
		/// </summary>
		public static int COUNTDOWN_RELEASE_GHOST = 10; // seconds

		private int gameBeginning;
		private List<Ghost> ghosts;
		private Maze maze;
		private Pac pac;
		private Vector2 entrance;
		private bool isInitialized;
		
		public static GhostManager Instance
		{
			get { return instance; }
		}

		/// <summary>
		/// In seconds
		/// </summary>
		public int PlayBeginning
		{
			get { return gameBeginning; }
			set { gameBeginning = value; }
		}
		public List<Ghost> Ghosts
		{
			get { return ghosts; }
			set { ghosts = value; }
		}
		public Maze Map
		{
			get { return maze; }
			set { maze = value; }
		}
		public Pac Pac
		{
			get { return pac; }
			set { pac = value; }
		}
		public Vector2 Entrance
		{
			get { return entrance; }
			set { entrance = value; }
		}
		public bool IsInitialized
		{
			get { return isInitialized; }
			set { isInitialized = value; }
		}

		private GhostManager()
		{
			PlayBeginning = -1;
			Ghosts = new List<Ghost>(4);
			IsInitialized = false;
		}

		public void Initialize(Maze maze, Pac pac, Ghost leader, Ghost[] others)
		{
			PlayBeginning = -1;
			if (Ghosts == null)
				Ghosts = new List<Ghost>(4);
			else
				Ghosts.Clear();

			Map = maze;
			Pac = pac;
			Ghosts.Add(leader);
			Ghosts.AddRange(others);

			// Search startup points for every ghosts
			List<Cell> list = Map.SearchTile(TileType.BLINKY_STARTUP);
			if (list != null && list.Count > 0)
			{
				Vector3 result = list[0].Dimension.Min;
				Ghosts[0].StartingPoint = Entrance = new Vector2(result.X, result.Y);

				// Now, the program must find the start area for the other ghosts
				list = Map.SearchTile(TileType.GHOSTS_STARTUP);
				if (list != null && list.Count >= (Ghosts.Count - 1))
				{
					for (int i = 1, maxi = Ghosts.Count; i < maxi; i++)
					{
						// 1 <= i <= Ghosts.Count, but Ghosts.Count-1 <= list.Count. Therefore, list[i-1] will not throw an ArrayOutOfBoundException
						result = list[i-1].Dimension.Min;
						Ghosts[i].StartingPoint = new Vector2(result.X, result.Y);
					}
				}
				else
					throw new InvalidOperationException("No start point for the ghosts in the maze");
			}
			else
				throw new InvalidOperationException("No start point for Blinky in the maze");
			
			IsInitialized = true;
		}

		public void Update(GameTime gameTime)
		{
			if (PlayBeginning != -1)
			{
				// Every 5 secondes, the game releases a ghost
				if ((gameTime.TotalGameTime.TotalSeconds - PlayBeginning) != 0 &&
					((int)Math.Round(gameTime.TotalGameTime.TotalSeconds - PlayBeginning)) % COUNTDOWN_RELEASE_GHOST == 0)
				{
					int index = (int) (gameTime.TotalGameTime.TotalSeconds - PlayBeginning) / COUNTDOWN_RELEASE_GHOST;

					if (index > 0 && index < Ghosts.Count)
					{
						Ghost g = Ghosts[index];
						if (g.State == GhostState.INITIALIZING)
						{
							g.State = GhostState.MOVING_MAZE;
#if DEBUG
							Console.WriteLine("Ghost n°" + (index + 1) + " is released");
#endif
						}
					}
				}
			}
		}
	}
}
