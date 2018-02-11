using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using PacPac.Grid.Exceptions;
using Microsoft.Xna.Framework.Graphics;
using PacPac.Core;

namespace PacPac.Grid
{
	/// <summary>
	/// Maze of the game
	/// </summary>
	public class Maze : Gear
	{
		#region Attributes & Properties
		private Cell[,] cells = new Cell[28, 32];
		private int width;
		private int height;
		public const int SPRITE_DIMENSION = 20;
		private string path = "";

		private TeleporterManager tm;

		private Texture2D tx_empty;
		private Texture2D tx_wall;
		private Texture2D tx_ghostWall;
		private Texture2D tx_ghostGate;
		private Texture2D tx_border;
		private Texture2D tx_pacdot;
		private Texture2D tx_fruit;

		/// <summary>
		/// Bidimensionnal array containing all the cells of the grid
		/// </summary>
		public Cell[,] Cells
		{
			get { return cells; }
		}

		/// <summary>
		/// Bidimensionnal array containing all the cells of the grid
		/// </summary>
		/// <param name="x">The column (starting from 0)</param>
		/// <param name="y">The row (starting from 0)</param>
		/// <returns>The cell at (<paramref name="x"/> ; <paramref name="y"/>)</returns>
		public Cell this[int x, int y]
		{
			get
			{
				return cells[x, y];
			}

			set
			{
				cells[x, y] = value;
			}
		}
		/// <summary>
		/// Bidimensionnal array containing all the cells of the grid
		/// </summary>
		/// <param name="x">The column (starting from 0)</param>
		/// <param name="y">The row (starting from 0)</param>
		/// <returns>The cell at (<paramref name="x"/> ; <paramref name="y"/>)</returns>
		public Cell this[float x, float y]
		{
			get { return this[(int) x, (int) y]; }
			set { this[(int) x, (int) y] = value; }
		}
		/// <summary>
		/// Bidimensionnal array containing all the cells of the grid
		/// </summary>
		/// <param name="indexes">The coordinates of the cell, such that <c>x</c> if the column and <c>y</c> the row (both starting from 0)</param>
		/// <returns>The cell at (<paramref name="indexes"/>.X ; <paramref name="y"/>.Y)</returns>
		public Cell this[Vector2 indexes]
		{
			get { return this[indexes.X, indexes.Y]; }
			set { this[indexes.X, indexes.Y] = value; }
		}

		/// <summary>
		/// Width of the maze
		/// </summary>
		public int Width
		{
			get { return width; }
			protected set { width = value; }
		}

		/// <summary>
		/// Height of the maze
		/// </summary>
		public int Height
		{
			get { return height; }
			protected set { height = value; }
		}

		/// <summary>
		/// Path to the file where the data of the maze is stored
		/// </summary>
		public string Path
		{
			get { return path; }
			set { path = value; }
		}

		/// <summary>
		/// Teleporter Manager
		/// </summary>
		public TeleporterManager TM
		{
			get { return tm; }
			set { tm = value; }
		}
		#endregion

		#region Constructor
		/// <summary>
		/// Default Constructor. Initialize the teleporter manager <see cref="TM"/>, and import the maze from file.
		/// </summary>
		/// <param name="game">The game instance, different from <c>null</c></param>
		/// <param name="path">The path of the file where the maze data is stored. If it is empty, a default file is chosen</param>
		/// <exception cref="ArgumentNullException">Throw if <paramref name="game"/> is null</exception>
		public Maze(Game game, string path = "") : base(game)
		{
			if (game == null)
				throw new ArgumentNullException();

			TM = new TeleporterManager();
			Width = 0;
			Height = 0;

			if (path.Equals(""))
				Path = "Resources/Maps/maze1.txt";
			else
				Path = path;

			Import(path);

			this.Game.Components.Add(this);
		}
		#endregion

		/// <summary>
		/// Import a maze from a file
		/// </summary>
		/// <param name="path">Path file</param>
		public void Import(string path)
		{
			Path = path;
			string rawContent = "";
			string content = "";
			using (var reader = new StreamReader(Path))
				rawContent = reader.ReadToEnd();

			if (rawContent != null && !rawContent.Equals(""))
			{
				for (int i = 0; i < rawContent.Length; i++)
				{
					char c = rawContent[i];

					// If 'c' represents the beginning of a comment-line, jump this line
					if (c == '#')
					{
						for (; c != '\n' && i < rawContent.Length; i++)
							c = rawContent[i];
						i--;
					}
					else
					{
						if (('0' <= c && c <= '9') || ('a' <= c && c <= 'z') || c == '\n')
							content += c;
					}
				}
			}

			// Check that all lines have the same length
			string[] lines = content.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.None);
			if (lines.Length > 0)
			{
				int length = lines[0].Length;
				if (length < 4)
					throw new InvalidMazeDimensionException(length, lines.Length, 28, 32);
				if (lines.Length < 4)
					throw new InvalidMazeDimensionException(length, lines.Length, 28, 32);

				int i = 1;
				foreach (string line in lines)
				{
					if (line.Length != length)
						throw new InvalidLineLengthException(length, line.Length, i);
					i++;
				}
			}
			else
				throw new EmptyMazeFileException(path);

