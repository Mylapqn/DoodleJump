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
	internal class Platform : GameObject
	{
		public float BounceForce = 15f;
		public Platform(SpriteSheet visualization) : base(visualization)
		{
			this.Visualization.Scale = 4;
		}
	}
}
