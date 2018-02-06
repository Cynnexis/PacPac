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

namespace PacPac
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Engine : Microsoft.Xna.Framework.Game
	{
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private SpriteFont sf_font;

		private GameState state;
		private GameTime gameTime;

		private Menu menu;
		private Maze maze;
		private Blinky blinky;
		private Pinky pinky;
		private Inky inky;
		private Clyde clyde;
		private Pac pac;

		private int score;
		private int level;

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
				score = value;
				if (score != 0 && score % 5000 == 0)
					pac.Life++;
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

			Score = 0;
			Level = 1;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			State = GameState.StartMenu;

			menu = new Menu(this);
			maze = new Maze(this);
			blinky = new Blinky(this);
			pinky = new Pinky(this);
			inky = new Inky(this);
			clyde = new Clyde(this);
			
			pac = new Pac(this, maze);

			pac.Ghosts.Add(blinky);
			pac.Ghosts.Add(pinky);
			pac.Ghosts.Add(inky);
			pac.Ghosts.Add(clyde);

			GhostManager.Instance.Initialize(maze, pac, blinky, new Ghost[] { pinky, inky, clyde });

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

			sf_font = this.Content.Load<SpriteFont>(@"Fonts\Verdana");

			SoundManager.Instance.LoadContent(this);
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
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
					
					break;
				case GameState.Pause:
					break;
				case GameState.Stop:
					break;
				default: // Playing & Pause
					spriteBatch.DrawString(sf_font,
						"PacPac",
						new Vector2(maze.Dimension.Max.X + 10, 20),
						Color.White);

					spriteBatch.DrawString(sf_font,
						"Life: ",
						new Vector2(maze.Dimension.Max.X + 10, 50),
						Color.White);

					Texture2D tx_pac = PacRepresentation.Instance.CurrentTexture != null ? PacRepresentation.Instance.CurrentTexture : pac.Texture;
					for (int i = 0; i < pac.Life; i++)
						spriteBatch.Draw(tx_pac, new Vector2(maze.Dimension.Max.X + 10 + i * Maze.SPRITE_DIMENSION, 80), Color.White);

					spriteBatch.DrawString(sf_font,
						"Score: " + Score,
						new Vector2(maze.Dimension.Max.X + 10, 110),
						Color.White);

					spriteBatch.DrawString(sf_font,
						"Level " + Level,
						new Vector2(maze.Dimension.Max.X + 10, 140),
						Color.White);

#if DEBUG
					spriteBatch.DrawString(sf_font,
						"Pac : (" + pac.Position.X + " ; " + pac.Position.Y + ")",
						new Vector2(maze.Dimension.Max.X + 10, 500),
						Color.White);
					spriteBatch.DrawString(sf_font,
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

// TODO: Replace Pac.Ghosts by GhostManager.Ghosts
// TODO: Remove PacRepresentation - It's useless
// TODO: Implement Game Over screen

/* COMMIT:
 * Random movement enchanced
 * Ghost can now run away when they are edible
*/
