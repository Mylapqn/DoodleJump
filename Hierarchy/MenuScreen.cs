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

namespace DoodleJump.Hierarchy
{
	internal class MenuScreen : Screen
	{
		private float totalTime = 0f;
		private SpriteSheetAnimation cat;

		public override void Initialize()
		{
			GameSettings.ActiveScreen = this;
			totalTime = 0f;
			cat = new SpriteSheetAnimation(GameSettings.Assets.Textures["cat_jump"], 2, 8, 3, 10, 11);
			cat.Scale = GameSettings.PIXEL_SCALE * 2;
		}

		public override void Deinitialize()
		{
			MediaPlayer.Stop();
		}

		public override void LoadContent(ContentManager content)
		{
			Debug.WriteLine("LoadContent MenuScreen");
		}

		public override void Update(float dt)
		{
			totalTime += dt;
			cat.Update(dt);
			if (Input.IsKeyReleased(Keys.Space))
			{
				GameSettings.PlayScreen.Initialize();
				GameSettings.Assets.Sounds["success"].Play(.2f, 0, 0f);
			}
		}

		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			spriteBatch.GraphicsDevice.Clear(new Color(.1f, .1f, .1f));
			spriteBatch.Begin(samplerState: SamplerState.LinearClamp);
			polygonDrawer.Begin();
			spriteBatch.Draw(GameSettings.Assets.Textures["vignette"], new Rectangle(0, 0, GameSettings.WindowWidth, GameSettings.WindowHeight), Color.White);

			float textScale = 0.4f;
			int textSpacing = 50;

			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "RAINBOW CAT",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight / 3),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: ColorUtilities.HSV(totalTime, 1, 1, 1),
				scale: MathF.Sin(totalTime * 2f) * .1f + .9f,
				rotation: MathF.Sin(totalTime * 4f) * .1f
				);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "PRESS SPACE TO START",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight / 2 + 100 + textSpacing * 1),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.White,
				scale: textScale
				);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "HOLD A AND D TO MOVE",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight / 2 + 100 + textSpacing * 2),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.DarkGray,
				scale: textScale
				);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "JUMP TO DIFFERENT PLATFORMS TO BUILD UP YOUR STREAK",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight / 2 + 100 + textSpacing * 4),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.White,
				scale: textScale
				);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "BETTER STREAK = BETTER SCORE",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight / 2 + 100 + textSpacing * 5),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.DarkGray,
				scale: textScale
				);
			spriteBatch.End();
			polygonDrawer.End();

			spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			cat.Position = new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight / 2);
			cat.Draw(spriteBatch);
			spriteBatch.End();


		}
	}
}
