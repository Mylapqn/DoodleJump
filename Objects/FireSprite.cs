using DoodleJump.Core;
using DoodleJump.Hierarchy;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Objects
{
	internal class FireSprite : GameObject
	{
		public FireSprite(PlayScreen screen):base(new SpriteSheetAnimation(Game1.Instance.Assets.Textures["fire_circles_100x100"], 8,8,0,60,0), screen)
		{
			(this.Visualization as SpriteSheetAnimation).AnimationDelayInFrames = 1;
		}
        public override void Draw(SpriteBatch spriteBatch)
        {
			Game1.Instance.polygonDrawer.DrawCircle(TopLeftPosition, 30, Color.Red, 2);
            base.Draw(spriteBatch);
		}
	}
}
