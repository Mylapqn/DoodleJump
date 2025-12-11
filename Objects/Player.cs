using DoodleJump.Core;
using DoodleJump.Hierarchy;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DoodleJump.Objects
{
	internal class Player : GameObject
	{
		public Player() : base(new SpriteSheetAnimation(GameSettings.Assets.Textures["fire_circles_100x100"], 8, 8, 0, 59, 0))
		{
			(this.Visualization as SpriteSheetAnimation).AnimationDelayInFrames = 1;
			this.Visualization.Scale = 1;
		}
		public bool IsStandingOn(GameObject obj)
		{
			Rectangle hb1 = this.Visualization.HitBoxRectangle;
			Rectangle hb2 = obj.Visualization.HitBoxRectangle;
			bool willFallThrough;
			if(hb1.Bottom < hb2.Top && hb1.Bottom + Velocity.Y >= hb2.Top)
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
		public override void Update(float dt)
		{
			bool grounded = false;
			Vector2 vel = this.Velocity;
			foreach (var obj in GameSettings.PlayScreen.GameObjects)
			{
				if (obj == this)
					continue;
				if (IsStandingOn(obj) && vel.Y > 0)
				{
					vel.Y = 0;
					grounded = true;
					break;
				}
			}
			if (grounded)
			{
				vel.Y = -20f;
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
			(this.Visualization as SpriteSheetAnimation).AnimationFPS = -vel.X * 15f;
			this.Velocity = vel;
			MoveGameObject();
			Visualization.Update(dt);

			if(this.Position.Y > GameSettings.PlayScreen.Camera.Position.Y + GameSettings.WindowHeight / 2f + 100)
			{
				GameSettings.EndScreen.Initialize();
			}
			if (this.Position.Y < GameSettings.PlayScreen.Camera.Position.Y - GameSettings.WindowHeight / 2f + 300)
			{
				GameSettings.PlayScreen.Camera.Position = new Vector2(GameSettings.PlayScreen.Camera.Position.X, this.Position.Y + GameSettings.WindowHeight / 2f - 300);
			}
		}
	}
}