			// Convert string to Cell[,]
			cells = new Cell[lines[0].Length, lines.Length];
			for (int i = 0; i < lines.Length; i++)
			{
				for (int j = 0; j < lines[i].Length; j++)
				{
					char c = lines[i][j];

					// If it is a teleporter
					if ('a' <= c && c <= 'z')
					{
						// Search if a such teleporter already exist:
						Teleporter t = TM.Search(c);
						if (t != null)
						{
							if (!t.Position2.Equals(new Vector2(-1, -1)))
								throw new TeleporterException("The teleporter \'" + t.Name + "\' already has a destination. How can a teleporter have 2 destinations? Poor pac...");
							t.Position2 = new Vector2(j, i);
						}
						else
							TM.Add(new Teleporter(c, new Vector2(j, i)));
						c = (char) ('0' + ((int) TileType.TELEPORTER));
					}

					cells[j, i] = Create(new Vector2(j * SPRITE_DIMENSION, i * SPRITE_DIMENSION), c - '0');
				}
			}

			// Check if there are 3 ghost_startup
			int nbGhostStartup = SearchTile(TileType.GHOSTS_STARTUP).Count;
			if (nbGhostStartup < 3)
				throw new EmptyMazeFileException("Invalid number of ghosts startup");

			// Finally, check if all teleporters are valid
			if (!TM.CheckTeleporters())
				throw new TeleporterException("One teleporter is not valid in the maze.");

			Position = new Vector2(0, 0);
			Width = cells.GetLength(0);
			Height = cells.GetLength(1);
			Size = new Vector2(Width * SPRITE_DIMENSION, Height * SPRITE_DIMENSION);

#if DEBUG
			Console.WriteLine("cells : [" + Width + " ; " + Height + "]");
#endif
		}

		/// <summary>
		/// Create an instance of Cell
		/// </summary>
		/// <param name="position"></param>
		/// <param name="tile"></param>
		/// <returns></returns>
		private Cell Create(Vector2 position, TileType tile)
		{
			return new Cell(Game,
				new BoundingBox(new Vector3(position.X, position.Y, 0),
				new Vector3(position.X + SPRITE_DIMENSION, position.Y + SPRITE_DIMENSION, 0)),
				tile);
		}
		/// <summary>
		/// Create an instance of Cell
		/// </summary>
		/// <param name="position"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		private Cell Create(Vector2 position, int i)
		{
			return Create(position, (TileType) Enum.Parse(typeof(TileType), i.ToString()));
		}

		/// <summary>
		/// Search all the cell where theire tile type is <paramref name="tile"/>.
		/// </summary>
		/// <param name="tile">The type to search</param>
		/// <returns>The list of all corresponding cells, different from <c>null</c></returns>
		public List<Cell> SearchTile(TileType tile)
		{
			List<Cell> list = new List<Cell>();

			for (int i = 0, maxi = cells.GetLength(0); i < maxi; i++)
				for (int j = 0, maxj = cells.GetLength(1); j < maxj; j++)
					if (cells[i, j] != null && cells[i, j].Tile == tile)
						list.Add(cells[i, j]);

			return list;
		}

		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		/// Load Contents from resources
		/// </summary>
		protected override void LoadContent()
		{
			tx_empty = Game.Content.Load<Texture2D>(@"Images\empty_black");
			tx_wall = Game.Content.Load<Texture2D>(@"Images\wall_blue");
			tx_ghostWall = Game.Content.Load<Texture2D>(@"Images\wall_blue");
			tx_ghostGate = Game.Content.Load<Texture2D>(@"Images\ghost_gate");
			tx_border = Game.Content.Load<Texture2D>(@"Images\wall_blue");
			tx_pacdot = Game.Content.Load<Texture2D>(@"Images\pacdot-m");
			tx_fruit = Game.Content.Load<Texture2D>(@"Images\power");

			base.LoadContent();
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (State == GameState.Playing)
			{
				int nbSpriteWidth = cells.GetLength(0);
				int nbSpriteHeight = cells.GetLength(1);

				Sprite.Begin();
				// Draw all cells
				for (int i = 0; i < nbSpriteWidth; i++)
				{
					for (int j = 0; j < nbSpriteHeight; j++)
					{
						Sprite.Draw(GetTileTexture(this[i, j].Tile),
										new Vector2(i * SPRITE_DIMENSION, j * SPRITE_DIMENSION),
										Color.Azure);
					}
				}
				Sprite.End();
			}

			base.Draw(gameTime);
		}

		/// <summary>
		/// Get the corresponding texture according to <paramref name="tile"/> value.
		/// </summary>
		/// <param name="tile">The tile type to convert into a texture</param>
		/// <returns>The corresponding texture</returns>
		public Texture2D GetTileTexture(TileType tile)
		{
			switch (tile)
			{
				case TileType.EMPTY:
					return tx_empty;
				case TileType.WALL:
					return tx_wall;
				case TileType.GHOST_WALL:
					return tx_ghostWall;
				case TileType.GHOST_GATE:
					return tx_ghostGate;
				case TileType.BORDER:
					return tx_border;
				case TileType.PACDOT:
					return tx_pacdot;
				case TileType.FRUIT:
					return tx_fruit;
				case TileType.PAC_STARTUP:
					return tx_empty;
				case TileType.BLINKY_STARTUP:
					return tx_empty;
				case TileType.GHOSTS_STARTUP:
					return tx_empty;
				case TileType.TELEPORTER:
					return tx_empty;
				default:
					return tx_empty;
			}
		}
	}
}
