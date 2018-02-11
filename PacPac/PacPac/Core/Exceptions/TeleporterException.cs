using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Grid.Exceptions
{
	/// <summary>
	/// Exception thrown when a teleporter is invalid
	/// </summary>
	public class TeleporterException : Exception
	{
		public TeleporterException() : base() { }
		public TeleporterException(string message) : base(message) { }
		public TeleporterException(string message, Exception innerException) : base(message, innerException) { }
	}
}
