using System;
using AwaitableCoroutine;
using Gui.Input;
using Gui.Services;
using Legion.Model;
using Legion.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Legion
{
    public class LegionGame : Game, IGuiServices
    {
        const float Scale = 1.5f;
        const int WorldWidth = 640;
        const int WorldHeight = 512;
        const float ScreenWidth = WorldWidth * Scale;
        const float ScreenHeight = WorldHeight * Scale;

        private readonly ILegionViewsManager _viewsManager;
        private readonly ICoroutineRunner _coroutineRunner;
        private readonly GraphicsDeviceManager _graphics;
        private readonly Matrix _scaleMatrix;

        SpriteBatch _spriteBatch;
        BasicDrawer _basicDrawer;
        ImagesStore _imagesStore;

        public LegionGame(ILegionViewsManager viewsManager, ICoroutineRunner coroutineRunner)
        {
            _viewsManager = viewsManager;
            _coroutineRunner = coroutineRunner;
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = (int) ScreenWidth;
            _graphics.PreferredBackBufferHeight = (int) ScreenHeight;

            _scaleMatrix = Matrix.CreateScale(Scale);
            InputManager.ScaleMatrix = _scaleMatrix;
            GameBounds = new Rectangle(0, 0, WorldWidth, WorldHeight);

            Content.RootDirectory = "Assets";
        }

        public IBasicDrawer BasicDrawer => _basicDrawer;

        public IImagesStore ImagesStore => _imagesStore;

        public Rectangle GameBounds { get; }

        public ILegionViewsManager ViewsManager => _viewsManager;

        public event Action GameLoaded;

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _basicDrawer = new BasicDrawer(_spriteBatch);
            _imagesStore = new ImagesStore();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            IsMouseVisible = true;
            _basicDrawer.LoadContent(this);
            _imagesStore.LoadContent(this);

            //NOTE: views are initalized here because it needs imagesProvider content loaded before this
            ViewsManager.Initialize();

            GameLoaded?.Invoke();
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _coroutineRunner.Update();
            ViewsManager.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(sortMode: SpriteSortMode.Deferred, transformMatrix: _scaleMatrix);
            base.Draw(gameTime);
            ViewsManager.Draw();

            _spriteBatch.End();
        }
    }
}