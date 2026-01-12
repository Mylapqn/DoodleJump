using DoodleJump.Core;
using DoodleJump.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Objects
{
	internal class EnemyPlatform : MovingPlatform
	{
		public EnemyPlatform(SpriteSheet visualization) : base(visualization)
		{
			this.moveSpeed *= 1.5f;
		}
		public override void Bounce(Player player)
		{
			player.Die();
		}
	}
}
