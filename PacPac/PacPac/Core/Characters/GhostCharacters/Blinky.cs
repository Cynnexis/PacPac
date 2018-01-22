using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacPac.Grid;
using PacPac.Grid.Exceptions;
using PacPac.Core.Characters.GhostCharacters;
using System.Diagnostics;
using PacPac.Core.Exceptions;
using PacPac.Core.Algorithms;

namespace PacPac.Core.Characters.GhostCharacters
{
	public class Blinky : Ghost
	{
		public static int COUNTDOWN = 4; // seconds
		private Vector2 pacPos;

		public Blinky(Game game) : base(game)
		{
			//Still = false;
			State = GhostState.RUNNING;
			this.Game.Components.Add(this);
		}

		/// <summary>
		/// Blinky Strategy: Blinky is following pac. To do so, the program take pac's position every 2-3 seconds and go to this tile.
		/// If this tile is reached by Blinky before the countdown, blinky will force the algorithm to take the new position of pac,
		/// therefore the ghost will follow pac nearer and nearer
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public override Direction? Strategy(GameTime gameTime)
		{
			/*try
			{
				if (ConvertPositionToTileIndexes().Equals(ConvertPositionToTileIndexes(pacPos)) || ((int) Math.Round(gameTime.TotalGameTime.TotalSeconds)) % COUNTDOWN == 0)
					pacPos = GhostManager.Instance.Pac.Position;

				Dijkstra dijkstra = new Dijkstra(GhostManager.Instance.Map);
				return dijkstra.ComputeDirection(
						// Start: Current ghost position
						ConvertPositionToTileIndexes(),
						// Destination: Last Pac's position
						ConvertPositionToTileIndexes(pacPos)
					);
			}
			catch (InfiniteLoopException ex)
			{
				Console.Error.WriteLine(ex.StackTrace);
			}*/

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
			if (GhostManager.Instance.IsInitialized)
				pacPos = GhostManager.Instance.Pac.Position;
			else
				pacPos = Vector2.Zero;

			base.Initialize();
		}

		protected override void LoadContent()
		{
			tx_ghost = Game.Content.Load<Texture2D>(@"Images\ghost_blinky");
			base.LoadContent();
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			// TODO: Add your update code here

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}
		#endregion
	}
}
