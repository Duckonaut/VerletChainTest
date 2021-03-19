using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VerletChainTest
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Main : Game
	{
		const float GRAVITY = 1f;
		public const int GROUND_LEVEL = 100;
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		public static Texture2D Pixel { get; set; }

		public List<ChainVertex> Vertices { get; set; }
		public List<ChainSegment> Segments { get; set; }
		public static Point WindowSize { get; set; }
		public static Random Rand { get; set; }
		public SpriteFont Font { get; set; }
		public bool ShowStats { get; set; }
		int currentVertex = 0;
		int currentShiftVertex = 0;
		public Main()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			Window.AllowUserResizing = true;
			
			Window.ClientSizeChanged += (sender, e) => WindowSize = Window.ClientBounds.Size;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			WindowSize = Window.ClientBounds.Size;
			Rand = new Random();
			Vertices = new List<ChainVertex>();
			Segments = new List<ChainSegment>();

			for (int i = 0; i < 8; i++)
			{
				Vertices.Add(new ChainVertex(new Vector2(20 * (i + 1), 120), (float)(Rand.NextDouble() * 8f + 4f), 0.9f, 0.5f, GRAVITY));
			}

			for (int i = 0; i < 7; i++)
			{
				Segments.Add(new ChainSegment(Vertices[i], Vertices[i + 1], (float)(Rand.NextDouble() * 20f + 10f)));
			}

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Pixel = Content.Load<Texture2D>("Pixel");

			Font = Content.Load<SpriteFont>("MainFont");
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			Input.CurrentState = Keyboard.GetState();
			Input.CurrentMouseState = Mouse.GetState();

			if (IsActive)
			{
				HandleInput();

				foreach (var vertex in Vertices)
				{
					vertex.Update();
					vertex.StandardConstrain();
					vertex.SetStatic();
				}

				foreach (var segment in Segments)
				{
					segment.ConstrainLine();
				}
			}

			Input.PastState = Input.CurrentState;
			Input.PastMouseState = Input.CurrentMouseState;

			base.Update(gameTime);
		}

		private void HandleInput()
		{
			if (Input.KeyboardHold(Keys.S)) // Select nearest vertex
			{
				var foundVertex = Vertices.IndexOf(Vertices.OrderBy(v => (v.Position - Input.MousePos.ToVector2()).LengthSquared()).First());

				if (Input.MouseClick)
				{
					currentVertex = foundVertex;
				}

				if (Input.RightMouseClick)
				{
					currentShiftVertex = foundVertex;
				}
			}
			else // Toggle static or add segments
			{
				if (Input.MouseClick)
				{
					Vertices[currentVertex].StaticPos = Input.MousePos.ToVector2();
					Vertices[currentVertex].Static = !Vertices[currentVertex].Static;
				}

				if (Input.RightMouseClick)
				{
					var newVertex = new ChainVertex(Input.MousePos.ToVector2(), (float)(Rand.NextDouble() * 8f + 4f), 0.9f, 0.5f, GRAVITY);
					Vertices.Add(newVertex);

					Segments.Add(new ChainSegment(newVertex, Vertices[currentVertex], (float)(Rand.NextDouble() * 20f + 10f)));

					currentVertex = Vertices.Count - 1;
				}
			}

			if (Input.KeyboardClick(Keys.L)) // Link / Unlink
			{
				if (currentShiftVertex != currentVertex)
				{
					if (Input.Shift)
					{
						var foundSegment = Segments.First(s => (s.Vertex1 == Vertices[currentVertex] && s.Vertex2 == Vertices[currentShiftVertex]) || (s.Vertex2 == Vertices[currentVertex] && s.Vertex1 == Vertices[currentShiftVertex]));

						if (foundSegment != null)
						{
							Segments.Remove(foundSegment);
						}
					}
					else
					{
						Segments.Add(new ChainSegment(Vertices[currentShiftVertex], Vertices[currentVertex], (float)(Rand.NextDouble() * 20f + 10f)));
					}
				}
			}

			if (Input.ScrollDown) // Change selected vertices
			{
				if (Input.Shift)
				{
					currentShiftVertex++;
					if (currentShiftVertex >= Vertices.Count)
					{
						currentShiftVertex = 0;
					}
				}
				else
				{
					currentVertex++;
					if (currentVertex >= Vertices.Count)
					{
						currentVertex = 0;
					}
				}
			}
			if (Input.ScrollUp)
			{
				if (Input.Shift)
				{
					currentShiftVertex--;
					if (currentShiftVertex < 0)
					{
						currentShiftVertex = Vertices.Count - 1;
					}
				}
				else
				{
					currentVertex--;
					if (currentVertex < 0)
					{
						currentVertex = Vertices.Count - 1;
					}
				}
			}

			if (Input.KeyboardHold(Keys.H)) // Hold current vertex to mouse
			{
				Vertices[currentVertex].Position = Input.MousePos.ToVector2();
			}

			if (Input.KeyboardClick(Keys.Back) || Input.KeyboardClick(Keys.Delete)) // Delete current vertex
			{
				if (Vertices.Count > 1)
				{
					var vertexToDelete = Vertices[currentVertex];

					foreach (var s in Segments.Where(s => s.Vertex1 == vertexToDelete || s.Vertex2 == vertexToDelete).ToList())
					{
						Segments.Remove(s);
					}

					Vertices.Remove(vertexToDelete);

					if (currentVertex >= Vertices.Count)
					{
						currentVertex = Vertices.Count - 1;
					}
				}
			}

			if (Input.KeyboardClick(Keys.I))
			{
				ShowStats = !ShowStats;
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
			spriteBatch.Draw(Main.Pixel, new Vector2(0, Main.WindowSize.Y - Main.GROUND_LEVEL), null, Color.White, 0f, Vector2.Zero, new Vector2(Window.ClientBounds.Width, 2f), SpriteEffects.None, 0);

			foreach (var segment in Segments)
			{
				segment.Draw(spriteBatch);
			}

			for (int i = 0; i < Vertices.Count; i++)
			{
				var vertex = Vertices[i];

				if (i == currentVertex)
				{
					vertex.Draw(spriteBatch, Color.Red);
				}
				else if (i == currentShiftVertex)
				{
					vertex.Draw(spriteBatch, Color.Blue);
				}
				else
				{
					vertex.Draw(spriteBatch);
				}
			}

			
			spriteBatch.Draw(Main.Pixel, Input.MousePos.ToVector2(), null, Color.White, 0f, new Vector2(0.5f, 0.5f), new Vector2(2f, 2f), SpriteEffects.None, 0);

			if (ShowStats)
			{
				string text = $"Vertex Count: {Vertices.Count}\nSegment Count: {Segments.Count}\nCurrent Vertex: {currentVertex}\nCurrent Shift Vertex: {currentShiftVertex}";
				spriteBatch.DrawString(Font, text, Vector2.Zero, Color.White);
			}

			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
