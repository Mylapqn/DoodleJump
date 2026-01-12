using DoodleJump.Core;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DoodleJump.Hierarchy
{
	internal abstract class BaseScreen : Screen
	{
		private const int BOTTOM_TEXT_EDGE_PADDING = 100;
		private const float BOTTOM_TEXT_SCALE = 0.5f;
		protected KeyboardState previousKeyboardState;
		protected KeyboardState currentKeyboardState;

		public override void Update(float dt)
		{
			previousKeyboardState = currentKeyboardState;
			currentKeyboardState = Keyboard.GetState();
		}

		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
			DrawBottomText(spriteBatch);
			spriteBatch.End();
		}

		private static void DrawBottomText(SpriteBatch spriteBatch)
		{
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"{DateTime.Now.ToString("HH:mm:ss")}",
				position: new Vector2(BOTTOM_TEXT_EDGE_PADDING, GameSettings.WindowHeight - BOTTOM_TEXT_EDGE_PADDING),
				color: Color.Red,
				scale: BOTTOM_TEXT_SCALE,
				alignHorizontal: 0f);

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"Bombs: {GameSettings.BombCount}/{GameSettings.BounceCount}",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight - BOTTOM_TEXT_EDGE_PADDING),
				color: Color.Red,
				scale: BOTTOM_TEXT_SCALE,
				alignHorizontal: 0.5f);

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: $"Score: {(int)GameSettings.ScoreDecimal}",
				position: new Vector2(GameSettings.WindowWidth - BOTTOM_TEXT_EDGE_PADDING, GameSettings.WindowHeight - BOTTOM_TEXT_EDGE_PADDING),
				color: Color.Red,
				scale: BOTTOM_TEXT_SCALE,
				alignHorizontal: 1f);

		}
	}
}
