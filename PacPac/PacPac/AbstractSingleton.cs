using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac
{
	public abstract class AbstractSingleton
	{
		protected bool isInitialized;

		public bool IsInitialized
		{
			get { return isInitialized; }
			protected set { isInitialized = value; }
		}

		protected AbstractSingleton()
		{
			IsInitialized = false;
		}
	}
}
