using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Rendering
{
	internal static class SpriteBatchExtension
	{
		public static Vector2 DrawStringAdvanced(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color = new Color(), float alignHorizontal = 0, float alignVertical = 0, float scale = 1, float rotation = 0)
		{
			Vector2 pivot = new();
			Vector2 measure = new();
			measure = font.MeasureString(text);
			if (alignHorizontal != 0 || alignVertical != 0)
			{
				pivot = measure;
				pivot.X *= alignHorizontal;
				pivot.Y *= alignVertical;
			}
			spriteBatch.DrawString(font, text, position, color, rotation, pivot, scale, SpriteEffects.None, 0);
			return Vector2.Multiply(measure, scale);
		}
	}
}
