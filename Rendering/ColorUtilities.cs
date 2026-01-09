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
	}
}
