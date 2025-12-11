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

		public Vector2 TopLeftPosition { get; set; }
		public Point Size { get; set; }     // Final on-screen size

		public int SpriteIndex { get; set; }

		public float Rotation { get; set; } = 0f;
		public float RotationSpeed { get; set; } = 0f;

		public Point SpriteSize =>
			new Point(Texture.Width / Columns, Texture.Height / Rows);

		public Rectangle DestinationRectangle =>
			new Rectangle(
				(int)TopLeftPosition.X,
				(int)TopLeftPosition.Y,
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

		public SpriteSheet(Texture2D texture, int rows, int columns)
		{
			Texture = texture;
			Rows = rows;
			Columns = columns;
			TopLeftPosition = Vector2.Zero;

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
				Vector2.Zero,
				SpriteEffects.None,
				0f
			);
		}

		public Rectangle HitBoxRectangle => DestinationRectangle;
	}
}
