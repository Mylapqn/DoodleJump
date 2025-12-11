using DoodleJump.Hierarchy;
using DoodleJump.Objects;
using DoodleJump.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DoodleJump.Core
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public PolygonDrawer polygonDrawer;
		public Assets Assets;
        public static Game1 Instance;

		public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
			_graphics.PreferredBackBufferWidth = GameSettings.WindowWidth;
			_graphics.PreferredBackBufferHeight = GameSettings.WindowHeight;
			_graphics.ApplyChanges();

            Assets = new Assets(this);

			Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Instance = this;
		}

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            GameSettings.ActiveScreen = new PlayScreen();
			FireSprite fire = new FireSprite(GameSettings.ActiveScreen);
            fire.TopLeftPosition = new Vector2(200, 200);
		}

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            polygonDrawer = new PolygonDrawer();
            polygonDrawer.Initialize(GraphicsDevice, new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true
            });
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
            polygonDrawer.Begin();
			GameSettings.ActiveScreen?.Draw(_spriteBatch);
			_spriteBatch.End();
            polygonDrawer.End();

			base.Draw(gameTime);
        }
    }
}
