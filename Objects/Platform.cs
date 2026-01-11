using DoodleJump.Core;
using DoodleJump.Hierarchy;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Objects
{
	public class Platform : GameObject
	{
		public float BounceForce;
		public float jumpOffsetTimer = 0;
		public const float JumpOffsetDuration = 0.5f;
		public Vector2 OffsetPosition
		{
			get; set;
		} = Vector2.Zero;
		public Vector2 OffsetDirection
		{
			get; set;
		} = Vector2.Zero;
		public Platform(SpriteSheet visualization) : base(visualization)
		{
			this.Visualization.Scale = 4;
			this.BounceForce = 21f;
		}
		public virtual void Bounce(Player player)
		{
			jumpOffsetTimer = JumpOffsetDuration;
			OffsetDirection = Vector2.Normalize(new Vector2(player.Velocity.X, -30));
			PlayBounceSound();
		}
		internal virtual void PlayBounceSound()
		{
			float pitch = (GameSettings.Random.NextSingle() - .5f) * .5f + .0f;
			int soundIndex = GameSettings.Random.Next(0, 2);
			string soundName = new string[] { "brick1", "brick2" }[soundIndex];
			GameSettings.Assets.Sounds[soundName].Play(.2f, pitch, 0f);
		}
		public override void Update(float dt)
		{
			if (jumpOffsetTimer > 0)
			{
				jumpOffsetTimer -= dt;
				float progress = 1 - (jumpOffsetTimer / JumpOffsetDuration);
				float offsetIntensity = -(float)(Math.Sin(progress * Math.PI) * 30);
				OffsetPosition = OffsetDirection * offsetIntensity;
			}
			else
			{
				OffsetPosition = Vector2.Zero;
			}
			base.Update(dt);
		}
		public override void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			Vector2 initialPosition = this.Position;
			Visualization.Position = this.Position + OffsetPosition;
			base.Draw(spriteBatch, polygonDrawer);
			Visualization.Position = initialPosition;
		}
	}
}
