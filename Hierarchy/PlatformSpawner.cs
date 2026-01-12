using DoodleJump.Core;
using DoodleJump.Objects;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Hierarchy
{
	internal class PlatformSpawner
	{
		//Constants

		private const float BUILDING_HEIGHT = 2000;
		private const float CLOUD_HEIGHT = 13000;
		private int platformWidth;
		private const int MAX_OFFSET = 350;
		private const int EDGE_BUFFER = 100;
		private const int MIN_SIDESTEP_DISTANCE = 300;
		private const int PLATFORM_SPACING = 250;
		private const float CHANCE_BOUNCE = 0.1f;
		private const float CHANCE_MOVING = 0.1f;
		private const float CHANCE_SIDESTEP = 0.05f;
		private const float CHANCE_ENEMY = 0.1f;
		private const float ENEMY_SPACING = .6f;

		public enum PlatformType
		{
			Normal,
			Bounce,
			Moving,
			Enemy
		}
		public Platform topPlatform;
		private Vector2 nextPlatformPosition;

		public void Initialize(Vector2 startPosition, int platformWidth)
		{
			nextPlatformPosition = startPosition;
			this.platformWidth = platformWidth;
		}

		public List<Platform> SpawnPlatforms(float topY)
		{
			List<Platform> addedPlatforms = new();

			while (nextPlatformPosition.Y > topY - EDGE_BUFFER)
			{
				PlatformType platformType = PlatformType.Normal;
				if (nextPlatformPosition.Y < -BUILDING_HEIGHT)
				{
					if (Probability(CHANCE_BOUNCE)) platformType = PlatformType.Bounce;
					else if (Probability(CHANCE_MOVING)) platformType = PlatformType.Moving;
				}


				Platform p = SpawnNewPlatform(addedPlatforms, platformType, nextPlatformPosition);
				Vector2 previousPosition = p.Position;

				bool sideStep = (Probability(CHANCE_SIDESTEP)) && platformType != PlatformType.Moving && previousPosition.Y < -BUILDING_HEIGHT;
				if (sideStep)
				{
					int sideStepOffset = RandomOffset();
					if (MathF.Abs(sideStepOffset) <= MIN_SIDESTEP_DISTANCE)
					{
						sideStepOffset = MathF.Sign(sideStepOffset) * (MIN_SIDESTEP_DISTANCE + 1);
					}
					Vector2 sideStepPosition = new Vector2(previousPosition.X + sideStepOffset, previousPosition.Y);
					sideStepPosition = WrapAroundPositionX(sideStepPosition, previousPosition.X, MIN_SIDESTEP_DISTANCE * 2);
					Platform sidePlatform = SpawnNewPlatform(addedPlatforms, PlatformType.Bounce, sideStepPosition);
				}

				bool enemy = (Probability(CHANCE_ENEMY)) && platformType != PlatformType.Moving && previousPosition.Y < -BUILDING_HEIGHT;
				if (enemy)
				{
					int sideStepOffset = RandomOffset();
					if (MathF.Abs(sideStepOffset) <= MIN_SIDESTEP_DISTANCE)
					{
						sideStepOffset = MathF.Sign(sideStepOffset) * (MIN_SIDESTEP_DISTANCE + 1);
					}
					Vector2 enemyPosition = new Vector2(previousPosition.X, previousPosition.Y - PLATFORM_SPACING * ENEMY_SPACING);
					enemyPosition = WrapAroundPositionX(enemyPosition, previousPosition.X, MIN_SIDESTEP_DISTANCE * 2);
					Platform sidePlatform = SpawnNewPlatform(addedPlatforms, PlatformType.Enemy, enemyPosition);
				}

				switch (platformType)
				{
					case PlatformType.Bounce:
						nextPlatformPosition.Y -= PLATFORM_SPACING;
						break;
					case PlatformType.Moving:
						nextPlatformPosition.Y -= PLATFORM_SPACING / 2;
						break;
					default:
					case PlatformType.Normal:
						nextPlatformPosition.Y -= PLATFORM_SPACING;
						break;

				}

				int offset = RandomOffset();
				nextPlatformPosition.X += offset;
				nextPlatformPosition = WrapAroundPositionX(nextPlatformPosition, previousPosition.X, MAX_OFFSET);
			}
			return addedPlatforms;

		}

		private bool Probability(float chance)
		{
			return GameSettings.Random.NextSingle() < chance;
		}

		private int RandomOffset()
		{
			return GameSettings.Random.Next(MAX_OFFSET * 2) - MAX_OFFSET;
		}

		private Platform SpawnNewPlatform(List<Platform> addedPlatforms, PlatformType platformType, Vector2 position)
		{
			int maxPlatformX = GameSettings.GameWidth / 2 - platformWidth;
			Platform newPlatform;
			Texture2D texture;
			switch (platformType)
			{
				case PlatformType.Bounce:
					if (position.Y <= -CLOUD_HEIGHT)
					{
						texture = GameSettings.Assets.Textures["platform_cloud_bounce"];
					}
					else
					{
						texture = GameSettings.Assets.Textures["platform_bounce"];
					}
					newPlatform = new BouncePlatform(new SpriteSheet(texture));
					break;
				case PlatformType.Moving:
					if (position.Y <= -CLOUD_HEIGHT)
					{
						texture = GameSettings.Assets.Textures["platform_cloud_move"];
					}
					else
					{
						texture = GameSettings.Assets.Textures["platform_moving"];
					}
					newPlatform = new MovingPlatform(new SpriteSheet(texture));
					break;
				case PlatformType.Enemy:
					texture = GameSettings.Assets.Textures["platform_enemy"];
					newPlatform = new EnemyPlatform(new SpriteSheet(texture));
					break;
				default:
				case PlatformType.Normal:
					if (position.Y <= -CLOUD_HEIGHT)
					{
						texture = GameSettings.Assets.Textures["platform_cloud"];
					}
					else if (position.Y >= -BUILDING_HEIGHT)
					{
						texture = GameSettings.Assets.Textures["platform_ac"];
					}
					else
					{
						texture = GameSettings.Assets.Textures["platform"];
					}
					newPlatform = new Platform(new SpriteSheet(texture));
					break;

			}
			addedPlatforms.Add(newPlatform);
			newPlatform.Position = position;
			return newPlatform;
		}

		private Vector2 WrapAroundPositionX(Vector2 position, float previousX, int maxAllowedOffset)
		{
			int maxPlatformX = GameSettings.GameWidth / 2 - platformWidth;
			if (position.X < -maxPlatformX)
			{
				if (Math.Abs(-maxPlatformX - previousX) + platformWidth * 2 > maxAllowedOffset)
					position.X = -maxPlatformX;
				else
					position.X = maxPlatformX;
			}
			if (position.X > maxPlatformX)
			{
				if (Math.Abs(maxPlatformX - previousX) + platformWidth * 2 > maxAllowedOffset)
					position.X = maxPlatformX;
				else
					position.X = -maxPlatformX;
			}
			return position;
		}

		public List<Platform> GetBottomPlatformsToRemove(List<Platform> platforms, float bottomY)
		{
			List<Platform> removedPlatforms = new();
			foreach (var platform in platforms.ToList())
			{
				if (platform.Position.Y > bottomY + EDGE_BUFFER)
				{
					removedPlatforms.Add(platform);
				}
			}
			return removedPlatforms;
		}
	}
}
