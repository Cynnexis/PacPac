using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacPac.Core
{
	public interface ComponentRepresentationInterface
	{
		void LoadContent(Game game);
		void Update(GameTime gameTime);
	}
}
