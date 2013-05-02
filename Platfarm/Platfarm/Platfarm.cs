using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platfarm
{
    public class Platfarm : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Level level;
        private KeyboardState currentKeyboardState;
        private KeyboardState prevKeyboardState;
        private bool pause;

        public Platfarm()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            level = new Level(Services);
        }

        protected override void Update(GameTime gameTime)
        {
            // Get keyboard state once so we can pass it through.  
            prevKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            // Previous keyboardstate was saved to prevent double taps:
            if (currentKeyboardState.IsKeyDown(Keys.Escape) 
                && !prevKeyboardState.IsKeyDown(Keys.Escape))
                pause = !pause;

            if(!pause)
                level.Update(gameTime, currentKeyboardState);

            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            level.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
