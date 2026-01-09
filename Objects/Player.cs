using DoodleJump.Core;
using DoodleJump.Hierarchy;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DoodleJump.Objects
{
	public class Player : GameObject
	{
		private bool grounded = false;
		private Platform standingOn;
		public float maxHeight = 0f;
		private Platform lastPlatform;
		public float timeSinceJump = 0f;
		private LineTrail trail = new LineTrail(color: Color.OrangeRed, maxPoints:30);
		//public Player() : base(new SpriteSheetAnimation(GameSettings.Assets.Textures["fire_circles_100x100"], 8, 8, 0, 59, 0))
		public Player() : base(new SpriteSheetAnimation(GameSettings.Assets.Textures["cat_jump"], 1, 3, 0, 2, 0))
		{
			(this.Visualization as SpriteSheetAnimation).IsAnimationStopped = true;
			this.Visualization.Scale = 4;
		}
		public bool IsStandingOn(GameObject obj, float dt)
		{
			Rectangle hb1 = this.Visualization.HitBoxRectangle;
			Rectangle hb2 = obj.Visualization.HitBoxRectangle;
			bool willFallThrough;
			if (hb1.Bottom < hb2.Top && hb1.Bottom + Velocity.Y * dt * 120 >= hb2.Top)
			{
				willFallThrough = true;
			}
			else
			{
				willFallThrough = false;
			}
			if (willFallThrough || (hb1.Bottom <= hb2.Top && hb1.Bottom + 10 >= hb2.Top))
			{
				if (hb1.Right > hb2.Left && hb1.Left < hb2.Right)
				{
					return true;
				}
			}
			return false;
		}
		public void CheckPlatformCollisions(IEnumerable<Platform> platforms, float dt)
		{
			foreach (var platform in platforms)
			{
				if (IsStandingOn(platform, dt) && Velocity.Y > 0)
				{
					standingOn = platform;
					grounded = true;
					return;
				}
			}
			grounded = false;
			standingOn = null;
			if(Position.Y > 0)
			{
				grounded = true;
				standingOn = platforms.First();
			}
		}
		public override void Update(float dt)
		{
			Vector2 vel = this.Velocity;
			if (grounded)
			{
				vel.Y = -standingOn.BounceForce;
				standingOn.Bounce(this);
				timeSinceJump = 0f;
				if (lastPlatform != null && lastPlatform != standingOn)
				{
					GameSettings.PlatformStreak++;
				}
				else
				{
					GameSettings.PlatformStreak = 0;
				}
				lastPlatform = standingOn;

			}
			else
			{
				vel.Y += dt * 45f;
				timeSinceJump += dt;
			}
			float speedX = 7f;
			if (Input.IsKeyDown(Keys.A))
			{
				vel.X = -speedX;
			}
			else if (Input.IsKeyDown(Keys.D))
			{
				vel.X = speedX;
			}
			else
			{
				vel.X *= 1f - dt * 5f;
			}
			//(this.Visualization as SpriteSheetAnimation).AnimationFPS = -vel.X * 15f;
			float rotation = MathF.Atan2(Velocity.X, -Velocity.Y);
			if (MathF.Abs(rotation) < .8f)
			{
				(this.Visualization as SpriteSheetAnimation).IdleSpriteIndex = 2;
				this.Visualization.Flip = vel.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			}
			else if (MathF.Abs(rotation) >= MathF.PI - .5f)
			{
				(this.Visualization as SpriteSheetAnimation).IdleSpriteIndex = 1;
				rotation += MathF.PI;
				this.Visualization.Flip = vel.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			}
			else
			{
				(this.Visualization as SpriteSheetAnimation).IdleSpriteIndex = 0;
				rotation -= MathF.PI / 2f;
				this.Visualization.Flip = vel.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			}
			this.Visualization.Rotation = rotation;
			this.Visualization.Color = ColorUtilities.StreakColor(GameSettings.PlatformStreak);
			this.Velocity = vel;
			MoveGameObject(dt);

			Vector2 pos = this.Position;
			bool edgeWrapped = false;
			if (this.Position.X < -GameSettings.GameWidth / 2)
			{
				pos.X = GameSettings.GameWidth / 2;
				edgeWrapped = true;
			}
			if (this.Position.X > GameSettings.GameWidth / 2)
			{
				pos.X = -GameSettings.GameWidth / 2;
				edgeWrapped = true;
			}
			this.Position = pos;
			if (-this.Position.Y > maxHeight)
			{
				maxHeight = -this.Position.Y;
			}
			Visualization.Update(dt);
			trail.Update(this.Position, ColorUtilities.StreakColor(GameSettings.PlatformStreak), edgeWrapped);

			if (this.Position.Y > GameSettings.PlayScreen.Camera.Position.Y + GameSettings.WindowHeight / 2f + 100)
			{
				//Die
				GameSettings.EndScreen.Initialize();
			}
			else if (this.Position.Y < GameSettings.PlayScreen.Camera.Position.Y - GameSettings.WindowHeight / 2f + 300)
			{
				GameSettings.PlayScreen.Camera.Position = new Vector2(GameSettings.PlayScreen.Camera.Position.X, this.Position.Y + GameSettings.WindowHeight / 2f - 300);
			}
		}
		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			trail.Draw(polygonDrawer);
			base.Draw(spriteBatch, polygonDrawer);
		}
	}
}
