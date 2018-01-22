using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core.Exceptions
{
	public class InfiniteLoopException : Exception
	{
		public InfiniteLoopException() : base() { }
		public InfiniteLoopException(int nbIterations) : base("An infinite loop has been detected with " + nbIterations + " iterations.") { }
	}
}
