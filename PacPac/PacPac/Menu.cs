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

		private Texture2D tx_play;
		private Texture2D tx_exit;

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
			tx_play = Game.Content.Load<Texture2D>(@"Images\play");
			tx_exit = Game.Content.Load<Texture2D>(@"Images\exit");
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
						((Game.GraphicsDevice.Viewport.Height - tx_exit.Height) / 2) + 200);

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
				sprite.Begin();
				if (Type == MenuType.START)
				{
					sprite.DrawString(FontManager.Instance.Crackman,
					"PacPac",
					new Vector2((Game.GraphicsDevice.Viewport.Width / 2) - 20, Game.GraphicsDevice.Viewport.Height / 6),
					Color.White);
				}
				else if (Type == MenuType.GAMEOVER)
				{
					sprite.DrawString(FontManager.Instance.Crackman,
					"Game Over",
					new Vector2((Game.GraphicsDevice.Viewport.Width / 2) - 50, Game.GraphicsDevice.Viewport.Height / 6),
					Color.White);
				}
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
