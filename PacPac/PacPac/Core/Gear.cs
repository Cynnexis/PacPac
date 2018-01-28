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

	public abstract class Gear : Microsoft.Xna.Framework.DrawableGameComponent
	{
		#region Variables & Properties
		private SpriteBatch sprite = null;
		private Texture2D texture = null;
		private BoundingBox dimension = new BoundingBox();
		private Vector2 position = new Vector2(0);
		private Vector2 size = new Vector2(0);
		private Vector2 speed = new Vector2(0);

		public SpriteBatch Sprite { get { return sprite; } set { sprite = value; } }
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
		public BoundingBox Dimension { get { return dimension; } set { dimension = value; } }
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

		public Gear(Game game) : base(game) {
			sprite = new SpriteBatch(game.GraphicsDevice);
		}

		public Vector2 ConvertPositionToTileIndexes(Vector2 position)
		{
			return new Vector2(
				(float)Math.Round((double) (position.X / ((float) Maze.SPRITE_DIMENSION))),
				(float)Math.Round((double) (position.Y / ((float) Maze.SPRITE_DIMENSION)))
			);
		}
		public Vector2 ConvertPositionToTileIndexes()
		{
			return ConvertPositionToTileIndexes(this.Position);
		}

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
