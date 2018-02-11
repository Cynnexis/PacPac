using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PacPac.Grid;
using PacPac.Grid.Exceptions;
using System.Threading;
using PacPac.Core.Characters.GhostCharacters;

namespace PacPac.Core
{
	/// <summary>
	/// Pac class
	/// </summary>
	public class Pac : LivingGear
	{
		#region Constants
		/// <summary>
		/// Default number of life at the beginning of the game
		/// </summary>
		public static int DEFAULT_LIFE = 3;

		/// <summary>
		/// Default time of invincibility for pac when it eats a fruit.
		/// </summary>
		public static int DEFAULT_INVINCIBLE_TIMEOUT_SECONDS = 10;
		#endregion

		#region Variables & Properties
		private PacRepresentation representation;
		private Maze maze;
		private int life;
		private bool invincible;

		private Timer timer = null;

		/// <summary>
		/// Contains all textures and graphic logic
		/// </summary>
		public PacRepresentation Representation
		{
			get { return representation; }
			set { representation = value; }
		}

		/// <summary>
		/// The current number of life
		/// </summary>
		public int Life
		{
			get { return life; }
			set { life = value; }
		}

		/// <summary>
		/// Tell if pac is in Invincibility mode. If yes, then the property update the mode of the ghosts
		/// </summary>
		public bool Invincible
		{
			get { return invincible; }
			set
			{
				invincible = value;

				if (State == GameState.Playing)
				{
					if (GhostManager.Instance.IsInitialized)
						foreach (Ghost g in GhostManager.Instance.Ghosts)
							if (g.State != GhostState.INITIALIZING && g.State != GhostState.MOVING_MAZE)
								g.State = invincible ? GhostState.EDIBLE : GhostState.RUNNING;

					if (invincible)
					{
						SoundManager.Instance.PauseMusic();
						SoundManager.Instance.PlayInvincible();
						if (timer == null)
						{
							timer = new Timer((Object obj) =>
							{
								Invincible = false;
								timer.Dispose();
								timer = null;
							}, null, DEFAULT_INVINCIBLE_TIMEOUT_SECONDS * 1000, Timeout.Infinite);
						}
						// If pac eat another fruit while invincible
						else
							timer.Change(DEFAULT_INVINCIBLE_TIMEOUT_SECONDS * 1000, Timeout.Infinite);
					}
					else
					{
						SoundManager.Instance.PlayMusic();
						SoundManager.Instance.PauseInvincible();
					}
				}
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="game">The game instance</param>
		/// <param name="maze">The current maze</param>
		public Pac(Game game, Maze maze) : base(game)
		{
			Representation = new PacRepresentation();
			this.maze = maze;

			Life = DEFAULT_LIFE;

			// Seach for the startup tile
			List<Cell> startupCells = maze.SearchTile(TileType.PAC_STARTUP);
			if (startupCells != null && startupCells.Count >= 1)
			{
				Vector3 result = startupCells[0].Dimension.Min;
				StartingPoint = new Vector2(result.X, result.Y);
			}
			// If the startup tile does not exist, search for the first empty tile
			else
			{
				List<Cell> otherCells = maze.SearchTile(TileType.EMPTY);
				if (otherCells != null && otherCells.Count >= 1)
				{
					Vector3 result = startupCells[0].Dimension.Min;
					StartingPoint = new Vector2(result.X, result.Y);
					maze[(int) StartingPoint.X, (int) StartingPoint.Y].Tile = TileType.PACDOT;
				}
				// If no empty tile hs been detected, search for the first pacdot tile
				else
				{
					List<Cell> otherCellsAgain = maze.SearchTile(TileType.PACDOT);
					if (otherCellsAgain != null && otherCellsAgain.Count >= 1)
					{
						Vector3 result = startupCells[0].Dimension.Min;
						StartingPoint = new Vector2(result.X, result.Y);
						maze[(int) StartingPoint.X, (int) StartingPoint.Y].Tile = TileType.PACDOT;
					}
					// Otherwise, throw an exception
					else
						throw new EmptyMazeFileException(maze.Path);
				}
			}

			Position = StartingPoint;

			this.Game.Components.Add(this);
		}
		#endregion

		#region LivingGear Override
		/// <summary>
		/// Kill pac and play the dying animation.
		/// </summary>
		public override void Die()
		{
			Representation.IsDying = true;
			Representation.OnDieStateChangedAction = (bool isDying) =>
			{
				if (!isDying)
				{
					// Test if a ghost is in the starting point
					if (GhostManager.Instance.IsInitialized)
					{
						bool isDeadly = false;
						for (int i = 0; i < GhostManager.Instance.Ghosts.Count && !isDeadly; i++)
							isDeadly = GhostManager.Instance.Ghosts[i].ConvertPositionToTileIndexes().Equals(ConvertPositionToTileIndexes(StartingPoint));

						// If there is no ghost, then place pac there
						if (!isDeadly)
							Position = StartingPoint;
						// Otherwise, the program must found another place
						else
						{
							List<Cell> available = maze.SearchTile(TileType.PACDOT);

							if (available == null)
								available = new List<Cell>();

							if (available.Count <= 0)
								available.AddRange(maze.SearchTile(TileType.EMPTY));

							Random r = new Random();
							Vector3 res = available[r.Next(available.Count)].Dimension.Min;
							Position = new Vector2(res.X, res.Y);
						}
					}
					else
						Position = StartingPoint;

					SoundManager.Instance.PlayMusic();
					Life -= 1;
					Representation.OnDieStateChangedAction = (bool isDying1) => { };
				}
			};
			SoundManager.Instance.PauseMusic();
			SoundManager.Instance.PlayPacEaten();
		}
		#endregion

		#region GameComponent Overrides
		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run. This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			this.Speed = new Vector2(2, 2);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			Representation.LoadContent(Game);
			Representation.OnTextureChangedAction = (Texture2D texture) => { Texture = texture; };

			base.LoadContent();
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			if (State == GameState.Playing)
			{
				Representation.Update(gameTime);

				// If pac is not dying
				if (!Representation.IsDying)
				{
					// Updating Position according to the Controls
					Vector2 oldPos = Position;
					Vector2 oldTile = ConvertPositionToTileIndexes();
					if (Controls.CheckKeyState(Controls.PAC_UP))
					{
						Position = new Vector2(Position.X, Position.Y - Speed.Y);
						Representation.LookingTo = Direction.UP;
					}
					else if (Controls.CheckKeyState(Controls.PAC_LEFT))
					{
						Position = new Vector2(Position.X - Speed.X, Position.Y);
						Representation.LookingTo = Direction.LEFT;
					}
					else if (Controls.CheckKeyState(Controls.PAC_DOWN))
					{
						Position = new Vector2(Position.X, Position.Y + Speed.Y);
						Representation.LookingTo = Direction.DOWN;
					}
					else if (Controls.CheckKeyState(Controls.PAC_RIGHT))
					{
						Position = new Vector2(Position.X + Speed.X, Position.Y);
						Representation.LookingTo = Direction.RIGHT;
					}

					// Check if the move is valid

					Vector2 tile = ConvertPositionToTileIndexes();

					if (Cell.IsTileTypeBlock(maze[tile].Tile))
						Position = oldPos;
					else
					{
						// Get the tile toward pac
						Cell cellToward = null;
						bool exceptionReached = false;
						try
						{
							switch (Representation.LookingTo)
							{
								case Direction.UP:
									cellToward = maze[(int)tile.X, (int)tile.Y - 1];
									break;
								case Direction.DOWN:
									cellToward = maze[(int)tile.X, (int)tile.Y + 1];
									break;
								case Direction.LEFT:
									cellToward = maze[(int)tile.X - 1, (int)tile.Y];
									break;
								case Direction.RIGHT:
									cellToward = maze[(int)tile.X + 1, (int)tile.Y];
									break;
								default:
									cellToward = maze[(int)tile.X + 1, (int)tile.Y];
									break;
							}
						}
#pragma warning disable CS0168 // La variable est déclarée mais jamais utilisée
						catch (IndexOutOfRangeException ignored)
						{
							bool didPacTeleport = maze.TM.Teleport(this);

							if (!didPacTeleport)
							{
								Position = oldPos;
								exceptionReached = true;
							}
						}
#pragma warning restore CS0168 // La variable est déclarée mais jamais utilisée

						// Check if pac can go forward
						if (!exceptionReached && cellToward != null && Dimension.Intersects(cellToward.Dimension) && Cell.IsTileTypeBlock(cellToward.Tile))
							Position = oldPos;
					}

					Ghost ghostDetected = null;
					if (GhostManager.Instance.IsInitialized && GhostManager.Instance.Ghosts != null)
						foreach (Ghost g in GhostManager.Instance.Ghosts)
							if (ConvertPositionToTileIndexes().Equals(g.ConvertPositionToTileIndexes()))
								ghostDetected = g;

					// Check if pac runs into a ghost
					if (ghostDetected != null)
					{
						// If the ghost is edible (pac is in invincible mode), the ghost dies and must returns to the ghost start area
						if (ghostDetected.State == GhostState.EDIBLE)
						{
							ghostDetected.Die();
							
							// Try to add +100 (score)
#pragma warning disable CS0168
							try
							{
								((Engine) Game).Score += 100;
							}
							catch (InvalidCastException ignored) { }
#pragma warning restore CS0168
						}
						// Otherwise, pac loses a life
						else if (ghostDetected.State != GhostState.EATEN)
							Die();
					}

					// Check if pac runs into a pacdot
					if (maze[(int)tile.X, (int)tile.Y].Tile == TileType.PACDOT)
					{
						maze[(int)tile.X, (int)tile.Y].Tile = TileType.EMPTY;
						((Engine)Game).Score += 1;
						SoundManager.Instance.PlayPacEatPacDot();
					}
					// Check if pac runs into a fruit
					else if (maze[(int)tile.X, (int)tile.Y].Tile == TileType.FRUIT)
					{
						maze[(int)tile.X, (int)tile.Y].Tile = TileType.EMPTY;
						((Engine)Game).Score += 50;
						Invincible = true;
					}
					// Check if pac runs into a teleporter
					else if (maze[(int)tile.X, (int)tile.Y].Tile == TileType.TELEPORTER)
						maze.TM.Teleport(this);
				}
			}

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (State == GameState.Playing)
			{
				if (Texture == null)
				{
					Representation.RefreshTexture();
					Texture = Representation.CurrentTexture;
				}

				Sprite.Begin();
				Sprite.Draw(Texture, Position, Color.Yellow);
				Sprite.End();
			}

			base.Draw(gameTime);
		}
		#endregion
	}
}
