using DoodleJump.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Rendering
{
	public class Camera
	{
		public Vector2 Position { get; set; } = Vector2.Zero;
		public float Zoom { get; set; } = 1.0f;
		public float Rotation { get; set; } = 0f;
		public Camera() { }

		public Matrix GetViewMatrix(bool offsetCenter)
		{
			Matrix view = Matrix.CreateTranslation(new Vector3(-Position, 0)) *
				Matrix.CreateRotationZ(Rotation) *
				Matrix.CreateScale(Zoom, Zoom, 1);

			if (offsetCenter)
				view *= Matrix.CreateTranslation(new Vector3(GameSettings.WindowWidth/ 2f, GameSettings.WindowHeight / 2f, 0));

			return view;
		}
	}
}
