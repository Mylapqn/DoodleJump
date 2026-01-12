using DoodleJump.Core;
using DoodleJump.Objects;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
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
	internal class EndScreen : BaseScreen
	{
		//UI
		const int TEXT_SPACING = 50;
		const float TEXT_SCALE = .4f;

		//Leaderboard UI
		const float LEADERBOARD_TEXT_SCALE = 0.3f;
		const int LEADERBOARD_TEXT_SPACING = 30;
		const int LEADERBOARD_MAX_ENTRIES = 10;
		const int LEADERBOARD_EDGE_PADDING = 200;
		private const int NAME_MAX_LENGTH = 20;
		private const int MIN_SCORE = 10;


		List<HighScore> highScores;
		float elapsedTime = 0f;
		bool isNewHighScore = false;
		bool enteringName = false;
		string playerName = "";
		public override void Initialize()
		{
			MediaPlayer.Stop();
			GameSettings.ActiveScreen = this;
			GameSettings.TimeScale = 1f;
			highScores = GameSettings.SaveSystem.LoadHighScores();
			isNewHighScore = IsNewHighScore((int)GameSettings.ScoreDecimal);
			if (isNewHighScore)
			{
				enteringName = true;
			}
		}
		public override void Deinitialize()
		{
			MediaPlayer.Stop();
		}
		private bool IsNewHighScore(int score)
		{
			if (score <= MIN_SCORE)
				return false;
			if (highScores.Count < LEADERBOARD_MAX_ENTRIES)
				return true;
			return score > highScores[highScores.Count - 1].Score;
		}
		private void AddHighScore(HighScore newScore)
		{
			if (IsNewHighScore(newScore.Score))
				highScores.Add(newScore);
			highScores = highScores.OrderByDescending(hs => hs.Score).ToList();
			GameSettings.SaveSystem.SaveHighScores(highScores);
		}

		public override void LoadContent(ContentManager content)
		{
			Debug.WriteLine("LoadContent EndScreen");
		}

		public override void Update(float dt)
		{
			elapsedTime += dt;
			if (enteringName)
			{
				char typedChar = Input.GetTypedChar();
				if (typedChar != '\0' && playerName.Length < NAME_MAX_LENGTH)
				{
					playerName += typedChar;
				}
				if (Input.IsKeyPressed(Keys.Back) && playerName.Length > 0)
				{
					playerName = playerName.Substring(0, playerName.Length - 1);
				}
				if (Input.IsKeyPressed(Keys.Enter) && playerName.Length > 0)
				{
					enteringName = false;
					AddHighScore(new HighScore(playerName, (int)GameSettings.ScoreDecimal, (int)GameSettings.MaxHeightReached, DateOnly.FromDateTime(DateTime.Now)));
				}
			}
			if (Input.IsKeyReleased(Keys.Space) && !enteringName)
			{
				Game1.Instance.StartGame(false);
			}
			if (Input.IsKeyReleased(Keys.Escape))
			{
				Game1.Instance.Exit();
			}

			base.Update(dt);
		}

		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			spriteBatch.GraphicsDevice.Clear(Color.Black);
			int TopPadding = GameSettings.WindowHeight / 6;
			spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			polygonDrawer.Begin();
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "GAME OVER",
				position: new Vector2(GameSettings.WindowWidth / 2, TopPadding),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.White,
				scale: 1
				);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"SCORE: {(int)GameSettings.ScoreDecimal}",
				position: new Vector2(GameSettings.WindowWidth / 2, TopPadding + TEXT_SPACING * 2),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.White,
				scale: TEXT_SCALE
				);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"REACHED HEIGHT: {(int)GameSettings.MaxHeightReached / GameSettings.HEIGHT_PER_METER} m",
				position: new Vector2(GameSettings.WindowWidth / 2, TopPadding + TEXT_SPACING * 3),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.DarkGray,
				scale: TEXT_SCALE
				);
			if (isNewHighScore)
			{
				DrawHighScore(spriteBatch, TEXT_SPACING, TEXT_SCALE, TopPadding + TEXT_SPACING * 4);
			}
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"LEADERBOARD",
				position: new Vector2(GameSettings.WindowWidth / 2, TopPadding + TEXT_SPACING * 8),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.White,
				scale: TEXT_SCALE
				);

			DrawLeaderboard(spriteBatch, TopPadding + TEXT_SPACING * 9);

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "Press Space to restart",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight - 200),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.DarkGray,
				scale: LEADERBOARD_TEXT_SCALE
			);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "Press ESC to close the game",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight - 200 + TEXT_SPACING * 1),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.DarkGray,
				scale: LEADERBOARD_TEXT_SCALE
				);
			spriteBatch.End();
			polygonDrawer.End();

			base.Draw(spriteBatch, polygonDrawer);

		}

		private void DrawHighScore(SpriteBatch spriteBatch, int textSpacing, float textScale, int initialY)
		{
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"NEW HIGH SCORE",
				position: new Vector2(GameSettings.WindowWidth / 2, initialY + textSpacing * 0),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: ColorUtilities.HSV(elapsedTime, 1, 1, 1),
				scale: textScale);
			if (enteringName)
			{
				float currentX = 200;
				currentX += spriteBatch.DrawStringAdvanced(
					font: GameSettings.Assets.Fonts["default_font"],
					text: $"ENTER NAME: ",
					position: new Vector2(currentX, initialY + textSpacing * 2),
					alignHorizontal: 0f,
					alignVertical: 0.5f,
					color: Color.DarkGray,
					scale: textScale
					).X;
				currentX += spriteBatch.DrawStringAdvanced(
					font: GameSettings.Assets.Fonts["default_font"],
					text: $"{playerName}",
					position: new Vector2(currentX, initialY + textSpacing * 2),
					alignHorizontal: 0f,
					alignVertical: 0.5f,
					color: Color.White,
					scale: textScale
					).X;
				currentX += spriteBatch.DrawStringAdvanced(
					font: GameSettings.Assets.Fonts["default_font"],
					text: $"|",
					position: new Vector2(currentX, initialY + textSpacing * 2),
					alignHorizontal: 0f,
					alignVertical: 0.5f,
					color: ColorUtilities.Premultiply(Color.White, MathF.Sin(elapsedTime * 10f)),
					scale: textScale
					).X;
			}

		}

		private void DrawLeaderboard(SpriteBatch spriteBatch, int startY)
		{
			int currentY = startY;
			for (int i = 0; i < Math.Min(LEADERBOARD_MAX_ENTRIES, highScores.Count); i++)
			{
				HighScore hs = highScores[i];
				Color color = Color.Gray;
				if (hs.Score == (int)GameSettings.ScoreDecimal)
				{
					color = Color.White;
				}
				spriteBatch.DrawStringAdvanced(
					font: GameSettings.Assets.Fonts["default_font"],
					text: $"{i + 1}. {hs.PlayerName}",
					position: new Vector2(LEADERBOARD_EDGE_PADDING, currentY),
					color: color,
					scale: LEADERBOARD_TEXT_SCALE
					);
				spriteBatch.DrawStringAdvanced(
					font: GameSettings.Assets.Fonts["default_font"],
					text: $"{hs.Score}",
					position: new Vector2(GameSettings.WindowWidth / 2, currentY),
					color: color,
					alignHorizontal: 0.0f,
					scale: LEADERBOARD_TEXT_SCALE
					);
				spriteBatch.DrawStringAdvanced(
					font: GameSettings.Assets.Fonts["default_font"],
					text: $"{hs.Date.ToString("yyyy-MM-dd")}",
					position: new Vector2(GameSettings.WindowWidth - LEADERBOARD_EDGE_PADDING, currentY),
					color: color,
					alignHorizontal: 1f,
					scale: LEADERBOARD_TEXT_SCALE
					);
				currentY += LEADERBOARD_TEXT_SPACING;
			}
		}
	}
}
