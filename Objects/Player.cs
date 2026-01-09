using DoodleJump.Core;
using DoodleJump.Hierarchy;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace DoodleJump.Objects
{
	public class Player : GameObject
	{
		private bool grounded = false;
		private Platform standingOn;
		public float maxHeight = 0f;
		public Player() : base(new SpriteSheetAnimation(GameSettings.Assets.Textures["fire_circles_100x100"], 8, 8, 0, 59, 0))
		{
			(this.Visualization as SpriteSheetAnimation).AnimationDelayInFrames = 1;
			this.Visualization.Scale = 1;
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
		}
		public override void Update(float dt)
		{
			Vector2 vel = this.Velocity;
			if (grounded)
			{
				vel.Y = -standingOn.BounceForce;
				standingOn.Bounce(this);
			}
			else
			{
				vel.Y += dt * 45f;
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
			this.Visualization.RotationSpeed = vel.X * 0.5f;
			this.Velocity = vel;
			MoveGameObject(dt);

			Vector2 pos = this.Position;
			if (this.Position.X < -GameSettings.GameWidth / 2) pos.X = GameSettings.GameWidth / 2;
			if (this.Position.X > GameSettings.GameWidth / 2) pos.X = -GameSettings.GameWidth / 2;
			this.Position = pos;
			if (-this.Position.Y > maxHeight)
			{
				maxHeight = -this.Position.Y;
			}
			Visualization.Update(dt);

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
	}
}
