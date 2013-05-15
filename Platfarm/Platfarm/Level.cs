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
        public readonly ContentManager Content;

        public List<Rectangle> LevelObjects { get; set; }
        public Vector2 StartPosition { get; set; }
        public Rectangle ExitBox { get; set; }
        public Player Player;
        public List<Enemy> Enemies; 
        public List<GameEntity> DeathList; 

        private readonly Texture2D _levelTexture;

        public Level(IServiceProvider serviceProvider)
        {
            Content = new ContentManager(serviceProvider, "Content");
            _levelTexture = Content.Load<Texture2D>("square");

            StartPosition = new Vector2(250, 280);

            LevelObjects = new List<Rectangle>
                {
                    new Rectangle(200, 300, 400, 10),  // Ground
                    new Rectangle(245, 250, 100, 10),  // A flat thingie in the air
                    new Rectangle(455, 250, 100, 10),  // More thingie
                    new Rectangle(360, 200, 80, 10),    // You know the drill by now
                    new Rectangle(300, 290, 10,10),
                    new Rectangle(500, 290, 10,10)
                };

            ExitBox = new Rectangle(390, 180, 20, 20);

            Enemies = new List<Enemy>
                {
                    new Enemy(this, new Vector2(550, 200))
                };

            DeathList = new List<GameEntity>();

            Player = new Player(this);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            Player.Update(gameTime, keyboardState);

            foreach (var enemy in Enemies)
            {
                enemy.Update(gameTime);
            }

            for (int index = DeathList.Count; index > 0; index--)
            {
                var deadEnemy = DeathList[index - 1];
                if (deadEnemy.DeathCountdown > 0.5f)
                {
                    DeathList.Remove(deadEnemy);
                    deadEnemy.Unload();
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Player.Draw(gameTime, spriteBatch);

            foreach (var enemy in Enemies)
            {
                enemy.Draw(gameTime, spriteBatch);
            }

            foreach (var levelObject in LevelObjects)
            {
                spriteBatch.Draw(_levelTexture, levelObject, Color.Azure);
            }

            spriteBatch.Draw(_levelTexture, ExitBox, Color.LightGreen);
        }
    }
}
