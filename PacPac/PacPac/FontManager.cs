using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac
{
	/// <summary>
	/// Singleton-class to manage all the fonts used in the game
	/// </summary>
	public class FontManager : AbstractSingleton
	{
		private static FontManager instance = new FontManager();

		private SpriteFont sf_verdana;
		private SpriteFont sf_arcade;
		private SpriteFont sf_crackman;
		private SpriteFont sf_crackmanTitle;

		/// <summary>
		/// Unique instance of the program
		/// </summary>
		public static FontManager Instance
		{
			get { return instance; }
			private set { instance = value; }
		}

		/// <summary>
		/// Verdana font
		/// </summary>
		public SpriteFont Verdana
		{
			get { return sf_verdana; }
			private set { sf_verdana = value; }
		}

		/// <summary>
		/// Arcade font
		/// </summary>
		public SpriteFont Arcade
		{
			get { return sf_arcade; }
			private set { sf_arcade = value; }
		}

		/// <summary>
		/// Crackman font
		/// </summary>
		public SpriteFont Crackman
		{
			get { return sf_crackman; }
			private set { sf_crackman = value; }
		}

		/// <summary>
		/// Crackman font, for title
		/// </summary>
		public SpriteFont CrackmanTitle
		{
			get { return sf_crackmanTitle; }
			private set { sf_crackmanTitle = value; }
		}

		/// <summary>
		/// Private constructor
		/// </summary>
		private FontManager() : base() { }

		/// <summary>
		/// Thiis method MUST BE called before using this class.
		/// </summary>
		/// <param name="game">The game instance to load content</param>
		/// <exception cref="ArgumentNullException">Thrown if
		/// <paramref name="game"/> is <c>null</c></exception>
		public void LoadContent(Game game)
		{
			if (game == null)
				throw new ArgumentNullException();

			Verdana = game.Content.Load<SpriteFont>(@"Fonts\Verdana");
			Arcade = game.Content.Load<SpriteFont>(@"Fonts\Arcade");
			Crackman = game.Content.Load<SpriteFont>(@"Fonts\Crackman");
			CrackmanTitle = game.Content.Load<SpriteFont>(@"Fonts\CrackmanTitle");

			IsInitialized = true;
		}
	}
}
