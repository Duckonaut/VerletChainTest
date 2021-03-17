using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerletChainTest
{
	public class Input
	{
		public static KeyboardState CurrentState;
		public static KeyboardState PastState;

		public static MouseState CurrentMouseState;
		public static MouseState PastMouseState;

		public static bool Button1 => KeyboardClick(Keys.Q);
		public static bool Button1Down => CurrentState.IsKeyDown(Keys.Q);
		public static bool Button2 => KeyboardClick(Keys.E);
		public static bool Button2Down => CurrentState.IsKeyDown(Keys.E);

		public static bool Up => CurrentState.IsKeyDown(Keys.W);
		public static bool Down => CurrentState.IsKeyDown(Keys.S);
		public static bool LeftClick => KeyboardClick(Keys.A);
		public static bool Left => CurrentState.IsKeyDown(Keys.A);
		public static bool RightClick => KeyboardClick(Keys.D);
		public static bool Right => CurrentState.IsKeyDown(Keys.D);
		public static bool Jump => KeyboardClick(Keys.Space);
		public static bool JumpDown => CurrentState.IsKeyDown(Keys.Space);

		public static bool Back => KeyboardClick(Keys.Escape);

		public static Point MousePos => new Point(CurrentMouseState.Position.X, CurrentMouseState.Position.Y);
		public static bool MouseClick => CurrentMouseState.LeftButton == ButtonState.Pressed && PastMouseState.LeftButton == ButtonState.Released;
		public static bool RightMouseClick => CurrentMouseState.RightButton == ButtonState.Pressed && PastMouseState.RightButton == ButtonState.Released;
		public static bool MouseHold => CurrentMouseState.LeftButton == ButtonState.Pressed;
		public static bool ScrollDown => CurrentMouseState.ScrollWheelValue > PastMouseState.ScrollWheelValue;
		public static bool ScrollUp => CurrentMouseState.ScrollWheelValue < PastMouseState.ScrollWheelValue;

		public static bool KeyboardClick(Keys key) => CurrentState.IsKeyDown(key) && PastState.IsKeyUp(key);
	}
}
