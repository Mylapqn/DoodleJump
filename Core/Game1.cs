using DoodleJump.Hierarchy;
using DoodleJump.Objects;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace DoodleJump.Core
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private PolygonDrawer _polygonDrawer;
		public static Game1 Instance;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = GameSettings.WindowWidth;
			_graphics.PreferredBackBufferHeight = GameSettings.WindowHeight;
			_graphics.ApplyChanges();

			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			

			Instance = this;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();

			GameSettings.MenuScreen = new MenuScreen();
			GameSettings.PlayScreen = new PlayScreen();
			GameSettings.EndScreen = new EndScreen();

			GameSettings.MenuScreen.Initialize();

		}

		protected override void LoadContent()
		{
			GameSettings.Assets = new Assets();
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			_polygonDrawer = new PolygonDrawer();
			_polygonDrawer.Initialize(GraphicsDevice, new BasicEffect(GraphicsDevice)
			{
				VertexColorEnabled = true
			});
			// TODO: use this.Content to load your game content here
			GameSettings.Assets.LoadAllAssets();
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			// TODO: Add your update logic here
			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

			Input.Update();
			GameSettings.ActiveScreen?.Update(dt);

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.IndianRed);

			GameSettings.ActiveScreen?.Draw(_spriteBatch, _polygonDrawer);

			base.Draw(gameTime);
		}
	}
}
