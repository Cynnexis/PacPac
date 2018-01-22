using Microsoft.Xna.Framework;
using PacPac.Grid.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core
{
	public class TeleporterManager
	{
		private List<Teleporter> teleporters;

		public List<Teleporter> Teleporters
		{
			get { return teleporters; }
			set { teleporters = value; }
		}

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

		public TeleporterManager()
		{
			Teleporters = new List<Teleporter>();
		}

		public bool CheckTeleporters()
		{
			foreach (Teleporter t in Teleporters)
				if (t.Position1.X < 0 || t.Position1.Y < 0 || t.Position2.X < 0 || t.Position2.Y < 0)
					return false;

			return true;
		}

		public void Add(Teleporter teleporter)
		{
			foreach (Teleporter t in Teleporters)
				if (t.Name == teleporter.Name)
					throw new TeleporterException("A teleporter with the name \'" + t.Name + "\' already exists.");

			Teleporters.Add(teleporter);
		}

		public bool Teleport(Pac pac)
		{
			bool result = false;

			for (int i = 0, maxi = Teleporters.Count; i < maxi && !result; i++)
				result = Teleporters[i].Teleport(pac);

			return result;
		}

		public Teleporter Search(char name)
		{
			foreach (Teleporter t in Teleporters)
				if (t.Name == name)
					return t;

			return null;
		}
	}
}
