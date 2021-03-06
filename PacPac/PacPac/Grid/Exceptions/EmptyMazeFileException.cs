﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Grid.Exceptions
{
	/// <summary>
	/// Thrown when <see cref="Maze.Import(string)"/> tried to import a maze
	/// from a file, but the file is empty or is invalid.
	/// </summary>
	public class EmptyMazeFileException : Exception
	{
		public EmptyMazeFileException(string path) : base("The file \"" + path + "\" is empty, or contains only comments.") { }
	}
}
