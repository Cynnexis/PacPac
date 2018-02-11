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
using PacPac.Core;

namespace PacPac
{
	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public class Menu : DrawableGameComponent
	{
		private SpriteBatch sprite;

		private MenuType type;

		private Texture2D tx_pac;
		private Texture2D tx_blinky;
		private Texture2D tx_pinky;
		private Texture2D tx_inky;
		private Texture2D tx_clyde;

		private Texture2D tx_play;
		private Texture2D tx_exit;

		// Dynamic Background attributes

		/// <summary>
		/// Rate of blue (from 0 to 1) for the background
		/// </summary>
		private float blue;

		/// <summary>
		/// Is the background going from black to blue (true) or from blue to black (false)?
		/// </summary>
		private bool raising;

		private Vector2 playPos;
		private Vector2 exitPos;

		public MenuType Type
		{
			get { return type; }
			set { type = value; }
		}

		public Menu(Game game, MenuType type = MenuType.START) : base(game)
		{
			Type = type;

			Game.Components.Add(this);
		}

		private GameState GetState()
		{
			GameState state = GameState.StartMenu;
			try
			{
				state = ((Engine)Game).State;
			}
			catch (InvalidCastException ex)
			{
				Console.Error.WriteLine(ex.StackTrace);
			}

			return state;
		}

		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			// TODO: Add your initialization code here
			sprite = new SpriteBatch(Game.GraphicsDevice);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			tx_pac = Game.Content.Load<Texture2D>(@"Images\pacman_lc");
			tx_blinky = Game.Content.Load<Texture2D>(@"Images\ghost_blinky");
			tx_pinky = Game.Content.Load<Texture2D>(@"Images\ghost_pinky");
			tx_inky = Game.Content.Load<Texture2D>(@"Images\ghost_inky");
			tx_clyde = Game.Content.Load<Texture2D>(@"Images\ghost_clyde");

			tx_play = Game.Content.Load<Texture2D>(@"Images\play");
			tx_exit = Game.Content.Load<Texture2D>(@"Images\exit");

			// Setting default value for the dynamic background
			blue = 0.2f;
			raising = true;

			base.LoadContent();
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			// TODO: Add your update code here
			if (GetState() == GameState.StartMenu)
			{
				SoundManager.Instance.PlayMenuMusic();

				playPos = new Vector2((Game.GraphicsDevice.Viewport.Width - tx_play.Width) / 2, (Game.GraphicsDevice.Viewport.Height - tx_play.Height) / 2);
				exitPos = new Vector2(
						(Game.GraphicsDevice.Viewport.Width - tx_exit.Width) / 2,
						((Game.GraphicsDevice.Viewport.Height - tx_exit.Height) / 2) + 100);

				MouseState mouse = Mouse.GetState();

				if (mouse.LeftButton == ButtonState.Pressed)
				{
					if (mouse.X >= playPos.X && mouse.X <= playPos.X + tx_play.Width &&
						mouse.Y >= playPos.Y && mouse.Y <= playPos.Y + tx_play.Height)
					{
						// TODO: Start the game
						Console.WriteLine("Play!");
						((Engine)Game).State = GameState.Playing;
					}
					else if (mouse.X >= exitPos.X && mouse.X <= exitPos.X + tx_exit.Width &&
							mouse.Y >= exitPos.Y && mouse.Y <= exitPos.Y + tx_exit.Height)
						Environment.Exit(0);
				}
			}
			else
				SoundManager.Instance.StopMenuMusic();

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (GetState() == GameState.StartMenu)
			{
				// Draw the background with the attribute 'blue'
				GraphicsDevice.Clear(new Color(0f, 0f, blue));

				// Update 'blue'. Every 100 miliseconds, add or remove 0.01 from 'blue', depending on the value of 'raising'
				if (Math.Round(gameTime.TotalGameTime.TotalMilliseconds) % 100 == 0)
				{
					// If the menu is going from dark to blue (raising == true) then add 0.01
					if (raising)
						blue += 0.01f;
					// Otherwise remove 0.01 from 'blue'
					else
						blue -= 0.01f;

					// If the variable 'blue' reaches its limit ]0.2 ; 0.8[, then invert the variable 'raising'
					if (blue >= 0.8f)
						raising = false;
					else if (blue <= 0.2f)
						raising = true;
				}

				// Drawn sprites
				sprite.Begin();

				// Draw the tile
				if (Type == MenuType.START)
				{
					sprite.DrawString(FontManager.Instance.CrackmanTitle,
					"PacPac",
					new Vector2((Game.GraphicsDevice.Viewport.Width / 2) - 130, Game.GraphicsDevice.Viewport.Height / 6),
					Color.White);
				}
				else if (Type == MenuType.GAMEOVER)
				{
					sprite.DrawString(FontManager.Instance.CrackmanTitle,
					"Game Over",
					new Vector2((Game.GraphicsDevice.Viewport.Width / 2) - 200, Game.GraphicsDevice.Viewport.Height / 6),
					Color.White);
				}

				// Draw pacpac
				sprite.Draw(tx_pac, new Vector2((Game.GraphicsDevice.Viewport.Width - tx_pac.Width) / 2, 200), Color.White);

				// Draw the ghosts
				sprite.Draw(tx_blinky, new Vector2(((Game.GraphicsDevice.Viewport.Width - tx_blinky.Width) / 2) + 40, 200), Color.White);
				sprite.Draw(tx_pinky, new Vector2(((Game.GraphicsDevice.Viewport.Width - tx_pinky.Width) / 2) + 65, 200), Color.White);
				sprite.Draw(tx_inky, new Vector2(((Game.GraphicsDevice.Viewport.Width - tx_inky.Width) / 2) + 90, 200), Color.White);
				sprite.Draw(tx_clyde, new Vector2(((Game.GraphicsDevice.Viewport.Width - tx_clyde.Width) / 2) + 115, 200), Color.White);

				// Draw buttons
				sprite.Draw(tx_play, playPos, Color.White);
				sprite.Draw(tx_exit,
					exitPos,
					Color.White);

				sprite.End();
			}

			base.Draw(gameTime);
		}
	}
}
