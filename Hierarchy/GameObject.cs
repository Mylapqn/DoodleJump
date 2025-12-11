using DoodleJump.Core;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Hierarchy
{

	public abstract class GameObject
	{
		public PlayScreen Screen { get; set; }
		public SpriteSheet Visualization { get; set; }

		public Vector2 TopLeftPosition
		{
			get => Visualization.TopLeftPosition;
			set => Visualization.TopLeftPosition = value;
		}

		public Point Size => Visualization.Size;

		public bool IsActive { get; set; } = true;

		public Vector2 Velocity { get; set; } = Vector2.Zero;

		public bool IsOutOfBounds
		{
			get
			{
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

		protected GameObject(SpriteSheet visualization, PlayScreen screen)
		{
			Visualization = visualization;
			Screen = screen;
			screen.GameObjects.Add(this);
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

			MoveGameObject();

			if (IsOutOfBounds)
				IsActive = false;

			Visualization.Update(dt);
		}

		public virtual void MoveGameObject()
		{
			TopLeftPosition += Velocity;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			if (!IsActive)
				return;

			Visualization.Draw(spriteBatch);
		}

		public bool IsCollidingWith(GameObject other)
		{
			if (other == null) return false;
			if (!IsActive || !other.IsActive) return false;

			return Visualization.HitBoxRectangle.Intersects(other.Visualization.HitBoxRectangle);
		}

		public virtual void Destroy()
		{
			IsActive = false;
			Screen.GameObjects.Remove(this);
			Screen = null;
		}
	}
}
