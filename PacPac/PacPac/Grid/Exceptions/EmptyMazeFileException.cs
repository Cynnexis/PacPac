using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Grid.Exceptions
{
	public class EmptyMazeFileException : Exception
	{
		public EmptyMazeFileException(string path) : base("The file \"" + path + "\" is empty, or contains only comments.") { }
	}
}
