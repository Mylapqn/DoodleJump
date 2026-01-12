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
	internal class Player : GameObject
	{
		//Animation & visuals
		private const float ANIMATION_FPS = 8f;
		private const float INITIAL_Y_OFFSET = 5f;
		private const int SPRITE_IDLE_INDEX = 11;

		//Trail
		private const int TRAIL_MAX_POINTS = 30;

		//Collision
		private const float COLLISION_PREDICTION_SPEED = 100f;
		private const int COLLISION_PIXEL_TOLERANCE = 1;

		//Movement & physics
		private const float AIR_SPEED_X = 8f;
		private const float WALK_SPEED_X = 5f;
		private const float AIR_FRICTION = 5f;
		private const float WALK_FRICTION = 10f;
		private const float NORMAL_GRAVITY = 45f;
		private const float POWERUP_GRAVITY = 10f;
		private const float ANIMATION_SPEED_MULTIPLIER = 3f;

		//Jumping / bouncing
		private const float POWERUP_BOUNCE_MULTIPLIER = 1.5f;

		//Streak system
		private const int STREAK_POWERUP_THRESHOLD = 10;

		//Sound
		private const float SOUND_VOLUME_OTHER = 0.15f;
		private const float SOUND_VOLUME_MEOW = 0.2f;
		private const float POPUP_LIFETIME = 1f;

		//Rotation
		private const float ROTATION_FORWARD_THRESHOLD = 0.8f;
		private const float ROTATION_INVERT_THRESHOLD = 0.5f;

		//Camera & bounds
		private const float DEATH_CAMERA_BUFFER = 100f;

		//Intro animation
		private const float INTRO_MOVE_INPUT = 1.1f;
		private const float INTRO_CANCEL_X = -40f;

		private bool grounded = false;
		private Platform standingOn;
		private Platform lastPlatform;
		public float timeSinceJump = 0f;
		private LineTrail trail = new LineTrail(color: Color.OrangeRed, maxPoints: TRAIL_MAX_POINTS);
		public bool walking = true;
		private bool introAnimation = true;
		public bool poweredUp = false;
		private TextPopupManager SceneTextPopupManager { get; set; }

		public Player(SpriteSheetAnimation anim, TextPopupManager textPopupManager)
			: base(anim)
		{
			anim.IsAnimationStopped = false;
			anim.AnimationFPS = ANIMATION_FPS;
			Visualization.Scale = GameSettings.PIXEL_SCALE;
			Position = new Vector2(-GameSettings.GameWidth / 2, -Size.Y / 2 - INITIAL_Y_OFFSET);
			SceneTextPopupManager = textPopupManager;
		}

		public bool IsStandingOn(GameObject obj, float dt)
		{
			Rectangle hb1 = Visualization.HitBoxRectangle;
			Rectangle hb2 = obj.Visualization.HitBoxRectangle;

			bool willFallThrough =
				hb1.Bottom < hb2.Top &&
				hb1.Bottom + Velocity.Y * dt * COLLISION_PREDICTION_SPEED >= hb2.Top;

			bool isCurrentlyOn =
				hb1.Bottom - COLLISION_PIXEL_TOLERANCE <= hb2.Top &&
				hb1.Bottom + COLLISION_PIXEL_TOLERANCE >= hb2.Top;

			bool leftRightOverlap = hb1.Right > hb2.Left && hb1.Left < hb2.Right;

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
		}

		public override void Update(float dt)
		{
			Vector2 vel = Velocity;
			Vector2 pos = Position;

			float speedX = AIR_SPEED_X;
			float friction = AIR_FRICTION;
			float gravity = poweredUp ? POWERUP_GRAVITY : NORMAL_GRAVITY;

			if (grounded)
			{
				if (walking)
				{
					speedX = WALK_SPEED_X;
					friction = WALK_FRICTION;
					Visualization.Rotation = 0;
					Visualization.Flip = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
					vel.Y = 0f;

					var anim = Visualization as SpriteSheetAnimation;
					if (MathF.Abs(vel.X) <= 0.1f)
					{
						anim.IdleSpriteIndex = SPRITE_IDLE_INDEX;
						anim.IsAnimationStopped = true;
					}
					else
					{
						anim.AnimationFPS = MathF.Abs(Velocity.X * ANIMATION_SPEED_MULTIPLIER);
						anim.IsAnimationStopped = false;
					}

					if (Input.IsKeyDown(Keys.Space))
					{
						walking = false;
						introAnimation = false;
						anim.IsAnimationStopped = true;
						GameSettings.Assets.Sounds["meow"].Play(SOUND_VOLUME_MEOW, 0, 0f);
					}
				}
				else
				{
					float bounceMultiplier = poweredUp ? POWERUP_BOUNCE_MULTIPLIER : 1f;
					vel.Y = -standingOn.BounceForce * bounceMultiplier;
					standingOn.Bounce(this);
					timeSinceJump = 0f;

					if (lastPlatform != null && lastPlatform != standingOn)
					{
						GameSettings.PlatformStreak++;
						GameSettings.MaxPlatformStreak = Math.Max(GameSettings.MaxPlatformStreak, GameSettings.PlatformStreak);

						if (GameSettings.PlatformStreak % STREAK_POWERUP_THRESHOLD == 0)
						{
							GameSettings.Assets.Sounds["meow"].Play(SOUND_VOLUME_MEOW, 0, 0f);
							GameSettings.Assets.Sounds["success"].Play(SOUND_VOLUME_OTHER, 0.2f, 0f);
							poweredUp = true;

							SceneTextPopupManager.AddPopup(
								new TextPopup(
									Position,
									$"{GameSettings.PlatformStreak} STREAK!",
									ColorUtilities.StreakColor(GameSettings.PlatformStreak),
									POPUP_LIFETIME));
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
							GameSettings.Assets.Sounds["fail"].Play(SOUND_VOLUME_OTHER, -0.2f, 0f);
							SceneTextPopupManager.AddPopup(
								new TextPopup(
									Position,
									"LOST STREAK",
									Color.LightGray,
									POPUP_LIFETIME));
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

			Color color = poweredUp
				? ColorUtilities.HSV(GameSettings.ElapsedGameTime % 1f, 1f, 1f)
				: ColorUtilities.StreakColor(GameSettings.PlatformStreak);

			Visualization.Color = color;
			Position = pos;
			MoveGameObject(dt);

			bool edgeWrapped = DoEdgeWrap();

			if (-Position.Y > GameSettings.MaxHeightReached)
				GameSettings.MaxHeightReached = -Position.Y;

			Visualization.Update(dt);
			if (!walking)
				trail.Update(Position, color, edgeWrapped);

			if (Position.Y > GameSettings.PlayScreen.Camera.Position.Y + GameSettings.WindowHeight / 2f / GameSettings.PlayScreen.Camera.Zoom + DEATH_CAMERA_BUFFER)
			{
				//Die
				GameSettings.EndScreen.Initialize();
			}
		}

		private void DoHorizontalMovement(float dt, float horizontalInput, float speedX, float friction)
		{
			Vector2 vel = Velocity;
			if (horizontalInput != 0)
				vel.X = horizontalInput * speedX;
			else
				vel.X *= 1f - dt * friction;

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
			var anim = Visualization as SpriteSheetAnimation;

			if (MathF.Abs(rotation) < ROTATION_FORWARD_THRESHOLD)
			{
				anim.IdleSpriteIndex = 2;
				Visualization.Flip = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			}
			else if (MathF.Abs(rotation) >= MathF.PI - ROTATION_INVERT_THRESHOLD)
			{
				anim.IdleSpriteIndex = 1;
				rotation += MathF.PI;
				Visualization.Flip = Velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			}
			else
			{
				anim.IdleSpriteIndex = 0;
				rotation -= MathF.PI / 2f;
				Visualization.Flip = Velocity.X < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			}

			Visualization.Rotation = rotation;
		}

		private bool DoEdgeWrap()
		{
			bool wrapped = false;
			Vector2 pos = Position;

			float halfWidth = GameSettings.GameWidth / 2f;

			if (Position.X < -halfWidth)
			{
				pos.X = halfWidth;
				wrapped = true;
			}
			if (Position.X > halfWidth)
			{
				pos.X = -halfWidth;
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
				horizontalInput = INTRO_MOVE_INPUT;
				if (Input.IsKeyDown(Keys.A) ||
					Input.IsKeyDown(Keys.D) ||
					Input.IsKeyDown(Keys.Space) ||
					Position.X > INTRO_CANCEL_X)
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