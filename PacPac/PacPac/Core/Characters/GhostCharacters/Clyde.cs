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
	public class Clyde : Ghost
	{
		/// <summary>
		/// Countdown until Clyde can update its strategy
		/// </summary>
		public static int COUNTDOWN = 7; // seconds

		/// <summary>
		/// Last time Clyde updated its strategy
		/// </summary>
		private int lastStrategyUpdate; // seconds
		private Vector2 goal; // In tile indexes
		private bool hasFallenInInfiniteLoop;

		public Clyde(Game game) : base(game)
		{
			hasFallenInInfiniteLoop = false;
			lastStrategyUpdate = -1;
			this.Game.Components.Add(this);
		}

		/// <summary>
		/// Clyde Strategy: Clyde moves randmoly on the map.
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
				((int) Math.Round(gameTime.TotalGameTime.TotalSeconds)) % COUNTDOWN == 0))
			{
				// Get all the pacdot tiles
				List<Cell> list = GhostManager.Instance.Map.SearchTile(TileType.PACDOT);

				// If the list is null, instanciate it
				if (list == null)
					list = new List<Cell>();

				// If there is not enough tiles, add the empty ones to the list
				if (list.Count <= 20)
					list.AddRange(GhostManager.Instance.Map.SearchTile(TileType.EMPTY));

				// Amongst all the tiles, get one randomly
				Random r = new Random((int) Math.Round(gameTime.TotalGameTime.TotalMilliseconds));

				Vector3 result = list[r.Next(0, list.Count)].Dimension.Min;
				goal = ConvertPositionToTileIndexes(new Vector2(result.X, result.Y));

				hasFallenInInfiniteLoop = false;
				lastStrategyUpdate = ((int)Math.Round(gameTime.TotalGameTime.TotalSeconds));
			}
			
			// Execute the Dijkstra's Algorithm to go to the new goal
			try
			{
				Dijkstra dijkstra = new Dijkstra(GhostManager.Instance.Map);
				return dijkstra.ComputeDirection(
						ConvertPositionToTileIndexes(),
						goal
					);
			}
			catch (InfiniteLoopException)
			{
				// If the algorithm threw an InfiniteLoopException, indicates that information for the next call of Strategy.
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
			// TODO: Add your initialization code here
			goal = Position;

			base.Initialize();
		}

		protected override void LoadContent()
		{
			tx_ghost = Game.Content.Load<Texture2D>(@"Images\ghost_clyde");
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
