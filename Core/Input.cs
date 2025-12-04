using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoodleJump.Core
{

	public static class Input
	{
		public enum MouseButton
		{
			Left,
			Right,
			Middle
		}
		private static KeyboardState currentKeyboardState;
		private static KeyboardState previousKeyboardState;

		private static MouseState currentMouseState;
		private static MouseState previousMouseState;

		public static void Update()
		{
			// Update previous states
			previousKeyboardState = currentKeyboardState;
			previousMouseState = currentMouseState;

			// Capture current states
			currentKeyboardState = Keyboard.GetState();
			currentMouseState = Mouse.GetState();
		}

		// Keyboard helpers
		public static bool IsKeyPressed(Keys key)
		{
			return currentKeyboardState.IsKeyDown(key) &&
				   !previousKeyboardState.IsKeyDown(key);
		}

		public static bool IsKeyReleased(Keys key)
		{
			return !currentKeyboardState.IsKeyDown(key) &&
				   previousKeyboardState.IsKeyDown(key);
		}

		public static bool IsKeyDown(Keys key)
		{
			return currentKeyboardState.IsKeyDown(key);
		}

		public static bool IsKeyUp(Keys key)
		{
			return currentKeyboardState.IsKeyUp(key);
		}

		private static bool GetMouseButtonState(MouseButton button, MouseState state)
		{
			ButtonState buttonState;
			switch (button)
			{
				case MouseButton.Left:
					buttonState = state.LeftButton;
					break;
				case MouseButton.Right:
					buttonState = state.RightButton;
					break;
				case MouseButton.Middle:
					buttonState = state.MiddleButton;
					break;
				default:
					buttonState = ButtonState.Released;
					break;
			}
			return buttonState == ButtonState.Pressed;
		}

		public static bool IsMouseButtonDown(MouseButton button)
		{
			return GetMouseButtonState(button, currentMouseState);
		}

		public static bool IsMouseButtonReleased(MouseButton button)
		{
			return GetMouseButtonState(button, currentMouseState) &&
				   GetMouseButtonState(button, previousMouseState);
		}

		public static bool IsMouseButtonPressed(MouseButton button)
		{
			return GetMouseButtonState(button, currentMouseState) &&
				   GetMouseButtonState(button, previousMouseState);
		}

		public static Vector2 MousePosition
		{
			get { return new Vector2(currentMouseState.X, currentMouseState.Y); }
		}
	}
}
