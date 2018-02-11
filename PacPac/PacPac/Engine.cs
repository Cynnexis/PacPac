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
using PacPac.Core;
using PacPac.Core.Characters.GhostCharacters;
using System.Threading;

namespace PacPac
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Engine : Microsoft.Xna.Framework.Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;

		private GameState state;
		private GameTime gameTime;

		private Menu menu;
		private Maze maze;
		private Blinky blinky;
		private Pinky pinky;
		private Inky inky;
		private Clyde clyde;
		private Pac pac;
		private int lastPacLife;

		private int score;
		private int bonus; // The point the 'score'	attribute just obtained
		private Timer t_bonus;
		private int level;

		/// <summary>
		/// When this attribute is true, it means that during the previous game, pacman died, and a "Game Over" screen must be displayed.
		/// Default value is false.
		/// </summary>
		private bool pacDied;

		public GameState State
		{
			get { return state; }
			set
			{
				GameState oldValue = state;
				state = value;

				if (oldValue == GameState.StartMenu && state == GameState.Playing)
				{
					SoundManager.Instance.PlayMusic();
					GhostManager.Instance.PlayBeginning = this.gameTime != null ? (int) this.gameTime.TotalGameTime.TotalSeconds : 0;
				}
			}
		}
		public int Score
		{
			get { return score; }
			set
			{
				int oldValue = score;
				score = value;
				if (score != 0 && score % 5000 == 0)
					pac.Life++;

				Bonus = score - oldValue;
			}
		}

		public int Bonus
		{
			get { return bonus; }
			protected set
			{
				bonus = value;

				// The bonus is available only for 3s. After that, it is equal to 0 again
				if (t_bonus == null)
				{
					t_bonus = new Timer((Object o) =>
					{
						bonus = 0;
						t_bonus.Dispose();
						t_bonus = null;
					}, null, 1000, Timeout.Infinite);
				}
				else
					t_bonus.Change(1000, Timeout.Infinite);
			}
		}

		public int Level
		{
			get { return level; }
			set { level = value; }
		}

		public Engine()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			lastPacLife = -1;
			pacDied = false;
		}

		public void NewGame()
		{
			this.Components.Clear();

			SoundManager.Instance.StopMusic();
			SoundManager.Instance.StopInvincible();

			menu = new Menu(this, pacDied ? MenuType.GAMEOVER : MenuType.START);
			maze = new Maze(this);
			blinky = new Blinky(this);
			pinky = new Pinky(this);
			inky = new Inky(this);
			clyde = new Clyde(this);

			pac = new Pac(this, maze);

			if (lastPacLife > 0)
				pac.Life = lastPacLife;

			GhostManager.Instance.Initialize(maze, pac, blinky, new Ghost[] { pinky, inky, clyde });

			menu.Initialize();
			maze.Initialize();
			blinky.Initialize();
			pinky.Initialize();
			inky.Initialize();
			clyde.Initialize();
			pac.Initialize();

			// Reset pacDied attribute
			pacDied = false;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			SoundManager.Instance.LoadContent(this);
			FontManager.Instance.LoadContent(this);

			State = GameState.StartMenu;

			Score = 0;
			Level = 1;

			NewGame();

			IsMouseVisible = true;

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			graphics.PreferredBackBufferWidth = 1024;
			graphics.PreferredBackBufferHeight = 620;
			graphics.ApplyChanges();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			base.UnloadContent();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			this.gameTime = gameTime;
			GhostManager.Instance.Update(gameTime);

			if (State == GameState.Playing)
			{
				// Check if pac ate all pacdot
				if (maze.SearchTile(TileType.PACDOT).Count == 0 && maze.SearchTile(TileType.FRUIT).Count == 0)
				{
					Level++;
					lastPacLife = pac.Life;
					NewGame();
					SoundManager.Instance.PlayMusic();
				}
				// Check if pac is dead
				else if (pac.Life <= 0)
				{
					// Reset the game
					pacDied = true;
					Initialize();
				}
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			
			spriteBatch.Begin();
			switch (State)
			{
				case GameState.StartMenu:
					// See Menu
					break;
				case GameState.Pause:
					break;
				case GameState.Stop:
					break;
				default: // Playing & Pause
					spriteBatch.DrawString(FontManager.Instance.Crackman,
						"PacPac",
						new Vector2(maze.Dimension.Max.X + 10, 20),
						Color.White);

					spriteBatch.DrawString(FontManager.Instance.Arcade,
						"Life: ",
						new Vector2(maze.Dimension.Max.X + 10, 50),
						Color.White);

					Texture2D tx_pac = pac.Representation.CurrentTexture != null ? pac.Representation.CurrentTexture : pac.Texture;
					for (int i = 0; i < pac.Life; i++)
						spriteBatch.Draw(tx_pac, new Vector2(maze.Dimension.Max.X + 10 + i * Maze.SPRITE_DIMENSION, 80), Color.White);

					// Drawn score
					spriteBatch.DrawString(FontManager.Instance.Arcade,
						"Score: " + Score,
						new Vector2(maze.Dimension.Max.X + 10, 110),
						Color.White);

					// Draw bonus
					if (Bonus != 0)
						spriteBatch.DrawString(FontManager.Instance.Arcade,
							(Bonus > 0 ? "+" : "") + Bonus,
							new Vector2(maze.Dimension.Max.X + 150, 110),
							Color.Yellow);

					spriteBatch.DrawString(FontManager.Instance.Arcade,
						"Level " + Level,
						new Vector2(maze.Dimension.Max.X + 10, 140),
						Color.White);

#if DEBUG
					spriteBatch.DrawString(FontManager.Instance.Arcade,
						"Pac : (" + pac.Position.X + " ; " + pac.Position.Y + ")",
						new Vector2(maze.Dimension.Max.X + 10, 500),
						Color.White);
					spriteBatch.DrawString(FontManager.Instance.Arcade,
						"       [" + pac.ConvertPositionToTileIndexes().X + ",  " + pac.ConvertPositionToTileIndexes().Y + "]",
						new Vector2(maze.Dimension.Max.X + 10, 530),
						Color.White);
#endif
					break;
			}
			
			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}

// TODO: Fix dying sound
// TODO: Fix lag due to ghosts (choose random spot)

/* COMMIT:
 * Dynamic background added for menu screen
 * Pac & ghosts sprites added in menu screen
 * Bonus system added
 * The life of pac is saved during the transiton of level
*/
