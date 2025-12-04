using DoodleJump.Hierarchy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DoodleJump.Core
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Assets Assets;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = GameSettings.WindowWidth;
			_graphics.PreferredBackBufferHeight = GameSettings.WindowHeight;
			_graphics.ApplyChanges();

            Assets = new Assets(this);

			Content.RootDirectory = "Content";
            IsMouseVisible = true;
		}

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
			// TODO: use this.Content to load your game content here
            Assets.LoadAllAssets();
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here
			_spriteBatch.Begin();
			GameSettings.ActiveScreen?.Draw(_spriteBatch);
			_spriteBatch.End();

			base.Draw(gameTime);
        }
    }
}
