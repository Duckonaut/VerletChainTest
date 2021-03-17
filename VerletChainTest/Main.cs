using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace VerletChainTest
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Main : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		public static Texture2D Pixel { get; set; }

		public List<ChainVertex> Vertices { get; set; }
		public List<ChainSegment> Segments { get; set; }

		public static Random Rand { get; set; }

		Vector2 otherPos;
		int currentVertex = 0;
		public Main()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			Rand = new Random();
			Vertices = new List<ChainVertex>();
			Segments = new List<ChainSegment>();

			for (int i = 0; i < 8; i++)
			{
				Vertices.Add(new ChainVertex(new Vector2(20 * (i + 1), 120), (float)(Rand.NextDouble() * 8f + 4f), 0.9f, 0.5f, 1f));
			}

			for (int i = 0; i < 7; i++)
			{
				Segments.Add(new ChainSegment(Vertices[i], Vertices[i + 1], (float)(Rand.NextDouble() * 20f + 10f)));
			}

			for (int i = 0; i < 4; i++)
			{
				Vertices.Add(new ChainVertex(new Vector2(60, 120 + (20 * i)), (float)(Rand.NextDouble() * 8f + 4f), 0.9f, 0.5f, 1f));
			}

			Segments.Add(new ChainSegment(Vertices[3], Vertices[8], (float)(Rand.NextDouble() * 20f + 10f)));

			for (int i = 0; i < 3; i++)
			{
				Segments.Add(new ChainSegment(Vertices[8 + i], Vertices[8 + i + 1], (float)(Rand.NextDouble() * 20f + 10f)));
			}

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Pixel = Content.Load<Texture2D>("Pixel");
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

			if (Input.MouseClick)
			{
				Vertices[currentVertex].StaticPos = Input.MousePos.ToVector2();
				Vertices[currentVertex].Static = !Vertices[currentVertex].Static;

			}

			if (Input.RightMouseClick)
			{
				var newVertex = new ChainVertex(Input.MousePos.ToVector2(), (float)(Rand.NextDouble() * 8f + 4f), 0.9f, 0.5f, 1f);
				Vertices.Add(newVertex);

				Segments.Add(new ChainSegment(newVertex, Vertices[currentVertex], (newVertex.Position - Vertices[currentVertex].Position).Length() * 0.8f));
			}

			if (Input.KeyboardClick(Keys.R))
			{
				otherPos = new Vector2(-1, -1);

			}	

			if (Input.ScrollDown)
			{
				currentVertex++;
				if (currentVertex >= Vertices.Count)
				{
					currentVertex = 0;
				}
			}
			if (Input.ScrollUp)
			{
				currentVertex--;
				if (currentVertex < 0)
				{
					currentVertex = Vertices.Count - 1;
				}
			}

			//Vertices[0].Position = Input.MousePos.ToVector2();


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

			

			Input.PastState = Input.CurrentState;
			Input.PastMouseState = Input.CurrentMouseState;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
			for (int i = 0; i < Vertices.Count; i++)
			{
				var vertex = Vertices[i];

				if (i == currentVertex)
				{
					vertex.Draw(spriteBatch, Color.Red);
				}
				else
				{
					vertex.Draw(spriteBatch);
				}
			}

			foreach (var segment in Segments)
			{
				segment.Draw(spriteBatch);
			}

			spriteBatch.Draw(Main.Pixel, Input.MousePos.ToVector2(), null, Color.White, 0f, new Vector2(0.5f, 0.5f), new Vector2(2f, 2f), SpriteEffects.None, 0);

			spriteBatch.End();
			base.Draw(gameTime);
		}
	}
}
