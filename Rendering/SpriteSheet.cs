using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Rendering
{
	public class SpriteSheet
	{
		public Texture2D Texture { get; }

		public int Rows { get; }
		public int Columns { get; }

		public Vector2 Position { get; set; }
		public Vector2 Origin { get; set; } = new Vector2(0.5f, 0.5f);
		public Point Size { get; set; }     // Final on-screen size
		public float Scale
		{
			get => (float)Size.X / SpriteSize.X;
			set => Size = new Point((int)(SpriteSize.X * value), (int)(SpriteSize.Y * value));
		}

		public int SpriteIndex { get; set; }

		public float Rotation { get; set; } = 0f;
		public float RotationSpeed { get; set; } = 0f;

		public Point SpriteSize =>
			new Point(Texture.Width / Columns, Texture.Height / Rows);

		public Rectangle DestinationRectangle =>
			new Rectangle(
				(int)(Position.X),
				(int)(Position.Y),
				Size.X,
				Size.Y
			);

		public Rectangle SourceRectangle
		{
			get
			{
				int spriteWidth = SpriteSize.X;
				int spriteHeight = SpriteSize.Y;

				int row = SpriteIndex / Columns;
				int col = SpriteIndex % Columns;

				return new Rectangle(
					col * spriteWidth,
					row * spriteHeight,
					spriteWidth,
					spriteHeight
				);
			}
		}

		public SpriteSheet(Texture2D texture)
			: this(texture, 1, 1)
		{
		}

		public SpriteSheet(Texture2D texture, int rows, int columns)
		{
			Texture = texture;
			Rows = rows;
			Columns = columns;
			Position = Vector2.Zero;

			Size = new Point(Texture.Width / columns, Texture.Height / rows);
		}

		public virtual void Update(float dt)
		{
			Rotation += RotationSpeed * dt;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(
				Texture,
				DestinationRectangle,
				SourceRectangle,
				Color.White,
				Rotation,
				new Vector2(SpriteSize.X * Origin.X, SpriteSize.Y * Origin.Y),
				SpriteEffects.None,
				0f
			);
		}

		public Rectangle HitBoxRectangle
		{
			get
			{
				Rectangle destRect = DestinationRectangle;
				return new Rectangle(
					destRect.X - (int)(Size.X * Origin.X),
					destRect.Y - (int)(Size.Y * Origin.Y),
					destRect.Width,
					destRect.Height
				);
			}
		}
	}
}
