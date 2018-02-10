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
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public class Pac : LivingGear
	{
		#region Constants
		public static int DEFAULT_LIFE = 3;
		public static int DEFAULT_INVINCIBLE_TIMEOUT_SECONDS = 10;
		#endregion

		#region Variables & Properties
		private PacRepresentation representation;
		private Maze maze;
		private int life;
		private bool invincible;
		private List<Ghost> ghosts;

		private Timer timer = null;

		public PacRepresentation Representation
		{
			get { return representation; }
			set { representation = value; }
		}

		public int Life
		{
			get { return life; }
			set { life = value; }
		}

		public bool Invincible
		{
			get { return invincible; }
			set
			{
				invincible = value;

				if (State == GameState.Playing)
				{
					foreach (Ghost g in Ghosts)
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

		public List<Ghost> Ghosts
		{
			get { return ghosts; }
			set { ghosts = value; }
		}
		#endregion

		#region Constructor
		public Pac(Game game, Maze maze) : base(game)
		{
			Representation = new PacRepresentation();
			this.maze = maze;

			Life = DEFAULT_LIFE;
			Ghosts = new List<Ghost>(4);

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
		public override void Die()
		{
			Representation.IsDying = true;
			Representation.OnDieStateChangedAction = (bool isDying) =>
			{
				if (!isDying)
				{
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
			// TODO: Add your initialization code here
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
			// TODO: Add your update code here

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
					catch (IndexOutOfRangeException ignored)
					{
						Console.Error.WriteLine(ignored.StackTrace);
						bool didPacTeleport = maze.TM.Teleport(this);

						if (!didPacTeleport)
						{
							Position = oldPos;
							exceptionReached = true;
						}
					}

					// Check if pac can go forward
					if (!exceptionReached && cellToward != null && Dimension.Intersects(cellToward.Dimension) && Cell.IsTileTypeBlock(cellToward.Tile))
						Position = oldPos;

					Ghost ghostDetected = null;
					if (Ghosts != null)
						foreach (Ghost g in Ghosts)
							if (ConvertPositionToTileIndexes().Equals(g.ConvertPositionToTileIndexes()))
								ghostDetected = g;

					// Check if pac runs into a ghost
					if (ghostDetected != null)
					{
						// If the ghost is edible (pac is in invincible mode), the ghost dies and must returns to the ghost start area
						if (ghostDetected.State == GhostState.EDIBLE)
							ghostDetected.Die();
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
						//SoundManager.Instance.PlayInvincible();
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
