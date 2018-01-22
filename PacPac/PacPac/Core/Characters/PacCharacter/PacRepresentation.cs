using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using static PacPac.Core.PacRepresentation;

namespace PacPac.Core
{
	public delegate void OnLookingToChanged(Direction lookingTo);
	public delegate void OnMouthStateChanged(MouthState month);
	public delegate void OnDieStateChanged(bool isDying);

	public class PacRepresentation : ComponentRepresentation
	{
		public enum MouthState
		{
			OPEN, CLOSE
		}

		private Direction lookingTo;
		private MouthState month;
		private bool isDying;
		private GameTime current;

		private TimeSpan dieBegin;
		private const int transitionLengthMilli = 400;
		private int dieStep;

		private Texture2D tx_pac_rc;
		private Texture2D tx_pac_ro;
		private Texture2D tx_pac_tc;
		private Texture2D tx_pac_to;
		private Texture2D tx_pac_bc;
		private Texture2D tx_pac_bo;
		private Texture2D tx_pac_lc;
		private Texture2D tx_pac_lo;

		private Texture2D tx_die0;
		private Texture2D tx_die1;
		private Texture2D tx_die2;
		private Texture2D tx_die3;
		private Texture2D tx_empty;

		private Texture2D tx_current;


		public Direction LookingTo
		{
			get { return lookingTo; }
			set
			{
				Direction oldValue = lookingTo;
				lookingTo = value;
				if (IsInitialized && oldValue != lookingTo)
				{
					RefreshTexture();
					OnLookingToChangedAction?.Invoke(lookingTo);
				}
			}
		}
		public MouthState Month
		{
			get { return month; }
			set
			{
				MouthState oldValue = month;
				month = value;
				if (IsInitialized && oldValue != month)
				{
					RefreshTexture();
					OnMouthStateChangedAction?.Invoke(month);
				}
			}
		}

		public bool IsDying
		{
			get { return isDying; }
			set
			{
				bool oldValue = isDying;
				isDying = value;

				// If the value just changed to 'true', then update dieBegin variable
				if (!oldValue && isDying)
					if (Current != null)
						dieBegin = Current.TotalGameTime;

				if (IsInitialized && oldValue != isDying)
				{
					RefreshTexture();
					OnDieStateChangedAction?.Invoke(isDying);
				}
			}
		}

		public GameTime Current
		{
			get { return current; }
			private set { current = value; }
		}

		public OnLookingToChanged OnLookingToChangedAction { get; set; }
		public OnMouthStateChanged OnMouthStateChangedAction { get; set; }
		public OnTextureChanged OnTextureChangedAction { get; set; }
		public OnDieStateChanged OnDieStateChangedAction { get; set; }
		public Texture2D CurrentTexture
		{
			get { return tx_current; }
			private set
			{
				Texture2D oldTexture = tx_current;
				tx_current = value;
				if (IsInitialized && oldTexture != null && !oldTexture.Equals(tx_current))
					OnTextureChangedAction?.Invoke(tx_current);
			}
		}

		// Instance of PacRepresentation
		private static PacRepresentation instance = new PacRepresentation();

		public static PacRepresentation Instance
		{
			get { return instance; }
		}

		private PacRepresentation()
		{
			LookingTo = Direction.RIGHT;
			Month = MouthState.CLOSE;
			isDying = false;
			dieStep = 0;
			RefreshTexture();
		}

		public override void LoadContent(Game game)
		{
			tx_pac_rc = game.Content.Load<Texture2D>(@"Images\pacman_rc");
			tx_pac_ro = game.Content.Load<Texture2D>(@"Images\pacman_ro");
			tx_pac_tc = game.Content.Load<Texture2D>(@"Images\pacman_tc");
			tx_pac_to = game.Content.Load<Texture2D>(@"Images\pacman_to");
			tx_pac_bc = game.Content.Load<Texture2D>(@"Images\pacman_bc");
			tx_pac_bo = game.Content.Load<Texture2D>(@"Images\pacman_bo");
			tx_pac_lc = game.Content.Load<Texture2D>(@"Images\pacman_lc");
			tx_pac_lo = game.Content.Load<Texture2D>(@"Images\pacman_lo");

			tx_die0 = game.Content.Load<Texture2D>(@"Images\pacman_dead0");
			tx_die1 = game.Content.Load<Texture2D>(@"Images\pacman_dead1");
			tx_die2 = game.Content.Load<Texture2D>(@"Images\pacman_dead2");
			tx_die3 = game.Content.Load<Texture2D>(@"Images\pacman_dead3");
			tx_empty = game.Content.Load<Texture2D>(@"Images\transparent");

			IsInitialized = true;
		}

		public override void Update(GameTime gameTime)
		{
			Current = gameTime;

			// Updating texture
			if (gameTime.TotalGameTime.Seconds % 2 == 0)
				Month = MouthState.CLOSE;
			else
				Month = MouthState.OPEN;

			RefreshTexture();
		}

		public void RefreshTexture()
		{
			if (IsDying)
			{
				double now = Current.TotalGameTime.TotalMilliseconds;
				double then = dieBegin.TotalMilliseconds;
				double delta = now - then;

				if (delta < 0)
				{
					Console.WriteLine("GameTime updated!");
#if DEBUG
					Debugger.Break();
#endif
				}

				if (now >= then + dieStep * transitionLengthMilli)
				{
					switch (dieStep)
					{
						case 0:
							CurrentTexture = tx_die0;
							break;
						case 1:
							CurrentTexture = tx_die1;
							break;
						case 2:
							CurrentTexture = tx_die2;
							break;
						case 3:
							CurrentTexture = tx_die3;
							break;
						case 4:
							CurrentTexture = tx_empty;
							break;
						case 5:
							goto default;
						default:
							IsDying = false;
							dieStep = -1;
							break;
					}

					dieStep++;
				}
			}
			
			if (!IsDying)
			{
				switch (LookingTo)
				{
					case Direction.UP:
						switch (Month)
						{
							case MouthState.OPEN:
								CurrentTexture = tx_pac_to;
								break;
							case MouthState.CLOSE:
								CurrentTexture = tx_pac_tc;
								break;
						}
						break;
					case Direction.DOWN:
						switch (Month)
						{
							case MouthState.OPEN:
								CurrentTexture = tx_pac_bo;
								break;
							case MouthState.CLOSE:
								CurrentTexture = tx_pac_bc;
								break;
						}
						break;
					case Direction.LEFT:
						switch (Month)
						{
							case MouthState.OPEN:
								CurrentTexture = tx_pac_lo;
								break;
							case MouthState.CLOSE:
								CurrentTexture = tx_pac_lc;
								break;
						}
						break;
					case Direction.RIGHT:
						switch (Month)
						{
							case MouthState.OPEN:
								CurrentTexture = tx_pac_ro;
								break;
							case MouthState.CLOSE:
								CurrentTexture = tx_pac_rc;
								break;
						}
						break;
				}
			}
		}
	}
}
