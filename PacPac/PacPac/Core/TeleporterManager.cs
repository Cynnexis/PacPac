using Microsoft.Xna.Framework;
using PacPac.Grid.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core
{
	/// <summary>
	/// Manage a collection of teleporters
	/// </summary>
	/// <seealso cref="Teleporter"/>
	public class TeleporterManager
	{
		private List<Teleporter> teleporters;

		/// <summary>
		/// The list of teleporters to manage
		/// </summary>
		public List<Teleporter> Teleporters
		{
			get { return teleporters; }
			set { teleporters = value; }
		}

		/// <summary>
		/// The indexer of the list of teleporters
		/// </summary>
		/// <param name="i">Index</param>
		/// <returns>The teleporter at the index <paramref name="i"/></returns>
		public Teleporter this[int i]
		{
			get
			{
				return Teleporters[i];
			}
			set
			{
				Teleporters[i] = value;
			}
		}

		/// <summary>
		/// Default constructor. Initialize an empty list of teleporter
		/// </summary>
		public TeleporterManager()
		{
			Teleporters = new List<Teleporter>();
		}

		/// <summary>
		/// Check if all teleporters are valid.
		/// </summary>
		/// <returns>Return <c>true</c> if all teleporters are valid,
		/// <c>false</c> if one or more are invalid</returns>
		public bool CheckTeleporters()
		{
			foreach (Teleporter t in Teleporters)
				if (t.Position1.X < 0 || t.Position1.Y < 0 || t.Position2.X < 0 || t.Position2.Y < 0)
					return false;

			return true;
		}

		/// <summary>
		/// Add a teleporter to the list
		/// </summary>
		/// <param name="teleporter">The teleporter to add</param>
		/// <exception cref="ArgumentNullException">Thrown if
		/// <paramref name="teleporter"/> is null</exception>
		/// <exception cref="TeleporterException">Thrown when another teleporter
		/// in the list has already the same name as
		/// <paramref name="teleporter"/></exception>
		public void Add(Teleporter teleporter)
		{
			if (teleporter == null)
				throw new ArgumentNullException();

			foreach (Teleporter t in Teleporters)
				if (t.Name == teleporter.Name)
					throw new TeleporterException("A teleporter with the name \'" + t.Name + "\' already exists.");

			Teleporters.Add(teleporter);
		}

		/// <summary>
		/// Use on the teleporter to teleport pac
		/// </summary>
		/// <param name="pac">The pac to teleport</param>
		/// <returns>Return <c>true</c> if one of the teleporter teleport pac.
		/// </returns>
		/// <seealso cref="Teleporter.Teleport(Pac)"/>
		public bool Teleport(Pac pac)
		{
			bool result = false;

			for (int i = 0, maxi = Teleporters.Count; i < maxi && !result; i++)
				result = Teleporters[i].Teleport(pac);

			return result;
		}

		/// <summary>
		/// Search a teleporter in the list with the name
		/// <paramref name="name"/>.
		/// </summary>
		/// <param name="name">The name if the teleporter to search for</param>
		/// <returns>Return the teleporter found in the list, or <c>null</c>.
		/// </returns>
		public Teleporter Search(char name)
		{
			foreach (Teleporter t in Teleporters)
				if (t.Name == name)
					return t;

			return null;
		}
	}
}
