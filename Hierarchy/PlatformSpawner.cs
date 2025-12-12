using DoodleJump.Core;
using DoodleJump.Objects;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Hierarchy
{
	public class PlatformSpawner
	{
		public Platform topPlatform;
		private Vector2 currentPosition = new Vector2();
		public PlatformSpawner() { }

		public List<Platform> SpawnPlaforms(float topY)
		{
			List<Platform> addedPlatforms = new();
			while (currentPosition.Y > topY - 100)
			{
				bool bounce = GameSettings.Random.NextDouble() < 0.1;
				Platform newPlatform = new Platform(new SpriteSheet(GameSettings.Assets.Textures[bounce ? "platform_bounce" : "platform"]));
				addedPlatforms.Add(newPlatform);
				newPlatform.BounceForce = bounce ? 40 : 23;
				if (currentPosition.X < -GameSettings.WindowWidth / 2 + 100)
				{
					currentPosition.X = -GameSettings.WindowWidth / 2 + 100;
				}
				if (currentPosition.X > GameSettings.WindowWidth / 2 - 100)
				{
					currentPosition.X = GameSettings.WindowWidth / 2 - 100;
				}
				newPlatform.Position = currentPosition;

				int maxOffset = 350;
				int offset = GameSettings.Random.Next(maxOffset * 2) - maxOffset;
				currentPosition.Y -= bounce ? 100 : 250 + GameSettings.Random.Next(50) - 25;
				currentPosition.X += offset;
			}
			return addedPlatforms;
		}

		public List<Platform> GetBottomPlatformsToRemove(List<Platform> platforms, float bottomY)
		{
			List<Platform> removedPlatforms = new();
			foreach (var platform in platforms.ToList())
			{
				if (platform.Position.Y > bottomY + 100)
				{
					removedPlatforms.Add(platform);
				}
			}
			return removedPlatforms;
		}
	}
}
