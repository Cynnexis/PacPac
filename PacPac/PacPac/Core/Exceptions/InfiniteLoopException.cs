using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Exceptions
{
	/// <summary>
	/// Exception thrown when an Infinite loop is detected
	/// </summary>
	public class InfiniteLoopException : Exception
	{
		public InfiniteLoopException() : base() { }
		public InfiniteLoopException(int nbIterations) : base("An infinite loop has been detected with " + nbIterations + " iterations.") { }
	}
}
