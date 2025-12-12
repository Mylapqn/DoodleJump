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
		public List<Platform> Platforms { get; private set; }
		public List<GameObject> GameObjects { get; private set; }
		private Player Player { get; set; }
		public Camera Camera;
		private float elapsedTime = 0f;
		private PlatformSpawner platformSpawner = new PlatformSpawner();

		public override void Initialize()
		{
			GameSettings.ActiveScreen = this;
			this.Camera = new Camera();
			this.GameObjects = new List<GameObject>();
			this.Platforms = new List<Platform>();

			Player = new Player();
			this.GameObjects.Add(Player);
			int currentX = 0;
			int currentY = 0;
			for (int i = 0; i < 100; i++)
			{
				
			}
			Player.Position = new Vector2(0, -150);
		}

		public override void LoadContent(ContentManager content)
		{
			Debug.WriteLine("LoadContent PlayScreen");
		}

		public override void Update(float dt)
		{
			elapsedTime += dt;
			Camera.Position += new Vector2(0, -200 * dt);

			List<Platform> addedPlatforms = platformSpawner.SpawnPlaforms(Camera.Position.Y - GameSettings.WindowHeight / 2);
			Platforms.AddRange(addedPlatforms);
			GameObjects.AddRange(addedPlatforms);

			List<Platform> removedPlatforms = platformSpawner.GetBottomPlatformsToRemove(Platforms, Camera.Position.Y + GameSettings.WindowHeight / 2);
			foreach (var platform in removedPlatforms)
			{
				Platforms.Remove(platform);
				GameObjects.Remove(platform);
			}


			Player.CheckPlatformCollisions(Platforms);
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
