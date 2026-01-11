using DoodleJump.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Rendering
{
	public class ColorUtilities
	{
		public static Color ColorFromHex(uint rrggbbaa)
		{
			uint rr = (rrggbbaa >> 24) & 0xFF;
			uint gg = (rrggbbaa >> 16) & 0xFF;
			uint bb = (rrggbbaa >> 8) & 0xFF;
			uint aa = rrggbbaa & 0xFF;

			return new Color((aa << 24) | (bb << 16) | (gg << 8) | rr);
		}
		public static Color StreakColor(int streak)
		{
			if (streak >= 60)
			{
				return ColorUtilities.ColorFromHex(0xFF006FFF);
			}
			else if (streak >= 40)
			{
				return ColorUtilities.ColorFromHex(0xFF3300FF);
			}
			else if (streak >= 30)
			{
				return ColorUtilities.ColorFromHex(0xFF8000FF);
			}
			else if (streak >= 20)
			{
				return ColorUtilities.ColorFromHex(0xF4E327FF);
			}
			else if (streak >= 10)
			{
				return ColorUtilities.ColorFromHex(0x3DDEB8FF);
			}
			return Color.White;
		}
		public static Color Premultiply(Color color, float alpha)
		{
			return new Color(
				(int)(color.R * alpha),
				(int)(color.G * alpha),
				(int)(color.B * alpha),
				(int)(color.A * alpha)
				);
		}
		public static Color HSV(float hue, float sat, float val, float alpha = 1f)
		{
			hue = hue - MathF.Floor(hue); // wrap 0–1

			float chroma = val * sat;
			float hueSector = hue * 6f;
			float secondComponent = chroma * (1f - MathF.Abs(hueSector % 2f - 1f));
			float lightnessOffset = val - chroma;

			float r = 0, g = 0, b = 0;

			if (hueSector < 1f) { r = chroma; g = secondComponent; }
			else if (hueSector < 2f) { r = secondComponent; g = chroma; }
			else if (hueSector < 3f) { g = chroma; b = secondComponent; }
			else if (hueSector < 4f) { g = secondComponent; b = chroma; }
			else if (hueSector < 5f) { r = secondComponent; b = chroma; }
			else { r = chroma; b = secondComponent; }

			return new Color(
				(byte)((r + lightnessOffset) * 255f),
				(byte)((g + lightnessOffset) * 255f),
				(byte)((b + lightnessOffset) * 255f),
				(byte)(alpha * 255f)
			);
		}
	}
}
