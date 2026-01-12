using DoodleJump.Core;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using System;

namespace DoodleJump.Objects
{
	internal class MovingPlatform : Platform
	{
		protected float moveSpeed = 250f;
		protected float edgePadding = 100f;
		public MovingPlatform(SpriteSheet visualization) : base(visualization)
		{
		}
		public override void Update(float dt)
		{
			Vector2 pos = Position;
			pos.X += moveSpeed * dt;
			if (pos.X > GameSettings.GameWidth / 2 - edgePadding)
			{
				moveSpeed = -MathF.Abs(moveSpeed);
			}
			if (pos.X < -GameSettings.GameWidth / 2 + edgePadding)
			{
				moveSpeed = MathF.Abs(moveSpeed);
			}
			Position = pos;
			base.Update(dt);
		}
	}
}
