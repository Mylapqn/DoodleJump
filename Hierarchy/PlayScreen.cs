using DoodleJump.Core;
using DoodleJump.Objects;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
		private List<BackgroundLayer> backgroundLayers = new List<BackgroundLayer>();

		public override void Initialize()
		{
			GameSettings.ActiveScreen = this;
			this.Camera = new Camera();
			this.GameObjects = new List<GameObject>();
			this.Platforms = new List<Platform>();

			BackgroundLayer bg2 = new BackgroundLayer(new SpriteSheet(GameSettings.Assets.Textures["bg_city_2"]), 0.7f, new Vector2(0, -300));
			GameObjects.Add(bg2);
			backgroundLayers.Add(bg2);

			BackgroundLayer bg1 = new BackgroundLayer(new SpriteSheet(GameSettings.Assets.Textures["bg_city_1"]), 0.5f, new Vector2(0, 300));
			GameObjects.Add(bg1);
			backgroundLayers.Add(bg1);

			BackgroundLayer bg = new BackgroundLayer(new SpriteSheet(GameSettings.Assets.Textures["fg_city"]), 0.1f, new Vector2(0, 100));
			GameObjects.Add(bg);
			backgroundLayers.Add(bg);


			Player = new Player();
			this.GameObjects.Add(Player);
			int currentX = 0;
			int currentY = 0;
			for (int i = 0; i < 100; i++)
			{

			}
			Player.Position = new Vector2(0, -150);
			GameSettings.TimeScale = 1f;
		}

		public override void LoadContent(ContentManager content)
		{
			Debug.WriteLine("LoadContent PlayScreen");
		}

		public override void Update(float dt)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Input.IsKeyReleased(Keys.Escape))
				GameSettings.EndScreen.Initialize();

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

			foreach (var background in backgroundLayers)
			{
				background.updateCameraPosition(Camera);
			}


			Player.CheckPlatformCollisions(Platforms, dt);
			foreach (var obj in GameObjects)
			{
				if (obj.IsActive)
				{
					obj.Update(dt);
				}
			}
			GameSettings.TimeScale = 1f + MathF.Min(Player.maxHeight / 50000f, 3f);
			GameSettings.Score = (int)(Player.maxHeight / 100);
		}

		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			spriteBatch.GraphicsDevice.Clear(Color.LightGray);

			Matrix world = Matrix.Identity;
			Matrix view = Camera.GetViewMatrix(false);
			Matrix reflection = Matrix.CreateScale(new Vector3(1, -1, 1));
			Matrix projection = Matrix.CreateOrthographic(GameSettings.WindowWidth, GameSettings.WindowHeight, 0, -2f);
			Matrix worldViewProjection = world * view * reflection * projection;

			//Draw World
			spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Camera.GetViewMatrix(true));
			polygonDrawer.Begin(viewMatrix: worldViewProjection);

			Vector2 screen = new Vector2(GameSettings.WindowWidth, GameSettings.WindowHeight);
			int sideWidth = (GameSettings.WindowWidth - GameSettings.GameWidth) / 2;
			polygonDrawer.DrawRectangle(Camera.Position - screen / 2, new Vector2(sideWidth, screen.Y), Color.Black);
			polygonDrawer.DrawRectangle(Camera.Position + new Vector2(screen.X / 2 - sideWidth, -screen.Y / 2), new Vector2(sideWidth, screen.Y), Color.Black);
			polygonDrawer.DrawRectangle(new Vector2(-screen.X / 2, 0), screen, Color.Black);

			foreach (var obj in GameObjects)
			{
				if (obj.IsActive)
				{
					obj.Draw(spriteBatch, polygonDrawer);
				}
			}

			spriteBatch.End();
			polygonDrawer.End();


			//Draw UI
			spriteBatch.Begin();
			spriteBatch.DrawStringAdvanced(
				GameSettings.Assets.Fonts["default_font"],
				$"Score:",
				new Vector2(GameSettings.WindowWidth / 2, 150),
				Color.White, 0.5f, 0f, 0.5f);
			spriteBatch.DrawStringAdvanced(
				GameSettings.Assets.Fonts["default_font"],
				$"{GameSettings.Score}",
				new Vector2(GameSettings.WindowWidth / 2, 200),
				Color.White, 0.5f, 0f, 1f);

			spriteBatch.End();


		}
	}
}
