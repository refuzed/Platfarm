using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platfarm
{
    public class Level
    {
        public List<Rectangle> LevelObjects { get; set; }
        public Vector2 StartPosition { get; set; }
        public Rectangle ExitBox { get; set; }

        public readonly ContentManager content;

        private Player player;
        private Texture2D levelTexture;

        public Level(IServiceProvider serviceProvider)
        {
            content = new ContentManager(serviceProvider, "Content");
            levelTexture = content.Load<Texture2D>("square");

            StartPosition = new Vector2(250, 280);
            LevelObjects = new List<Rectangle>
                {
                    new Rectangle(200, 300, 400, 10),  // Ground
                    new Rectangle(245, 250, 100, 10),  // A flat thingie in the air
                    new Rectangle(455, 250, 100, 10),  // More thingie
                    new Rectangle(360, 200, 80, 10)    // You know the drill by now
                };
            ExitBox = new Rectangle(390, 180, 20, 20);

            player = new Player(this);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            player.Update(gameTime, keyboardState);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            player.Draw(spriteBatch);

            spriteBatch.Draw(levelTexture, ExitBox, Color.LightGreen);

            foreach (var levelObject in LevelObjects)
            {
                spriteBatch.Draw(levelTexture, levelObject, Color.Azure);
            }
        }
    }
}
