using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PacPac.Core
{
	/// <summary>
	/// Abstract class to describe a game component representation
	/// </summary>
	/// <seealso cref="PacRepresentation"/>
	public abstract class ComponentRepresentation : ComponentRepresentationInterface
	{

		private bool isInitialized = false;

		public bool IsInitialized
		{
			get { return isInitialized; }
			protected set { isInitialized = true; }
		}

		public abstract void LoadContent(Game game);
		public abstract void Update(GameTime gameTime);
	}
}
