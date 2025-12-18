using DoodleJump.Core;
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

	public abstract class GameObject
	{
		public SpriteSheet Visualization { get; set; }

		public Vector2 Position
		{
			get => Visualization.Position;
			set => Visualization.Position = value;
		}

		public Point Size => Visualization.Size;

		public bool IsActive { get; set; } = true;

		public Vector2 Velocity { get; set; } = Vector2.Zero;

		public bool IsOutOfBounds
		{
			get
			{
				return false;
				//TODO temp fix
				Rectangle hb = Visualization.HitBoxRectangle;
				int w = GameSettings.WindowWidth;
				int h = GameSettings.WindowHeight;

				// Completely to the left, right, above or below the window
				if (hb.Right < 0) return true;
				if (hb.Left > w) return true;
				if (hb.Bottom < 0) return true;
				if (hb.Top > h) return true;

				return false;
			}
		}
		public float InitialRotation { get; set; } = 0f;

		protected GameObject(SpriteSheet visualization)
		{
			Visualization = visualization;
		}
		public Vector2 Forward
		{
			get
			{
				float angle = Visualization.Rotation + InitialRotation;
				return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
			}
		}

		public virtual void Update(float dt)
		{
			if (!IsActive)
				return;

			MoveGameObject(dt);

			if (IsOutOfBounds)
				IsActive = false;

			Visualization.Update(dt);
		}

		public virtual void MoveGameObject(float dt)
		{
			Position += Velocity * dt;
		}

		public virtual void Draw(SpriteBatch spriteBatch, PolygonDrawer polygonDrawer)
		{
			if (!IsActive)
				return;

			Visualization.Draw(spriteBatch);
			if (GameSettings.DebugDraw)
			{
				Rectangle hitboxUnrotated = Visualization.HitBoxRectangle;
				polygonDrawer.DrawRectangle(hitboxUnrotated.Location.ToVector2(), hitboxUnrotated.Size.ToVector2(), Color.Yellow, 1);
				polygonDrawer.DrawCircle(Position, 5, Color.Yellow, 2);
			}
		}

		public bool IsCollidingWith(GameObject other)
		{
			if (other == null) return false;
			if (!IsActive || !other.IsActive) return false;

			return Visualization.HitBoxRectangle.Intersects(other.Visualization.HitBoxRectangle);
		}
	}
}
