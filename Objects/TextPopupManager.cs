using DoodleJump.Rendering;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Objects
{
	internal class TextPopupManager
	{
		private List<TextPopup> activePopups = new List<TextPopup>();
		public TextPopupManager() { }
		public void AddPopup(TextPopup popup)
		{
			activePopups.Add(popup);
		}
		public void Update(float dt)
		{
			for (int i = activePopups.Count - 1; i >= 0; i--)
			{
				activePopups[i].ElapsedTime += dt;
				if (activePopups[i].ElapsedTime >= activePopups[i].Duration)
				{
					activePopups.RemoveAt(i);
				}
			}
		}
		public void Draw(SpriteBatch sb, PolygonDrawer polygonDrawer)
		{
			foreach (var popup in activePopups)
			{
				popup.Draw(sb, polygonDrawer);
			}
		}
	}
}
