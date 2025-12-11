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
	public class EndScreen : Screen
	{

		public override void Initialize()
		{
			GameSettings.ActiveScreen = this;
		}

		public override void LoadContent(ContentManager content)
		{
			Debug.WriteLine("LoadContent EndScreen");
		}

		public override void Update(float dt)
		{
			if (Input.IsMouseButtonReleased(Input.MouseButton.Left))
			{
				Game1.Instance.Exit();
			}
		}

		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			spriteBatch.GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin(samplerState: SamplerState.PointClamp);
			polygonDrawer.Begin();
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "Game Over",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight / 2),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.White,
				scale: 1
				);
			spriteBatch.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: "Click anywhere to close the game",
				position: new Vector2(GameSettings.WindowWidth / 2, GameSettings.WindowHeight / 2+200),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				color: Color.White,
				scale: .3f
				);
			spriteBatch.End();
			polygonDrawer.End();


		}
	}
}
