using DoodleJump.Hierarchy;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Objects
{
	internal class BouncePlatform : Platform
	{
		public BouncePlatform(SpriteSheet visualization) : base(visualization)
		{
			this.Visualization.Scale = 4;
			BounceForce = 60f;
		}
	}
}
