using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Grid.Exceptions
{
	public class BoundingBoxException : Exception
	{
		public BoundingBoxException() : base() { }
		public BoundingBoxException(string message) : base(message) { }
		public BoundingBoxException(string message, Exception innerException) : base(message, innerException) { }
	}
}
