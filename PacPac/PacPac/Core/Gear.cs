using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacPac.Grid;

namespace PacPac.Core
{
	public delegate void OnTextureChanged(Texture2D texture);
	public delegate void OnDimensionChanged(BoundingBox dimension);
	public delegate void OnPositionChanged(Vector2 position);
	public delegate void OnSizeChanged(Vector2 size);
	public delegate void OnSpeedChanged(Vector2 speed);

	/// <summary>
	/// Gear is a <see cref="DrawableGameComponent"/> with extra attributes to
	/// describes an object in the game.
	/// </summary>
	public abstract class Gear : DrawableGameComponent
	{
		#region Variables & Properties
		private SpriteBatch sprite = null;
		private Texture2D texture = null;
		private BoundingBox dimension = new BoundingBox();
		private Vector2 position = new Vector2(0);
		private Vector2 size = new Vector2(0);
		private Vector2 speed = new Vector2(0);

		/// <summary>
		/// The sprite of the component
		/// </summary>
		public SpriteBatch Sprite { get { return sprite; } set { sprite = value; } }

		/// <summary>
		/// The texture of the component
		/// </summary>
		public Texture2D Texture
		{
			get { return texture; }
			set
			{
				Texture2D oldTexture = texture;
				texture = value;
				if (texture != null)
					Size = new Vector2(texture.Width, texture.Height);

				if (!Equals(oldTexture, texture))
					OnTextureChangedAction?.Invoke(texture);
			}
		}

		/// <summary>
		/// The dimension of the component
		/// </summary>
		/// <seealso cref="Size"/>
		public BoundingBox Dimension { get { return dimension; } set { dimension = value; } }

		/// <summary>
		/// The current position of the component
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
			set
			{
				Vector2 oldPosition = position;
				position = value;
				if (position != null)
					UpdateBoundingBox();

				if (!Equals(oldPosition, position))
					OnPositionChangedAction?.Invoke(position);
			}
		}

		/// <summary>
		/// The size of the component
		/// </summary>
		/// <seealso cref="Dimension"/>
		public Vector2 Size
		{
			get { return size; }
			set
			{
				Vector2 oldSize = size;
				size = value;
				if (size != null)
					UpdateBoundingBox();

				if (!Equals(oldSize, size))
					OnSizeChangedAction?.Invoke(size);
			}
		}

		/// <summary>
		/// The speed of the component
		/// </summary>
		public Vector2 Speed
		{
			get { return speed; }
			set
			{
				Vector2 oldSpeed = speed;
				speed = value;

				if (!Equals(oldSpeed, speed))
					OnSpeedChangedAction?.Invoke(speed);
			}
		}

		/// <summary>
		/// Fetch the current game state
		/// </summary>
		public GameState State
		{
			get
			{
				GameState state = GameState.Playing;
				try
				{
					state = ((Engine) Game).State;
				}
				catch (InvalidCastException ex)
				{
					Console.Error.WriteLine(ex.StackTrace);
				}

				return state;
			}
		}
		#endregion

		#region Action Delegates
		public OnTextureChanged OnTextureChangedAction { get; set; }
		public OnDimensionChanged OnDimensionChangedAction { get; set; }
		public OnPositionChanged OnPositionChangedAction { get; set; }
		public OnSizeChanged OnSizeChangedAction { get; set; }
		public OnSpeedChanged OnSpeedChangedAction { get; set; }
		#endregion

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="game">The game instance</param>
		public Gear(Game game) : base(game) {
			sprite = new SpriteBatch(game.GraphicsDevice);
		}

		/// <summary>
		/// Convert <paramref name="position"/> to tile indexes (for
		/// <see cref="Maze"/>)
		/// </summary>
		/// <param name="position">Position in pixel-base to convert into tile-indexes-base</param>
		/// <returns>Return <paramref name="position"/> in tile indexes</returns>
		public Vector2 ConvertPositionToTileIndexes(Vector2 position)
		{
			return new Vector2(
				(float)Math.Round((double) (position.X / ((float) Maze.SPRITE_DIMENSION))),
				(float)Math.Round((double) (position.Y / ((float) Maze.SPRITE_DIMENSION)))
			);
		}
		/// <summary>
		/// Convert the current <see cref="Position"/> of the component to
		/// tile indexes (for <see cref="Maze"/>)
		/// </summary>
		/// <returns>Return the position in tile indexes</returns>
		public Vector2 ConvertPositionToTileIndexes()
		{
			return ConvertPositionToTileIndexes(this.Position);
		}

		/// <summary>
		/// Update the dimension of the component according to
		/// <see cref="Position"/> & <see cref="Size"/>.
		/// </summary>
		public void UpdateBoundingBox()
		{
			Dimension = new BoundingBox(new Vector3(Position.X, Position.Y, 0),
				new Vector3(Position.X + Size.Y, Position.Y + Size.Y, 0));
		}
		
		public override void Update(GameTime gameTime)
		{
			UpdateBoundingBox();
			base.Update(gameTime);
		}
	}
}
