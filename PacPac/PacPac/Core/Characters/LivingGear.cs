using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PacPac.Core
{
	/// <summary>
	/// This class is a gear adapted for movable and living object in the game.
	/// </summary>
	/// <seealso cref="Gear"/>
	/// <seealso cref="LivingGearInterface"/>
	public abstract class LivingGear : Gear, LivingGearInterface
	{
		private Vector2 startingPoint;

		/// <summary>
		/// Starting point of the gear
		/// </summary>
		public Vector2 StartingPoint
		{
			get { return startingPoint; }
			set { startingPoint = value; }
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		/// <param name="game"></param>
		public LivingGear(Game game) : base(game) { }

		/// <summary>
		/// Die method
		/// </summary>
		public abstract void Die();
	}
}
