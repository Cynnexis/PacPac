using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacPac.Core.Characters.GhostCharacters;

namespace PacPac
{
	/// <summary>
	/// Abstract calss to help to implement the Singleton design pattern
	/// </summary>
	/// <seealso cref="GhostManager"/>
	/// <seealso cref="SoundManager"/>
	/// <seealso cref="FontManager"/>
	public abstract class AbstractSingleton
	{
		protected bool isInitialized;

		/// <summary>
		/// Is the instance initialized?
		/// </summary>
		public bool IsInitialized
		{
			get { return isInitialized; }
			protected set { isInitialized = value; }
		}

		/// <summary>
		/// Protected default constructor
		/// </summary>
		protected AbstractSingleton()
		{
			IsInitialized = false;
		}
	}
}
