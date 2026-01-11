using DoodleJump.Core;
using DoodleJump.Objects;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private Vector2 currentPosition = new Vector2(0, -200);
		public PlatformSpawner() { }
		private int previousX = 0;

		public List<Platform> SpawnPlatforms(float topY)
		{
			List<Platform> addedPlatforms = new();
			int maxOffset = 350;
			int platformWidth = 50;
			int maxPlatformX = GameSettings.GameWidth / 2 - platformWidth;
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
				Debug.WriteLine($"Trying to put platform at {currentPosition.X}");
				if (currentPosition.X < -maxPlatformX)
				{
					Debug.WriteLine($"{currentPosition.X} is less than {-maxPlatformX}.");
					Debug.WriteLine($"Comparing if Math.Abs({-maxPlatformX}- {previousX}) + {platformWidth * 2} is more than {maxOffset}.");
					Debug.WriteLine($"So, comparing if {Math.Abs(-maxPlatformX - previousX) + platformWidth * 2} is more than {maxOffset}.");
					if (Math.Abs(-maxPlatformX - previousX) + platformWidth * 2 > maxOffset)
						Debug.WriteLine($"It is.");
					else
						Debug.WriteLine($"It is not.");
					if (Math.Abs(-maxPlatformX - previousX) + platformWidth * 2 > maxOffset)
						currentPosition.X = -maxPlatformX;
					else
						currentPosition.X = maxPlatformX;
				}
				if (currentPosition.X > maxPlatformX)
				{
					Debug.WriteLine($"{currentPosition.X} is more than {-maxPlatformX}.");
					Debug.WriteLine($"Comparing if Math.Abs({maxPlatformX}- {previousX}) + {platformWidth * 2} is more than {maxOffset}.");
					Debug.WriteLine($"So, comparing if {Math.Abs(maxPlatformX - previousX) + platformWidth * 2} is more than {maxOffset}.");
					if (Math.Abs(maxPlatformX - previousX) + platformWidth * 2 > maxOffset)
						Debug.WriteLine($"It is.");
					else
						Debug.WriteLine($"It is not.");
					if (Math.Abs(maxPlatformX - previousX) + platformWidth * 2 > maxOffset)
						currentPosition.X = maxPlatformX;
					else
						currentPosition.X = -maxPlatformX;
				}
				Debug.WriteLine($"Final platform at {currentPosition.X}");
				previousX = (int)currentPosition.X;
				newPlatform.Position = currentPosition;

				switch (platformType)
				{
					case PlatformType.Bounce:
						currentPosition.Y -= 280;
						break;
					case PlatformType.Moving:
						currentPosition.Y -= 150;
						break;
					default:
					case PlatformType.Normal:
						currentPosition.Y -= 250 + GameSettings.Random.Next(-25,26);
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
