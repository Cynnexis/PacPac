using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Grid.Exceptions
{
	public class InvalidLineLengthException : Exception
	{
		public InvalidLineLengthException(int defaultLength, int invalidFoundLength, int invalidLineNumber) : base("The first line of the maze text was " + defaultLength + ", but the program founds at line " + invalidLineNumber + " a length of " + invalidFoundLength + ". All lines must have the same length.") { }
	}
}
