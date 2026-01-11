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
using static System.Formats.Asn1.AsnWriter;

namespace DoodleJump.Hierarchy
{
	public class EndScreen : Screen
	{
		List<HighScore> highScores;
		float elapsedTime = 0f;
		bool isNewHighScore = false;
		bool enteringName = false;
		string playerName = "";
		public override void Initialize()
		{
			GameSettings.ActiveScreen = this;
			GameSettings.TimeScale = 1f;
			highScores = GameSettings.SaveSystem.LoadHighScores();
			isNewHighScore = IsNewHighScore((int)GameSettings.ScoreDecimal);
			if (isNewHighScore)
			{
				enteringName = true;
			}
		}
		private bool IsNewHighScore(int score)
		{
			if (score == 0)
				return false;
			if (highScores.Count < 10)
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
				if (typedChar != '\0' && playerName.Length < 20)
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
					AddHighScore(new HighScore(playerName, (int)GameSettings.ScoreDecimal, (int)GameSettings.MaxHeightReached));
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
		}

		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			spriteBatch.GraphicsDevice.Clear(Color.Black);
			int textSpacing = 50;
			float textScale = 0.4f;
			int initialY = GameSettings.WindowHeight / 6;
			spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			polygonDrawer.Begin();
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "GAME OVER",
				position: new Vector2(GameSettings.WindowWidth / 2, initialY),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.White,
				scale: 1
				);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"SCORE: {(int)GameSettings.ScoreDecimal}",
				position: new Vector2(GameSettings.WindowWidth / 2, initialY + textSpacing * 2),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.White,
				scale: textScale
				);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"REACHED HEIGHT: {(int)GameSettings.MaxHeightReached / 100} m",
				position: new Vector2(GameSettings.WindowWidth / 2, initialY + textSpacing * 3),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.DarkGray,
				scale: textScale
				);
			if (isNewHighScore)
			{
				DrawHighScore(spriteBatch, textSpacing, textScale, initialY + textSpacing * 4);
			}
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"LEADERBOARD",
				position: new Vector2(GameSettings.WindowWidth / 2, initialY + textSpacing * 8),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.White,
				scale: textScale
				);

			DrawLeaderboard(spriteBatch, initialY + textSpacing * 9);

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "Press Space to restart",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight - 200),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.DarkGray,
				scale: .3f
			);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "Press ESC to close the game",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight - 200 + textSpacing * 1),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.DarkGray,
				scale: .3f
				);
			spriteBatch.End();
			polygonDrawer.End();


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
				textScale = 0.5f;
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
			float leaderboardScale = 0.3f;
			int leaderboardSpacing = 30;
			for (int i = 0; i < Math.Min(10, highScores.Count); i++)
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
					position: new Vector2(200, currentY),
					color: color,
					scale: leaderboardScale
					);
				spriteBatch.DrawStringAdvanced(
					font: GameSettings.Assets.Fonts["default_font"],
					text: $"{hs.Score}",
					position: new Vector2(GameSettings.WindowWidth - 200, currentY),
					color: color,
					alignHorizontal: 1f,
					scale: leaderboardScale
					);
				currentY += leaderboardSpacing;
			}
		}
	}
}
