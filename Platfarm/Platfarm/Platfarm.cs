using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platfarm
{
    public class Platfarm : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        private Level _level;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _prevKeyboardState;
        private bool _pause;

        public Platfarm()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _level = new Level(Services);
        }

        protected override void Update(GameTime gameTime)
        {
            // Get keyboard state once so we can pass it through.  
            _prevKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            // Previous keyboardstate was saved to prevent double taps:
            if (_currentKeyboardState.IsKeyDown(Keys.Escape) 
                && !_prevKeyboardState.IsKeyDown(Keys.Escape))
                _level = new Level(Services);

            if(!_pause)
                _level.Update(gameTime, _currentKeyboardState);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin();

            _level.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
