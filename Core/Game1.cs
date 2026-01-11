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
			StartGame();
			Window.Title = "Rainbow Cat";
		}

		public void StartGame(bool showMenu = true)
		{
			GameSettings.Initialize();
			if (showMenu)
				GameSettings.MenuScreen.Initialize();
			else
				GameSettings.PlayScreen.Initialize();
		}

		protected override void LoadContent()
		{
			GameSettings.Assets = new Assets(Content);
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

			// TODO: Add your update logic here
			float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
			dt *= GameSettings.TimeScale;

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
