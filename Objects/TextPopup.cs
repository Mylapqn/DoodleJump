using DoodleJump.Core;
using DoodleJump.Hierarchy;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Objects
{
	public class TextPopup
	{
		public Vector2 Position { get; set; }
		public string Text { get; set; }
		public float Duration { get; set; }
		public float ElapsedTime { get; set; }
		private Color Color { get; set; }
		public TextPopup(Vector2 position, string text, Color color, float duration = 1)
		{
			this.Position = position;
			this.Text = text;
			this.Color = color;
			this.Duration = duration;
		}

		public void Draw(SpriteBatch sb, PolygonDrawer polygonDrawer)
		{
			sb.DrawStringAdvanced(
				font: GameSettings.Assets.Fonts["default_font"],
				text: Text,
				position: Position + new Vector2(0, -(ElapsedTime / Duration) * 30f),
				color: ColorUtilities.Premultiply(Color, 1 - (ElapsedTime / Duration)),
				alignHorizontal: 0.5f,
				alignVertical: 0.5f,
				scale: 0.5f,
				rotation: 0
				);
		}
	}
}
