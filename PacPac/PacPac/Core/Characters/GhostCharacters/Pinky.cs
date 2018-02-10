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
		/// <summary>
		/// Countdown until Pinky can update its strategy
		/// </summary>
		public static int COUNTDOWN = 2; // seconds

		/// <summary>
		/// Last time Pinky updated its strategy
		/// </summary>
		private int lastStrategyUpdate; // seconds

		private Vector2 goal;
		private bool hasFallenInInfiniteLoop;

		public Pinky(Game game) : base(game)
		{
			lastStrategyUpdate = -1;
			goal = new Vector2(-1, -1);
			hasFallenInInfiniteLoop = false;
			this.Game.Components.Add(this);
		}

		/// <summary>
		/// Pinky Strategy: Land on a square of 10x10 tiles located 2 tiles in front of pac
		/// to ambush him.
		/// <para>
		/// Note: In the first Pac-Man game, Pinky had actually an overflow bug.
		/// When Pac-Man was turn upward, near the limit of the grid, Pinky tried to land
		/// in another area, near Pac-Man. In this version, the program won't simulate this bug.
		/// </para>
		/// </summary>
		/// <param name="gameTime"></param>
		/// <returns></returns>
		public override Direction? Strategy(GameTime gameTime)
		{
			// Get a grid of 10x10 of cell in front of pac:
			Cell[,] area = new Cell[10, 10];
			List<Cell> available = new List<Cell>();

			// If Pkinky is in its goal OR dikstra's algorithm fell into an infinite loop OR the countdown is over, then update the strategy
			if (ConvertPositionToTileIndexes().Equals(goal) ||
				hasFallenInInfiniteLoop ||
				lastStrategyUpdate == -1 ||
				(gameTime.TotalGameTime.TotalSeconds != 0 &&
				((int)Math.Round(gameTime.TotalGameTime.TotalSeconds)) != lastStrategyUpdate &&
				((int)Math.Round(gameTime.TotalGameTime.TotalSeconds)) % COUNTDOWN == 0))
			{
				Vector2 pac = GhostManager.Instance.Pac.ConvertPositionToTileIndexes();
				int width = GhostManager.Instance.Map.Width;
				int height = GhostManager.Instance.Map.Height;
				int mini, maxi;
				int minj, maxj;
				switch (GhostManager.Instance.Pac.Representation.LookingTo)
				{
					case Direction.UP:
						mini = (int) pac.X - 5;
						maxi = (int) pac.X + 4;
						minj = (int) pac.Y - 12;
						maxj = (int) pac.Y - 3;
						break;
					case Direction.DOWN:
						mini = (int)pac.X - 5;
						maxi = (int)pac.X + 4;
						minj = (int)pac.Y + 3;
						maxj = (int)pac.Y + 12;
						break;
					case Direction.LEFT:
						mini = (int)pac.X - 12;
						maxi = (int)pac.X - 3;
						minj = (int)pac.Y - 4;
						maxj = (int)pac.Y + 5;
						break;
					default:
						mini = (int)pac.X + 3;
						maxi = (int)pac.X + 12;
						minj = (int)pac.Y - 5;
						maxj = (int)pac.Y + 4;
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

				// Select only available tiles
				for (int i = 0; i < area.GetLength(0); i++)
					for (int j = 0; j < area.GetLength(1); j++)
						if (area[i, j] != null && !Cell.IsTileTypeBlock(area[i, j].Tile))
							available.Add(area[i, j]);
				
				Random r = new Random((int)Math.Round(gameTime.TotalGameTime.TotalMilliseconds));

				// If no tiles has been found, go to a random place
				if (available.Count <= 0)
				{
					/*
					while (result.Equals(new Vector3(0, 0, 0)))
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
						result = list[r.Next(0, list.Count)].Dimension.Min;
					}
					goal = ConvertPositionToTileIndexes(new Vector2(result.X, result.Y));
					*/

					Vector2? res = GenerateRandomPlace();

					if (res == null)
					{
						hasFallenInInfiniteLoop = true;
						return null;
					}

					goal = (Vector2) res;
				}
				else
				{
					Vector3 result = available[r.Next(0, available.Count)].Dimension.Min;
					goal = ConvertPositionToTileIndexes(new Vector2(result.X, result.Y));
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
						goal);
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
