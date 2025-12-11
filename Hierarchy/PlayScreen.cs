using DoodleJump.Core;
using DoodleJump.Objects;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Hierarchy
{
	public class PlayScreen : Screen
	{
		public List<GameObject> GameObjects { get; private set; }
		public Camera Camera;

		public override void Initialize()
		{
			GameSettings.ActiveScreen = this;
			this.Camera = new Camera();
			this.GameObjects = new List<GameObject>();

			Player player = new Player();
			this.GameObjects.Add(player);
			int currentX = 0;
			int currentY = 0;
			for (int i = 0; i < 100; i++)
			{
				bool bounce = GameSettings.Random.NextDouble() < 0.5;
				Platform platform1 = new Platform(new SpriteSheet(GameSettings.Assets.Textures["platform"]));
				this.GameObjects.Add(platform1);
				if (currentX < -GameSettings.WindowWidth / 2 + 100)
				{
					currentX = -GameSettings.WindowWidth / 2 + 100;
				}
				if (currentX > GameSettings.WindowWidth / 2 - 100)
				{
					currentX = GameSettings.WindowWidth / 2 - 100;
				}
				platform1.Position = new Vector2(currentX, currentY);

				int maxOffset = 350;
				int offset = GameSettings.Random.Next(maxOffset * 2) - maxOffset;
				currentY -= 300;
				currentX += offset;
			}
			player.Position = new Vector2(0, -150);
		}

		public override void LoadContent(ContentManager content)
		{
			Debug.WriteLine("LoadContent PlayScreen");
		}

		public override void Update(float dt)
		{
			Camera.Position += new Vector2(0, -200 * dt);
			foreach (var obj in GameObjects)
			{
				if (obj.IsActive)
				{
					obj.Update(dt);
				}
			}
		}

		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			Debug.WriteLine("draw");

			Matrix world = Matrix.Identity;
			Matrix view = Camera.GetViewMatrix(false);
			Matrix reflection = Matrix.CreateScale(new Vector3(1, -1, 1));
			Matrix projection = Matrix.CreateOrthographic(GameSettings.WindowWidth, GameSettings.WindowHeight, 0, -2f);
			Matrix worldViewProjection = world * view * reflection * projection;


			spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.GetViewMatrix(true));
			polygonDrawer.Begin(viewMatrix: worldViewProjection);

			foreach (var obj in GameObjects)
			{
				if (obj.IsActive)
				{
					obj.Draw(spriteBatch, polygonDrawer);
				}
			}

			spriteBatch.End();
			polygonDrawer.End();


		}
	}
}
