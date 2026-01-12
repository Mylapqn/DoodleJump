using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Hierarchy
{
	internal abstract class Screen
	{
		protected Screen() { }

		public abstract void Initialize();
		public abstract void Deinitialize();

		public abstract void LoadContent(ContentManager content);

		public abstract void Update(float dt);

		public abstract void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer);
	}
}
