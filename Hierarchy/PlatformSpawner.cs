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
		public enum PlatformType
		{
			Normal,
			Bounce,
			Moving
		}
		public Platform topPlatform;
		private Vector2 currentPosition = new Vector2();
		public PlatformSpawner() { }

		public List<Platform> SpawnPlaforms(float topY)
		{
			List<Platform> addedPlatforms = new();
			while (currentPosition.Y > topY - 100)
			{
				PlatformType platformType = PlatformType.Normal;
				if (GameSettings.Random.NextDouble() < 0.1) platformType = PlatformType.Bounce;
				else if (GameSettings.Random.NextDouble() < 0.1) platformType = PlatformType.Moving;

				Platform newPlatform;
				switch (platformType)
				{
					case PlatformType.Bounce:
						newPlatform = new BouncePlatform(new SpriteSheet(GameSettings.Assets.Textures["platform_bounce"]));
						break;
					case PlatformType.Moving:
						newPlatform = new MovingPlatform(new SpriteSheet(GameSettings.Assets.Textures["platform_moving"]));
						break;
					default:
					case PlatformType.Normal:
						newPlatform = new Platform(new SpriteSheet(GameSettings.Assets.Textures["platform"]));
						break;

				}
				addedPlatforms.Add(newPlatform);
				const float platformWidth = 50;
				if (currentPosition.X < -GameSettings.GameWidth / 2 + platformWidth)
				{
					currentPosition.X = GameSettings.GameWidth / 2 - platformWidth;
				}
				if (currentPosition.X > GameSettings.GameWidth / 2 - platformWidth)
				{
					currentPosition.X = -GameSettings.GameWidth / 2 + platformWidth;
				}
				newPlatform.Position = currentPosition;

				int maxOffset = 350;

				switch (platformType)
				{
					case PlatformType.Bounce:
						currentPosition.Y -= 100;
						break;
					case PlatformType.Moving:
						currentPosition.Y -= 150;
						break;
					default:
					case PlatformType.Normal:
						currentPosition.Y -= 250 + GameSettings.Random.Next(50) - 25;
						break;

				}

				int offset = GameSettings.Random.Next(maxOffset * 2) - maxOffset;
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
