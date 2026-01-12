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
	internal class PlatformBomb : GameObject
	{
		public PlatformBomb(Vector2 position, SpriteSheet visualization) : base(visualization)
		{
			this.Position = position;
		}

		//Called by PlayScreen
		public Platform CheckPlatformCollisions(IEnumerable<Platform> platforms)
		{
			foreach (var platform in platforms)
			{
				if (IsCollidingWith(platform))
				{
					return platform;
				}
			}
			return null;
		}
	}
}
