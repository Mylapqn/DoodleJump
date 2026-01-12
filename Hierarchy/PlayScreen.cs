using DoodleJump.Core;
using DoodleJump.Objects;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace DoodleJump.Hierarchy
{
	internal class PlayScreen : Screen
	{
		//Constants
		private const int CAMERA_MOVE_SPEED_UP = 200;
		private const float MUSIC_VOLUME = 0.2f;
		private const float TIME_SCALE = 1f;
		private const float MAX_TIME_SCALE_HEIGHT = 100000f;
		private const float MAX_TIME_SCALE = 3f;
		private const float SCORE_FROM_HEIGHT = 0.01f;
		private const float CAMERA_ZOOM = 1f;

		//UI
		private const float TEXT_SCALE_SMALL = 0.3f;
		private const float TEXT_SCALE_LARGE = 1f;
		private const int MAX_STREAK_EFFECT = 50;
		const int SIDE_PADDING = 100;

		//Streak
		private const int MAX_STREAK_MULTIPLIER = 4;
		private const int BASE_STREAK_MULTIPLIER = 1;
		private const float MULTIPLIER_PER_STREAK = .06f;

		public List<Platform> Platforms { get; private set; }
		public List<GameObject> GameObjects { get; private set; }
		private Player Player { get; set; }

		public Camera Camera;
		private PlatformSpawner platformSpawner = new PlatformSpawner();
		private List<BackgroundLayer> backgroundLayers = new List<BackgroundLayer>();
		private float lastMaxHeight = 0f;
		private float ScoreMultiplier => MathF.Min(MAX_STREAK_MULTIPLIER, BASE_STREAK_MULTIPLIER + (float)GameSettings.PlatformStreak * MULTIPLIER_PER_STREAK);
		private bool slowMode = false;
		private TextPopupManager PopupManager = new TextPopupManager();
		private List<HighScore> highScores;
		private bool started = false;


		public override void Initialize()
		{
			GameSettings.ActiveScreen = this;
			this.Camera = new Camera();
			this.GameObjects = new List<GameObject>();
			this.Platforms = new List<Platform>();
			highScores = GameSettings.SaveSystem.LoadHighScores();

			SpawnBackgroundLayers();

			Player = new Player(
				new SpriteSheetAnimation(
					GameSettings.Assets.Textures["cat_jump"],
					rows: 2,
					columns: 8,
					minIndex: 3,
					maxIndex: 10,
					idleIndex: 11),
				PopupManager);
			this.GameObjects.Add(Player);
			GameSettings.TimeScale = 1f;

			Platform initialPlatform = new Platform(new SpriteSheet(GameSettings.Assets.Textures["platform"])) { };
			initialPlatform.Position = new Vector2(0, initialPlatform.Visualization.Size.Y / 2);
			initialPlatform.Visualization.Size = new Point(GameSettings.GameWidth, initialPlatform.Visualization.Size.Y);
			initialPlatform.IsVisible = false;
			Platforms.Add(initialPlatform);
			GameObjects.Add(initialPlatform);
			platformSpawner.Initialize(new Vector2(0, -220), GameSettings.Assets.Textures["platform"].Width / 2 * GameSettings.PIXEL_SCALE);


		}

		private void SpawnBackgroundLayers()
		{
			(string name, float parallax, float height)[] layers = new (string, float, float)[]
			{
				("bg_sky", 1f, 800),
				("bg_city_3", 0.7f, -1100),
				("bg_city_2", 0.5f, -700),
				("bg_city_1", 0.3f, -1000),
				("bg_city_0", 0f, 12)
			};
			foreach (var layer in layers)
			{
				BackgroundLayer bgLayer = new BackgroundLayer(new SpriteSheet(GameSettings.Assets.Textures[layer.name]), layer.parallax, new Vector2(0, layer.height));
				GameObjects.Add(bgLayer);
				backgroundLayers.Add(bgLayer);
			}
		}

		public override void Deinitialize()
		{
			MediaPlayer.Stop();
		}

		public override void LoadContent(ContentManager content)
		{
			Debug.WriteLine("LoadContent PlayScreen");
		}

		public override void Update(float dt)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Input.IsKeyReleased(Keys.Escape))
				GameSettings.EndScreen.Initialize();

			if (started)
			{
				GameSettings.ElapsedGameTime += dt;
				Camera.Position += new Vector2(0, -CAMERA_MOVE_SPEED_UP * dt);
			}
			else if (!Player.walking)
			{
				started = true;
				MediaPlayer.Play(GameSettings.Assets.Songs["play_music"]);
				MediaPlayer.IsRepeating = true;
				MediaPlayer.Volume = MUSIC_VOLUME;
			}

			Camera.Zoom = CAMERA_ZOOM * MathF.Max(1f, 1.1f - GameSettings.ElapsedGameTime / 16f);

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

			GameSettings.TimeScale = TIME_SCALE * (1f + MathF.Min(GameSettings.MaxHeightReached / MAX_TIME_SCALE_HEIGHT, MAX_TIME_SCALE));
			if (slowMode)
			{
				GameSettings.TimeScale *= 0.5f;
			}
			float addedScore = (GameSettings.MaxHeightReached - lastMaxHeight) * SCORE_FROM_HEIGHT * ScoreMultiplier;
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
				spriteBatch.Draw(GameSettings.Assets.Textures["pixel"], new Rectangle(-GameSettings.WindowWidth, 12, GameSettings.WindowWidth * 2, GameSettings.WindowHeight), new Rectangle(0, 0, 1, 1), ColorUtilities.ColorFromHex(0x101010FF));
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
			DrawVignetteByElevation(spriteBatch, 1000);
			DrawVignetteByElevation(spriteBatch, 5000);

			// Score Label

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"SCORE",
				position: new Vector2(SIDE_PADDING, 150),
				color: mainColor,
				alignHorizontal: 0f,
				alignVertical: 0f,
				scale: TEXT_SCALE_SMALL);

			// Score Number

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"{(int)GameSettings.ScoreDecimal}",
				position: new Vector2(SIDE_PADDING, 230),
				color: mainColor,
				alignHorizontal: 0f,
				alignVertical: 0.6f,
				scale: TEXT_SCALE_LARGE);

			// Timer

			TimeSpan time = TimeSpan.FromSeconds(GameSettings.ElapsedGameTime);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"{time.Minutes}:{time.Seconds}.{time.Milliseconds / 10}",
				position: new Vector2(SIDE_PADDING, 270),
				color: mutedColor,
				alignHorizontal: 0f,
				alignVertical: 0f,
				scale: TEXT_SCALE_SMALL);

			// Meters

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"{(int)GameSettings.MaxHeightReached / GameSettings.HEIGHT_PER_METER} M",
				position: new Vector2(SIDE_PADDING, 310),
				color: mutedColor,
				alignHorizontal: 0f,
				alignVertical: 0f,
				scale: TEXT_SCALE_SMALL);

			// Streak Label

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"STREAK",
				position: new Vector2(GameSettings.WindowWidth - SIDE_PADDING, 150),
				color: streakColor,
				alignHorizontal: 1f,
				alignVertical: 0f,
				scale: TEXT_SCALE_SMALL);

			// Streak Number Effects

			float streakRatio = MathF.Min(1f, (float)GameSettings.PlatformStreak / MAX_STREAK_EFFECT);
			float jumpFadeout = 1f - MathF.Min(1f, Player.timeSinceJump / 0.3f);
			float baseScale = 0.5f;
			float streakBonus = MathF.Min(0.5f, MathF.Pow(streakRatio, 1.2f));
			float jumpWiggle = MathF.Sin(Player.timeSinceJump * 20f)
								* jumpFadeout
								* MathF.Min(0.5f, streakRatio);

			float highStreakShake = GameSettings.PlatformStreak >= MAX_STREAK_EFFECT ? MathF.Sin(GameSettings.ElapsedGameTime * 60f) * .1f : 0f;

			float scale = TEXT_SCALE_LARGE * (baseScale + streakBonus + jumpWiggle);
			float width = GameSettings.Assets.Fonts["default_font"].MeasureString($"{GameSettings.PlatformStreak}").X * scale * .8f;

			// Streak Number

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"{GameSettings.PlatformStreak}",
				position: new Vector2(GameSettings.WindowWidth - SIDE_PADDING - width / 2, 230),
				color: streakColor,
				alignHorizontal: 0.5f,
				alignVertical: 0.6f,
				scale: scale,
				rotation: MathF.Cos(Player.timeSinceJump * 30) * jumpFadeout * MathF.Min(0.3f, streakRatio) + highStreakShake
				);

			// Score Multiplier

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"{ScoreMultiplier.ToString("0.#")}x",
				position: new Vector2(GameSettings.WindowWidth - SIDE_PADDING, 270),
				color: streakColor,
				alignHorizontal: 1f,
				alignVertical: 0f,
				scale: TEXT_SCALE_SMALL);
		}

		private void DrawVignetteByElevation(SpriteBatch spriteBatch, float maxElevation)
		{
			if (Camera.Position.Y > -maxElevation)
				spriteBatch.Draw(GameSettings.Assets.Textures["vignette"], new Rectangle(0, 0, GameSettings.WindowWidth, GameSettings.WindowHeight), ColorUtilities.Premultiply(Color.White, (Camera.Position.Y + maxElevation) / maxElevation));
		}

		private void DrawTutorial(SpriteBatch spriteBatch)
		{
			float startTextHeight = 200;
			float startTextSpacing = 80;
			float moveTextHeight = -130;
			float smallTextScale = TEXT_SCALE_SMALL;
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
				scale: TEXT_SCALE_LARGE);
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
