using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacPac.Core.Algorithms;
using PacPac.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Characters.GhostCharacters
{
	public class Inky : Ghost
	{
		private AStar astar;

		public Inky(Game game) : base(game)
		{
			this.Game.Components.Add(this);
		}

		public override Direction? Strategy(GameTime gameTime)
		{
			try
			{
				/*Dijkstra dijkstra = new Dijkstra(GhostManager.Instance.Map);
				return dijkstra.ComputeDirection(
						// Start: Current ghost position
						ConvertPositionToTileIndexes(),
						// Destination: Pac's position
						GhostManager.Instance.Pac.ConvertPositionToTileIndexes());*/
				Path path = astar.ComputePath(
						// Start: Current ghost position
						ConvertPositionToTileIndexes(),
						// Destination: Pac's position
						GhostManager.Instance.Pac.ConvertPositionToTileIndexes()
					);
				return astar.WhichDirection(ConvertPositionToTileIndexes(), path);
			}
			catch (InfiniteLoopException ex)
			{
				Console.Error.WriteLine(ex.StackTrace);
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
			if (GhostManager.Instance.IsInitialized)
				astar = new AStar(GhostManager.Instance.Map);

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
