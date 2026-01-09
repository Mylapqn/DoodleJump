using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Rendering
{
	public class SpriteSheetAnimation : SpriteSheet
	{
		public int MinSpriteIndex { get; set; }
		public int MaxSpriteIndex { get; set; }
		public int IdleSpriteIndex { get; set; }

		public bool IsAnimationStopped { get; set; }

		int frameCounter = 0;
		float frameFloat = 0;
		public float AnimationFPS { get; set; } = -12f;

		public SpriteSheetAnimation(
			Texture2D texture,
			int rows,
			int columns,
			int minIndex,
			int maxIndex,
			int idleIndex)
			: base(texture, rows, columns)
		{
			MinSpriteIndex = minIndex;
			MaxSpriteIndex = maxIndex;
			IdleSpriteIndex = idleIndex;

			SpriteIndex = IdleSpriteIndex;
		}

		public override void Update(float dt)
		{

			if (IsAnimationStopped)
			{
				SpriteIndex = IdleSpriteIndex;
				return;
			}

			frameFloat += AnimationFPS * dt;

			if (frameFloat >= 1)
			{
				frameFloat = 0;

				SpriteIndex++;

				if (SpriteIndex > MaxSpriteIndex)
					SpriteIndex = MinSpriteIndex;
			}
			if (frameFloat <= -1)
			{
				frameFloat = 0;

				SpriteIndex--;

				if (SpriteIndex < 0)
					SpriteIndex = MaxSpriteIndex;
			}

			base.Update(dt);
		}
	}
}
