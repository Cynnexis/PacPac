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
		private Vector2 currentTileIndexes;

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
			//Eaten = true;
			State = GhostState.EATEN;
		}

		public abstract Direction? Strategy(GameTime gameTime);

		public void Move(GameTime gameTime)
		{
			if (GhostManager.Instance.IsInitialized)
			{
				// If the position of the ghost changed, then refresh the algorithm
				if (currentTileIndexes.Equals(ConvertPositionToTileIndexes()))
				{
					Direction? direction;

					switch (State)
					{
						case GhostState.INITIALIZING:
							break;
						case GhostState.MOVING_MAZE:
							MoveToMaze();
							break;
						case GhostState.RUNNING:
							direction = Strategy(gameTime);
							if (direction != null)
								MoveToDirection((Direction)direction);
							break;
						case GhostState.EDIBLE:
							direction = Strategy(gameTime);
							if (direction != null)
								MoveToDirection((Direction)direction);
							break;
						case GhostState.EATEN:
							MoveToStart();
							break;
						default:
							break;
					}

					currentTileIndexes = ConvertPositionToTileIndexes();
				}
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

		public void MoveToMaze()
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
						MoveToDirection((Direction) dir);
					else
						State = GhostState.RUNNING;
				}
			}
		}

		public void MoveToStart()
		{
			Dijkstra dijkstra = new Dijkstra(GhostManager.Instance.Map);
			Direction? direction = dijkstra.ComputeDirection(
							// Start: Current ghost position
							ConvertPositionToTileIndexes(),
							// Destination: Pac's position
							ConvertPositionToTileIndexes(StartingPoint));

			if (direction != null)
				MoveToDirection((Direction)direction);
			// Else, if the ghost arrives
			else
				State = GhostState.RUNNING;
		}
		
		public static Random GetRandomInstance()
		{
			return new Random(
#if DEBUG
					// If DEBUG, use the same seed for each execution
					42
#else
					// If RELEASE, use the UNIX timestamp as seed
					(int) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds)
#endif
					);
		}

		#region GameComponent Overrides
		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			Position = StartingPoint;
			currentTileIndexes = ConvertPositionToTileIndexes();
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
			if (!Possessed)
				Move(gameTime);

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			Sprite.Begin();
			Sprite.Draw(Texture, Position, Color.White);
			Sprite.End();
			base.Draw(gameTime);
		}
		#endregion
	}
}
