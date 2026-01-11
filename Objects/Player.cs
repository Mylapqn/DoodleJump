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
		private Platform lastPlatform;
		public float timeSinceJump = 0f;
		private LineTrail trail = new LineTrail(color: Color.OrangeRed, maxPoints: 30);
		public bool walking = true;
		private bool introAnimation = true;
		public bool poweredUp = false;
		private TextPopupManager SceneTextPopupManager { get; set; }
		//public Player() : base(new SpriteSheetAnimation(GameSettings.Assets.Textures["fire_circles_100x100"], 8, 8, 0, 59, 0))
		public Player(TextPopupManager textPopupManager) : base(new SpriteSheetAnimation(GameSettings.Assets.Textures["cat_jump"], 2, 8, 3, 10, 11))
		{
			(Visualization as SpriteSheetAnimation).IsAnimationStopped = false;
			(Visualization as SpriteSheetAnimation).AnimationFPS = 8;
			Visualization.Scale = 4;
			Position = new Vector2(-GameSettings.WindowWidth / 2, -Size.Y / 2 - 5);
			this.SceneTextPopupManager = textPopupManager;
		}
		public bool IsStandingOn(GameObject obj, float dt)
		{
			Rectangle hb1 = Visualization.HitBoxRectangle;
			Rectangle hb2 = obj.Visualization.HitBoxRectangle;
			bool willFallThrough = (hb1.Bottom < hb2.Top && hb1.Bottom + Velocity.Y * dt * 120 >= hb2.Top);
			bool isCurrentlyOn = (hb1.Bottom - 1 <= hb2.Top && hb1.Bottom + 1 >= hb2.Top);
			bool leftRightOverlap = (hb1.Right > hb2.Left && hb1.Left < hb2.Right);

			return leftRightOverlap && (isCurrentlyOn || willFallThrough);


		}
		public void CheckPlatformCollisions(IEnumerable<Platform> platforms, float dt)
		{
			foreach (var platform in platforms)
			{
				if (IsStandingOn(platform, dt) && Velocity.Y >= 0)
				{
					standingOn = platform;
					grounded = true;
					return;
				}
			}
			grounded = false;
			standingOn = null;
			/*if (Position.Y > -platforms.First().Size.Y * 1.5f)
			{
				grounded = true;
				standingOn = platforms.First();
			}*/
		}
		public override void Update(float dt)
		{
			Vector2 vel = Velocity;
			Vector2 pos = Position;

			float speedX = 8f;
			float friction = 5f;
			float gravity = poweredUp ? 10f : 45f;



			if (grounded)
			{
				//pos.Y = standingOn.Position.Y - this.Size.Y / 2 - standingOn.Size.Y / 2;
				if (walking)
				{
					speedX = 5f;
					friction = 10f;
					Visualization.Rotation = 0;
					Visualization.Flip = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
					vel.Y = 0f;
					if (MathF.Abs(vel.X) <= .1f)
					{
						(Visualization as SpriteSheetAnimation).IdleSpriteIndex = 11;
						(Visualization as SpriteSheetAnimation).IsAnimationStopped = true;
					}
					else
					{
						(Visualization as SpriteSheetAnimation).AnimationFPS = MathF.Abs(Velocity.X * 3);
						(Visualization as SpriteSheetAnimation).IsAnimationStopped = false;
					}
					//(this.Visualization as SpriteSheetAnimation).AnimationFPS = 12f;
					if (Input.IsKeyDown(Keys.Space))
					{
						walking = false;
						introAnimation = false;
						(Visualization as SpriteSheetAnimation).IsAnimationStopped = true;
						GameSettings.Assets.Sounds["meow"].Play(.2f, 0, 0f);
					}
				}
				else
				{
					float bounceMultiplier = poweredUp ? 1.5f : 1f;
					vel.Y = -standingOn.BounceForce * bounceMultiplier;
					standingOn.Bounce(this);
					timeSinceJump = 0f;
					if (lastPlatform != null && lastPlatform != standingOn)
					{
						GameSettings.PlatformStreak++;
						GameSettings.MaxPlatformStreak = Math.Max(GameSettings.MaxPlatformStreak, GameSettings.PlatformStreak);
						if (GameSettings.PlatformStreak % 10 == 0)
						{
							GameSettings.Assets.Sounds["meow"].Play(.2f, 0, 0f);
							GameSettings.Assets.Sounds["success"].Play(.15f, .2f, 0f);
							poweredUp = true;
							SceneTextPopupManager.AddPopup(
								new TextPopup(Position,
									$"{GameSettings.PlatformStreak} STREAK!",
									ColorUtilities.StreakColor(GameSettings.PlatformStreak),
									1f
									)
								);
						}
						else
						{
							poweredUp = false;
						}

					}
					else
					{
						if (GameSettings.PlatformStreak > 0)
						{
							GameSettings.Assets.Sounds["fail"].Play(.15f, -.2f, 0f);
							SceneTextPopupManager.AddPopup(
									new TextPopup(Position,
										$"LOST STREAK",
										Color.LightGray,
										.5f
										)
									);

						}
						GameSettings.PlatformStreak = 0;
						poweredUp = false;
					}
					lastPlatform = standingOn;
				}

			}
			else
			{
				vel.Y += dt * gravity;
				timeSinceJump += dt;
				SetRotationFromVelocity();
			}

			Velocity = vel;

			float horizontalInput = ProcessInput();
			DoHorizontalMovement(dt, horizontalInput, speedX, friction);
			Color color = ColorUtilities.StreakColor(GameSettings.PlatformStreak);
			if (poweredUp)
			{
				color = ColorUtilities.HSV(GameSettings.ElapsedGameTime * 1f % 1f, 1f, 1f);
			}
			Visualization.Color = color;
			Position = pos;
			MoveGameObject(dt);

			bool edgeWrapped = DoEdgeWrap();

			if (-Position.Y > GameSettings.MaxHeightReached)
			{
				GameSettings.MaxHeightReached = -Position.Y;
			}
			Visualization.Update(dt);
			if (!walking) trail.Update(Position, color, edgeWrapped);

			if (Position.Y > GameSettings.PlayScreen.Camera.Position.Y + GameSettings.WindowHeight / 2f + 100)
			{
				//Die
				GameSettings.EndScreen.Initialize();
			}
			else if (Position.Y < GameSettings.PlayScreen.Camera.Position.Y - GameSettings.WindowHeight / 2f + 300)
			{
				GameSettings.PlayScreen.Camera.Position = new Vector2(GameSettings.PlayScreen.Camera.Position.X, Position.Y + GameSettings.WindowHeight / 2f - 300);
			}
		}

		private void DoHorizontalMovement(float dt, float horizontalInput, float speedX, float friction)
		{
			Vector2 vel = Velocity;
			if (horizontalInput != 0)
			{
				vel.X = horizontalInput * speedX;
			}
			else
			{
				vel.X *= 1f - dt * friction;
			}
			Velocity = vel;
		}

		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			trail.Draw(polygonDrawer);
			base.Draw(spriteBatch, polygonDrawer);
		}

		private void SetRotationFromVelocity()
		{
			float rotation = MathF.Atan2(Velocity.X, -Velocity.Y);
			if (MathF.Abs(rotation) < .8f)
			{
				(Visualization as SpriteSheetAnimation).IdleSpriteIndex = 2;
				Visualization.Flip = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			}
			else if (MathF.Abs(rotation) >= MathF.PI - .5f)
			{
				(Visualization as SpriteSheetAnimation).IdleSpriteIndex = 1;
				rotation += MathF.PI;
				Visualization.Flip = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			}
			else
			{
				(Visualization as SpriteSheetAnimation).IdleSpriteIndex = 0;
				rotation -= MathF.PI / 2f;
				Visualization.Flip = Velocity.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			}
			Visualization.Rotation = rotation;
		}
		private bool DoEdgeWrap()
		{
			bool wrapped = false;
			Vector2 pos = Position;
			if (Position.X < -GameSettings.GameWidth / 2)
			{
				pos.X = GameSettings.GameWidth / 2;
				wrapped = true;
			}
			if (Position.X > GameSettings.GameWidth / 2)
			{
				pos.X = -GameSettings.GameWidth / 2;
				wrapped = true;
			}
			Position = pos;
			return wrapped;
		}
		private float ProcessInput()
		{
			float horizontalInput = 0f;
			if (introAnimation)
			{
				horizontalInput = 1.1f;
				if (Input.IsKeyDown(Keys.A) || Input.IsKeyDown(Keys.D) || Input.IsKeyDown(Keys.Space) || Position.X > -40)
				{
					introAnimation = false;
				}
			}
			else if (Input.IsKeyDown(Keys.A))
			{
				horizontalInput = -1f;
			}
			else if (Input.IsKeyDown(Keys.D))
			{
				horizontalInput = 1f;
			}
			return horizontalInput;
		}
	}

}