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
		private PlatformSpawner platformSpawner = new PlatformSpawner();
		private List<BackgroundLayer> backgroundLayers = new List<BackgroundLayer>();
		private float lastMaxHeight = 0f;
		private float ScoreMultiplier => MathF.Min(4, 1 + (float)GameSettings.PlatformStreak / 20f);
		private bool slowMode = false;
		private TextPopupManager PopupManager = new TextPopupManager();
		private List<HighScore> highScores;


		public override void Initialize()
		{
			GameSettings.ActiveScreen = this;
			this.Camera = new Camera();
			this.GameObjects = new List<GameObject>();
			this.Platforms = new List<Platform>();
			highScores = GameSettings.SaveSystem.LoadHighScores();

			BackgroundLayer sky = new BackgroundLayer(new SpriteSheet(GameSettings.Assets.Textures["bg_sky"]), 1f, new Vector2(0, 800));
			GameObjects.Add(sky);
			backgroundLayers.Add(sky);

			BackgroundLayer bg2 = new BackgroundLayer(new SpriteSheet(GameSettings.Assets.Textures["bg_city_2"]), 0.7f, new Vector2(0, -300));
			GameObjects.Add(bg2);
			backgroundLayers.Add(bg2);

			BackgroundLayer bg1 = new BackgroundLayer(new SpriteSheet(GameSettings.Assets.Textures["bg_city_1"]), 0.5f, new Vector2(0, 300));
			GameObjects.Add(bg1);
			backgroundLayers.Add(bg1);

			BackgroundLayer bg = new BackgroundLayer(new SpriteSheet(GameSettings.Assets.Textures["fg_city"]), 0.1f, new Vector2(0, 100));
			GameObjects.Add(bg);
			backgroundLayers.Add(bg);



			Player = new Player(PopupManager);
			this.GameObjects.Add(Player);
			GameSettings.TimeScale = 1f;

			Platform initialPlatform = new Platform(new SpriteSheet(GameSettings.Assets.Textures["platform"])) { };
			initialPlatform.Position = new Vector2(0, initialPlatform.Visualization.Size.Y / 2);
			initialPlatform.Visualization.Size = new Point(GameSettings.GameWidth, initialPlatform.Visualization.Size.Y);
			Platforms.Add(initialPlatform);
			GameObjects.Add(initialPlatform);
		}

		public override void LoadContent(ContentManager content)
		{
			Debug.WriteLine("LoadContent PlayScreen");
		}

		public override void Update(float dt)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Input.IsKeyReleased(Keys.Escape))
				GameSettings.EndScreen.Initialize();

			if (!Player.walking)
			{
				GameSettings.ElapsedGameTime += dt;
				Camera.Position += new Vector2(0, -200 * dt);
			}
			Camera.Zoom = MathF.Max(1f, 1.1f - GameSettings.ElapsedGameTime / 16f);

			List<Platform> addedPlatforms = platformSpawner.SpawnPlatforms(Camera.Position.Y - GameSettings.WindowHeight / 2);
			Platforms.AddRange(addedPlatforms);
			GameObjects.AddRange(addedPlatforms);

			List<Platform> removedPlatforms = platformSpawner.GetBottomPlatformsToRemove(Platforms, Camera.Position.Y + GameSettings.WindowHeight / 2 / Camera.Zoom);
			foreach (var platform in removedPlatforms)
			{
				Platforms.Remove(platform);
				GameObjects.Remove(platform);
			}


			Player.CheckPlatformCollisions(Platforms, dt);
			foreach (var obj in GameObjects)
			{
				if (obj.IsActive)
				{
					obj.Update(dt);
				}
			}
			PopupManager.Update(dt);
			if (Input.IsKeyReleased(Keys.F))
			{
				slowMode = !slowMode;
			}

			GameSettings.TimeScale = 1f + MathF.Min(GameSettings.MaxHeightReached / 100000f, 3f);
			if (slowMode)
			{
				GameSettings.TimeScale *= 0.5f;
			}
			float addedScore = (GameSettings.MaxHeightReached - lastMaxHeight) / 100 * ScoreMultiplier;
			GameSettings.ScoreDecimal += addedScore;
			lastMaxHeight = GameSettings.MaxHeightReached;
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

			DrawObjects(spriteBatch, polygonDrawer);
			DrawHighScoresInWorld(spriteBatch, polygonDrawer);

			if (Camera.CanSeePoint(new Vector2(0, -100)))
			{
				spriteBatch.Draw(GameSettings.Assets.Textures["pixel"], new Rectangle(-GameSettings.WindowWidth, -1, GameSettings.WindowWidth * 2, GameSettings.WindowHeight), new Rectangle(0, 0, 1, 1), ColorUtilities.ColorFromHex(0x101010FF));
				DrawTutorial(spriteBatch);
			}

			PopupManager.Draw(spriteBatch, polygonDrawer);

			spriteBatch.End();
			polygonDrawer.End();


			//Draw UI
			spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
			DrawUI(spriteBatch);
			spriteBatch.End();
		}

		private void DrawObjects(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			foreach (var background in backgroundLayers)
			{
				background.updateCameraPosition(Camera);
			}

			foreach (var obj in GameObjects)
			{
				if (obj.IsActive)
				{
					obj.Draw(spriteBatch, polygonDrawer);
				}
			}
		}

		private void DrawHighScoresInWorld(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			float textScale = 0.3f;
			for (int i = 0; i < Math.Min(10, highScores.Count); i++)
			{
				float padding = 100;
				var hs = highScores[i];
				float width = spriteBatch.DrawStringAdvanced(
					font: GameSettings.Assets.Fonts["default_font"],
					text: $"{i + 1}. {hs.PlayerName} - {hs.Score}",
					position: new Vector2(0, -hs.ReachedHeight),
					color: ColorUtilities.Premultiply(Color.White, 0.5f),
					alignHorizontal: 0.5f,
					alignVertical: 0f,
					scale: textScale).X;
				polygonDrawer.DrawLine(
					start: new Vector2(-GameSettings.WindowWidth, -hs.ReachedHeight),
					end: new Vector2(-width / 2 - padding, -hs.ReachedHeight),
					color: ColorUtilities.Premultiply(Color.White, 0.5f),
					width: 2f
					);
				polygonDrawer.DrawLine(
					start: new Vector2(GameSettings.WindowWidth, -hs.ReachedHeight),
					end: new Vector2(width / 2 + padding, -hs.ReachedHeight),
					color: ColorUtilities.Premultiply(Color.White, 0.5f),
					width: 2f
					);
			}
		}

		private void DrawUI(SpriteBatch spriteBatch)
		{
			float uiOpacity = MathF.Min(1, GameSettings.ElapsedGameTime / 2f);
			Color mainColor = ColorUtilities.Premultiply(Color.White, uiOpacity);
			Color mutedColor = ColorUtilities.Premultiply(Color.White, uiOpacity * .5f);
			Color streakColor = ColorUtilities.Premultiply(ColorUtilities.StreakColor(GameSettings.PlatformStreak), uiOpacity);


			spriteBatch.Draw(GameSettings.Assets.Textures["vignette"], new Rectangle(0, 0, GameSettings.WindowWidth, GameSettings.WindowHeight), Color.White);

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"SCORE",
				position: new Vector2(100, 150),
				color: mainColor,
				alignHorizontal: 0f,
				alignVertical: 0f,
				scale: 0.3f);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"{(int)GameSettings.ScoreDecimal}",
				position: new Vector2(100, 230),
				color: mainColor,
				alignHorizontal: 0f,
				alignVertical: 0.6f,
				scale: 1f);

			TimeSpan time = TimeSpan.FromSeconds(GameSettings.ElapsedGameTime);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"{time.Minutes}:{time.Seconds}.{time.Milliseconds / 10}",
				position: new Vector2(100, 270),
				color: mutedColor,
				alignHorizontal: 0f,
				alignVertical: 0f,
				scale: 0.3f);

			/*if (Player.poweredUp)
			{
				spriteBatch.DrawStringAdvanced(
					font: GameSettings.Assets.Fonts["default_font"],
					text: "SUPER CAT",
					position: new Vector2(GameSettings.WindowWidth / 2, 230),
					color: ColorUtilities.HSV(GameSettings.ElapsedGameTime, 1, 1, 1),
					alignHorizontal: 0.5f,
					alignVertical: 0.6f,
					scale: 1f);
			}*/



			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"STREAK",
				position: new Vector2(GameSettings.WindowWidth - 100, 150),
				color: streakColor,
				alignHorizontal: 1f,
				alignVertical: 0f,
				scale: 0.3f);

			float width = GameSettings.Assets.Fonts["default_font"].MeasureString($"{GameSettings.PlatformStreak}").X;

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"{GameSettings.PlatformStreak}",
				position: new Vector2(GameSettings.WindowWidth - 100 - width / 2 * 0.5f, 230),
				color: streakColor,
				alignHorizontal: 0.5f,
				alignVertical: 0.6f,
				scale: 0.5f + MathF.Min(0.5f, MathF.Pow(GameSettings.PlatformStreak / 100f, 1.2f)) + MathF.Sin(Player.timeSinceJump * 20) * MathF.Max(0, 1f - Player.timeSinceJump / .3f) * MathF.Min(0.5f, GameSettings.PlatformStreak / 50f),
				rotation: MathF.Cos(Player.timeSinceJump * 30) * MathF.Max(0, 1f - Player.timeSinceJump / .3f) * MathF.Min(0.3f, GameSettings.PlatformStreak / 50f)
				);

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"{ScoreMultiplier.ToString("0.#")}x",
				position: new Vector2(GameSettings.WindowWidth - 100, 270),
				color: streakColor,
				alignHorizontal: 1f,
				alignVertical: 0f,
				scale: 0.3f);
		}

		private void DrawTutorial(SpriteBatch spriteBatch)
		{
			float startTextHeight = 200;
			float startTextSpacing = 80;
			float moveTextHeight = -130;
			float smallTextScale = 0.4f;
			float totalOpacity = 1 - MathF.Min(1, GameSettings.ElapsedGameTime / 2f);
			Color tutorialColor = ColorUtilities.Premultiply(Color.White, totalOpacity);
			Color tutorialOffColor = ColorUtilities.Premultiply(Color.White, .5f * totalOpacity);

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"[A] [D] MOVE",
				position: new Vector2(0, moveTextHeight),
				color: tutorialOffColor,
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				scale: smallTextScale);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"PRESS",
				position: new Vector2(0, startTextHeight - startTextSpacing),
				color: tutorialOffColor,
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				scale: smallTextScale);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"SPACE",
				position: new Vector2(0, startTextHeight),
				color: tutorialColor,
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				scale: 1f);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"TO START",
				position: new Vector2(0, startTextHeight + startTextSpacing),
				color: tutorialOffColor,
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				scale: smallTextScale);
		}
	}
}
