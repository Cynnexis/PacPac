using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacPac.Core.Algorithms;
using PacPac.Core.Exceptions;
using PacPac.Grid;
using PacPac.Grid.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PacPac.Core.Characters.GhostCharacters
{
	public class Pinky : Ghost
	{
		public Pinky(Game game) : base(game)
		{
			this.Game.Components.Add(this);
		}

		/// <summary>
		/// Pinky Strategy: Land on a square of 4x4 tiles located 2 tiles in front of pac
		/// to ambush him.
		/// <para>
		/// Note: In the first Pac-Man game, Pinky had actually an overflow bug.
		/// When Pac-Man was turn upward, near the limit of the grid, Pinky tried to land
		/// in another area, near Pac-Man.In this version, the program won't simulate this bug.
		/// </para>
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public override Direction? Strategy(GameTime gameTime)
		{
			// Get a grid of 4x4 of cell in front of pac:
			/*Cell[,] area = new Cell[4,4];
			Vector2 pac = GhostManager.Instance.Pac.ConvertPositionToTileIndexes();
			int width = GhostManager.Instance.Map.Width;
			int height = GhostManager.Instance.Map.Height;
			int mini, maxi;
			int minj, maxj;
			switch (PacRepresentation.Instance.LookingTo)
			{
				case Direction.UP:
					mini = (int) pac.X - 2;
					maxi = (int) pac.X + 1;
					minj = (int) pac.Y - 6;
					maxj = (int) pac.Y - 3;
					break;
				case Direction.DOWN:
					mini = (int)pac.X - 2;
					maxi = (int)pac.X + 1;
					minj = (int)pac.Y + 3;
					maxj = (int)pac.Y + 6;
					break;
				case Direction.LEFT:
					mini = (int)pac.X - 6;
					maxi = (int)pac.X - 3;
					minj = (int)pac.Y - 1;
					maxj = (int)pac.Y + 2;
					break;
				default:
					mini = (int)pac.X + 3;
					maxi = (int)pac.X + 6;
					minj = (int)pac.Y - 2;
					maxj = (int)pac.Y + 1;
					break;
			}

			// Get all the cells from the maze. If one coordinate is out of range, replace it bu null in the array.
			for (int i = mini; i <= maxi; i++)
			{
				for (int j = minj; j <= maxj; j++)
				{
					if (0 <= i && i < width &&
						0 <= j && j < height)
						area[i - mini, j - minj] = GhostManager.Instance.Map[i, j];
					else
						area[i - mini, j - minj] = null;
				}
			}

			Vector2 goal = new Vector2(-1, -1);
			int rec = 0;
			while (goal.Equals(new Vector2(-1, -1)))
			{
				Random r = new Random((int)Math.Round(gameTime.TotalGameTime.TotalMilliseconds));
				int x = r.Next(0, 4);
				int y = r.Next(0, 4);

				if (area[x, y] != null)
				{
					Vector3 result = area[x, y].Dimension.Min;
					goal = new Vector2(result.X, result.Y);
				}

				rec++;
				if (rec >= 100)
					throw new InfiniteLoopException();
			}

			try
			{
				Dijkstra dijkstra = new Dijkstra(GhostManager.Instance.Map);
				return dijkstra.ComputeDirection(
						// Start: Current ghost position
						ConvertPositionToTileIndexes(),
						// Destination: Pac's position
						ConvertPositionToTileIndexes(goal));
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

			base.Initialize();
		}

		protected override void LoadContent()
		{
			tx_ghost = Game.Content.Load<Texture2D>(@"Images\ghost_pinky");
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
