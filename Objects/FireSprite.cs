using DoodleJump.Core;
using DoodleJump.Hierarchy;
using DoodleJump.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Objects
{
	internal class FireSprite : GameObject
	{
		public FireSprite():base(new SpriteSheet(Game1.Instance.Assets.Textures["textures/sprites/fire_circles_100x100"], 100, 100))
		{
		}
	}
}
