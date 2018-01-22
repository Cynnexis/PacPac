using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PacPac.Core
{
	public abstract class LivingGear : Gear, LivingGearInterface
	{
		private Vector2 startingPoint;

		public Vector2 StartingPoint
		{
			get { return startingPoint; }
			set { startingPoint = value; }
		}

		public LivingGear(Game game) : base(game) { }

		public abstract void Die();
	}
}
