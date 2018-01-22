using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Grid.Exceptions
{
	public class InvalidMazeDimensionException : Exception
	{
		public InvalidMazeDimensionException(int invalidX, int invalidY, int expectedX, int expectedY) :
			base("The dimension (" + invalidX + " ; " + invalidY + ") is not valid. The expected dimension is (" + expectedX + " ; " + expectedY + ").")
		{ }
	}
}
