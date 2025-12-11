using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Hierarchy
{
	public class PlayScreen : Screen
	{
		public List<GameObject> GameObjects { get; private set; } = new List<GameObject>();
		public override void Draw(SpriteBatch spriteBatch)
		{
			foreach (var obj in GameObjects)
			{
				if (obj.IsActive)
				{
					obj.Draw(spriteBatch);
				}
			}
		}

		public override void Initialize()
		{
			throw new NotImplementedException();
		}

		public override void LoadContent(ContentManager content)
		{
			throw new NotImplementedException();
		}

		public override void Update(float dt)
		{
			foreach (var obj in GameObjects)
			{
				if (obj.IsActive)
				{
					obj.Update(dt);
				}
			}
		}
	}
}
