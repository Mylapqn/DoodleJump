using DoodleJump.Core;
using DoodleJump.Hierarchy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Rendering
{
	internal class BackgroundLayer : GameObject
	{
		public float depth = 0;
		Vector2 initialPosition;
		public BackgroundLayer(SpriteSheet visualization, float depth, Vector2 initialPosition) : base(visualization)
		{
			this.Visualization.Scale = GameSettings.PIXEL_SCALE;
			this.Visualization.Origin = new Vector2(.5f, 1);
			this.initialPosition = initialPosition;
			this.depth = depth;
		}
		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			base.Draw(spriteBatch, polygonDrawer);
		}
		public void updateCameraPosition(Camera cam)
		{
			this.Position = initialPosition + cam.Position * depth;
		}
	}
}
