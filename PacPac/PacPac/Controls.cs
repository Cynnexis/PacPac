using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac
{
	/// <summary>
	/// Class to describe the control of the game
	/// </summary>
	[Serializable]
	public class Controls
	{
		public const Keys PAC_UP = Keys.Z;
		public const Keys PAC_LEFT = Keys.Q;
		public const Keys PAC_DOWN = Keys.S;
		public const Keys PAC_RIGHT = Keys.D;

		public static bool CheckKeyState(Keys key)
		{
			KeyboardState keyboard = Keyboard.GetState();
			return keyboard.IsKeyDown(key);
		}
	}
}
