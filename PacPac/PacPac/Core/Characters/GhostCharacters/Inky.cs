using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacPac.Core.Algorithms;
using PacPac.Core.Exceptions;
using PacPac.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Characters.GhostCharacters
{
	public class Inky : Ghost
	{
		/// <summary>
		/// Countdown until Inky can update its strategy
		/// </summary>
		public static int COUNTDOWN = 5; // seconds

		/// <summary>
		/// Countdown until Inky change its Strategy mode (random or pacpac chase)
		/// </summary>
		public static int COUNTDOWN_MODE = 7; // seconds

		/// <summary>
		/// Last time Inky updated its strategy
		/// </summary>
		private int lastStrategyUpdate; // seconds
		private bool hasFallenInInfiniteLoop;
		private bool randomMode;
		private Vector2 goal; // In tile indexes

		public Inky(Game game) : base(game)
		{
			lastStrategyUpdate = -1;
			hasFallenInInfiniteLoop = false;
			randomMode = true;
			goal = new Vector2(-1, -1);

			this.Game.Components.Add(this);
		}

		/// <summary>
		/// Inky's Strategy has two modes: The first one is "Random". As Clyde's strategy, Inky will hang around the maze.
		/// The second strategy is chasing after pacpac. Those strategies are changing every <c>COUNTDOWN_MODE</c> seconds.
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public override Direction? Strategy(GameTime gameTime)
		{
			// If Clyde is in its goal OR dikstra's algorithm fell into an infinite loop OR the countdown is over, then update the strategy
			if (ConvertPositionToTileIndexes().Equals(goal) ||
				hasFallenInInfiniteLoop ||
				lastStrategyUpdate == -1 ||
				(gameTime.TotalGameTime.TotalSeconds != 0 &&
				((int)Math.Round(gameTime.TotalGameTime.TotalSeconds)) != lastStrategyUpdate &&
				((int)Math.Round(gameTime.TotalGameTime.TotalSeconds)) % COUNTDOWN == 0))
			{
				// Change the mode
				if (((int)Math.Round(gameTime.TotalGameTime.TotalSeconds)) % COUNTDOWN_MODE == 0)
					randomMode = !randomMode;

				if (randomMode)
				{
					/*
					// Get all the pacdot tiles
					List<Cell> list = GhostManager.Instance.Map.SearchTile(TileType.PACDOT);

					// If the list is null, instanciate it
					if (list == null)
						list = new List<Cell>();

					// If there is not enough tiles, add the empty ones to the list
					if (list.Count <= 20)
						list.AddRange(GhostManager.Instance.Map.SearchTile(TileType.EMPTY));

					// Amongst all the tiles, get one randomly
					Random r = new Random((int)Math.Round(gameTime.TotalGameTime.TotalMilliseconds));

					Vector3 result = list[r.Next(0, list.Count)].Dimension.Min;
					goal = ConvertPositionToTileIndexes(new Vector2(result.X, result.Y));
					*/

					Vector2? res = GenerateRandomPlace();

					if (res == null)
					{
						hasFallenInInfiniteLoop = true;
						return null;
					}

					goal = (Vector2)res;
				}

				hasFallenInInfiniteLoop = false;
				lastStrategyUpdate = ((int)Math.Round(gameTime.TotalGameTime.TotalSeconds));
			}

			try
			{
				Dijkstra dijkstra = new Dijkstra(GhostManager.Instance.Map);
				return dijkstra.ComputeDirection(
						// Start: Current ghost position
						ConvertPositionToTileIndexes(),
						randomMode ? goal : GhostManager.Instance.Pac.ConvertPositionToTileIndexes());
				/*
				Path path = astar.ComputePath(
						// Start: Current ghost position
						ConvertPositionToTileIndexes(),
						// Destination: Pac's position
						GhostManager.Instance.Pac.ConvertPositionToTileIndexes()
					);
				return astar.WhichDirection(ConvertPositionToTileIndexes(), path);*/
			}
			catch (InfiniteLoopException ex)
			{
				hasFallenInInfiniteLoop = true;
			}

			return null;
		}

		#region GameComponent Overrides
		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			/*if (GhostManager.Instance.IsInitialized)
				astar = new AStar(GhostManager.Instance.Map);*/

			base.Initialize();
		}

		protected override void LoadContent()
		{
			tx_ghost = Game.Content.Load<Texture2D>(@"Images\ghost_inky");
			base.LoadContent();
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}
		#endregion
	}
}
