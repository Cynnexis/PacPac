using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacPac.Core.Algorithms;
using PacPac.Core.Characters.GhostCharacters;
using PacPac.Core.Exceptions;
using PacPac.Grid;
using PacPac.Grid.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core
{
	public delegate void OnGhostStateChanged(GhostState state);

	public abstract class Ghost : LivingGear, GhostInterface
	{
		private GhostState state;
		/*private bool edible;
		private bool still;
		private bool isMovingToMaze;
		private bool eaten;*/
		private bool possessed;

		/// <summary>
		/// This variable is only used in the method "Move(GameTime)".
		/// <seealso cref="Move(GameTime)"/>
		/// </summary>
		protected Vector2 currentTileIndexes;

		/// <summary>
		/// Last direction computed by the strategy of the ghost. This variable is used in "Move(GameTime)"
		/// <seealso cref="Move(GameTime)"/>
		/// </summary>
		private Direction? lastDirection;

		protected Texture2D tx_ghost;
		protected Texture2D tx_edible;

		public GhostState State
		{
			get { return state; }
			set
			{
				GhostState oldValue = state;
				state = value;

				if (oldValue != state)
				{
					Refresh();
					lastDirection = null;
					currentTileIndexes = new Vector2(-1, -1);
					OnGhostStateChangedAction?.Invoke(state);
				}
			}
		}

		/*
		/// <summary>
		/// Indicate if the ghost can be hurt by pac, because it is invincible
		/// </summary>
		public bool Edible
		{
			get { return edible; }
			set
			{
				edible = value;

				Refresh();
			}
		}

		/// <summary>
		/// Indicate if the ghost is not moving, in the ghost start point
		/// </summary>
		public bool Still
		{
			get { return still; }
			set { still = value; }
		}

		/// <summary>
		/// Indicate if the ghost is moving from the startup point to the entrance of the maze
		/// </summary>
		public bool IsMovingToMaze
		{
			get { return isMovingToMaze; }
			set { isMovingToMaze = value; }
		}

		/// <summary>
		/// Indicate if the ghost has been eaten by pac, and is moving to the ghost start point
		/// </summary>
		public bool Eaten
		{
			get { return eaten; }
			set { eaten = value; }
		}*/

		/// <summary>
		/// Indicate if the ghost is moving because of another entity.
		/// If Possessed = true, then the ghost is not moving accoridng to its algorithm, but by another method/class, such as
		/// GhostManager. While Possessed is true, the ghost cannot move by itself.
		/// <seealso cref="GhostManager"/>
		/// </summary>
		public bool Possessed
		{
			get { return possessed; }
			set { possessed = true; }
		}

		public OnGhostStateChanged OnGhostStateChangedAction { get; set; }

		public Ghost(Game game) : base(game) {
			State = GhostState.INITIALIZING;
			currentTileIndexes = new Vector2(-1, -1);
		}

		public void Refresh()
		{
			if (State == GhostState.EDIBLE)
				Texture = tx_edible;
			else if (State == GhostState.EATEN)
				Texture = tx_edible;
			else
				Texture = tx_ghost;
		}

		public override void Die()
		{
			State = GhostState.EATEN;
		}

		public abstract Direction? Strategy(GameTime gameTime);

		public void Move(GameTime gameTime)
		{
			if (GhostManager.Instance.IsInitialized)
			{
				// If it is the first time than Move is called OR the position of the ghost changed, then refresh the algorithm
				if (lastDirection == null || currentTileIndexes.Equals(new Vector2(-1, -1)) || !currentTileIndexes.Equals(ConvertPositionToTileIndexes()))
				{
					switch (State)
					{
						case GhostState.INITIALIZING:
							lastDirection = null;
							break;
						case GhostState.MOVING_MAZE:
							lastDirection = MoveToMaze();
							break;
						case GhostState.RUNNING:
							lastDirection = Strategy(gameTime);
							break;
						case GhostState.EDIBLE:
							//lastDirection = Strategy(gameTime);
							Vector2? possiblePlace = GenerateRandomPlace();

							if (possiblePlace == null)
								lastDirection = null;
							else
							{
#pragma warning disable CS0168
								try
								{
									Dijkstra dijkstra = new Dijkstra(GhostManager.Instance.Map);
									lastDirection = dijkstra.ComputeDirection(
										ConvertPositionToTileIndexes(),
										(Vector2) possiblePlace
									);
								}
								catch (InfiniteLoopException ignored) { }
#pragma warning restore CS0168
							}
							break;
						case GhostState.EATEN:
							lastDirection = MoveToStart();
							break;
						default:
							lastDirection = null;
							break;
					}

					currentTileIndexes = ConvertPositionToTileIndexes();
				}

				if (lastDirection != null)
					MoveToDirection((Direction)lastDirection);
			}
		}
		
		public void MoveToDirection(Direction direction)
		{
			switch (direction)
			{
				case Direction.UP:
					Position = new Vector2(Position.X, Position.Y - Speed.Y);
					break;
				case Direction.DOWN:
					Position = new Vector2(Position.X, Position.Y + Speed.Y);
					break;
				case Direction.LEFT:
					goto default;
				case Direction.RIGHT:
					Position = new Vector2(Position.X + Speed.X, Position.Y);
					break;
				default:
					Position = new Vector2(Position.X - Speed.X, Position.Y);
					break;
			}
		}

		public Direction? MoveToMaze()
		{
			if (State == GhostState.MOVING_MAZE)
			{
				if (GhostManager.Instance.IsInitialized)
				{
					Dijkstra dijkstra = new Dijkstra(GhostManager.Instance.Map);
					Direction? dir = dijkstra.ComputeDirection(
						ConvertPositionToTileIndexes(),
						ConvertPositionToTileIndexes(GhostManager.Instance.Entrance)
					);

					if (dir != null)
					{
						MoveToDirection((Direction)dir);
						return dir;
					}
					else
						State = GhostState.RUNNING;
				}
			}
			return null;
		}

		public Direction? MoveToStart()
		{
			Dijkstra dijkstra = new Dijkstra(GhostManager.Instance.Map);
			Direction? direction = dijkstra.ComputeDirection(
							// Start: Current ghost position
							ConvertPositionToTileIndexes(),
							// Destination: Pac's position
							ConvertPositionToTileIndexes(StartingPoint));

			if (direction != null)
			{
				//MoveToDirection((Direction)direction);
				return direction;
			}
			// Else, if the ghost arrives
			else
				State = GhostState.RUNNING;

			return null;
		}

		public Vector2? GenerateRandomPlace()
		{
			if (GhostManager.Instance.IsInitialized) {
				// Get all the pacdot tiles
				List<Cell> list = GhostManager.Instance.Map.SearchTile(TileType.PACDOT);

				// If the list is null, instanciate it
				if (list == null)
					list = new List<Cell>();

				// If there is not enough tiles, add the empty ones to the list
				if (list.Count <= 20)
					list.AddRange(GhostManager.Instance.Map.SearchTile(TileType.EMPTY));

				// Amongst all the tiles, get one randomly
				Random r = new Random();

				Vector3 result = list[r.Next(0, list.Count)].Dimension.Min;
				return ConvertPositionToTileIndexes(new Vector2(result.X, result.Y));
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
			Position = StartingPoint;
			//currentTileIndexes = ConvertPositionToTileIndexes();
			Speed = new Vector2(1.5f, 1.5f);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			if (tx_ghost == null)
				// The default value is Blinky's texture
				tx_ghost = Game.Content.Load<Texture2D>(@"Images\ghost_blinky");

			if (tx_edible == null)
				tx_edible = Game.Content.Load<Texture2D>(@"Images\ghost_edible");

			Refresh();
			base.LoadContent();
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			if (!Possessed && base.State == GameState.Playing)
				Move(gameTime);

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (base.State == GameState.Playing)
			{
				Sprite.Begin();
				Sprite.Draw(Texture, Position, Color.White);
				Sprite.End();
			}
			base.Draw(gameTime);
		}
		#endregion
	}
}
