using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac
{
	public class FontManager : AbstractSingleton
	{
		private static FontManager instance = new FontManager();

		private SpriteFont sf_verdana;
		private SpriteFont sf_arcade;
		private SpriteFont sf_crackman;
		private SpriteFont sf_crackmanTitle;

		public static FontManager Instance
		{
			get { return instance; }
			private set { instance = value; }
		}

		public SpriteFont Verdana
		{
			get { return sf_verdana; }
			private set { sf_verdana = value; }
		}

		public SpriteFont Arcade
		{
			get { return sf_arcade; }
			private set { sf_arcade = value; }
		}

		public SpriteFont Crackman
		{
			get { return sf_crackman; }
			private set { sf_crackman = value; }
		}

		public SpriteFont CrackmanTitle
		{
			get { return sf_crackmanTitle; }
			private set { sf_crackmanTitle = value; }
		}

		private FontManager() : base() { }

		public void LoadContent(Game game)
		{
			if (game == null)
				throw new NullReferenceException();

			Verdana = game.Content.Load<SpriteFont>(@"Fonts\Verdana");
			Arcade = game.Content.Load<SpriteFont>(@"Fonts\Arcade");
			Crackman = game.Content.Load<SpriteFont>(@"Fonts\Crackman");
			CrackmanTitle = game.Content.Load<SpriteFont>(@"Fonts\CrackmanTitle");

			IsInitialized = true;
		}
	}
}
