using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerletChainTest
{
	public class ChainVertex
	{
		public Vector2 Position { get; set; }
		public Vector2 LastPosition { get; set; }
		public float Drag { get; set; }
		public float GroundBounce { get; set; }
		public float Gravity { get; set; }
		public float Scale { get; set; }
		public bool Static { get; set; }
		public Vector2 StaticPos { get; set; }

		public ChainVertex(Vector2 position, float scale, float drag = 0.9f, float groundBounce = 0.5f, float gravity = 0.2f)
		{
			Position = position;
			LastPosition = position;
			Scale = scale;
			Drag = drag;
			GroundBounce = groundBounce;
			Gravity = gravity;
		}

		public void Update()
		{
			Vector2 delta = (Position - LastPosition) * Drag;

			LastPosition = Position;
			Position += delta;

			Position += new Vector2(0, Gravity);
		}

		public void SetStatic()
		{
			if (Static)
			{
				Position = StaticPos;
			}
		}

		public void StandardConstrain()
		{
			if (Position.Y > 400)
			{
				Vector2 delta = (Position - LastPosition) * Drag;

				float speed = delta.Length();
				Position = new Vector2(Position.X, 400);
				LastPosition = new Vector2(Position.X, 400 + delta.Y * GroundBounce);
			}
		}
		public void Draw(SpriteBatch sB, Color color = default(Color))
		{
			if (color == default(Color))
			{
				color = Color.White;
			}

			Vector2 delta = (Position - LastPosition);
			float rotation = (float)Math.Atan2(delta.Y, delta.X) + 1.57f;

			sB.Draw(Main.Pixel, Position, null, color, 0f, new Vector2(0.5f), Scale, SpriteEffects.None, 0);
		}
	}
}
