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

        public List<Tile> LevelObjects { get; set; }
        public Vector2 StartPosition { get; set; }
        public Player Player { get; set; }
        public List<Enemy> Enemies { get; set; }
        public List<GameEntity> DeathList { get; set; }

        public const int GridMultiplier = 16;

        private readonly Texture2D _tileTexture;
        private readonly Texture2D _sceneryTexture;
        private readonly Texture2D _tubeTexture;

        public Level(IServiceProvider serviceProvider, Random random)
        {
            Content = new ContentManager(serviceProvider, "Content");
            _tileTexture = Content.Load<Texture2D>("Tiles");
            _sceneryTexture = Content.Load<Texture2D>("Scenery");
            _tubeTexture = Content.Load<Texture2D>("Tube");
            
            StartPosition = new Vector2(2 * GridMultiplier, 26 * GridMultiplier);
            
            LoadLevelObjects();

            Enemies = new List<Enemy>
                {
                    new Turtle(this, new Vector2(random.Next(225,475), 200)),
                    new Goomba(this, new Vector2(random.Next(225,475), 200)),
                };

            DeathList = new List<GameEntity>();

            Player = new Player(this);
        }

        private void LoadLevelObjects()
        {
            var Level = Content.Load<Texture2D>("Level1");
            Color[,] colorArray = TextureTo2DArray(Level);
            LevelObjects = new List<Tile>();

            for (int i = 0; i < colorArray.GetLength(0); i++)
            {
                for (int j = 0; j < colorArray.GetLength(1); j++)
                {
                    if (colorArray[i, j] != Color.White)
                        LevelObjects.Add(new Tile(_tileTexture, new Vector2(0, GridMultiplier * ColorArrayToInt(colorArray[i, j])), true, new Vector2(i * GridMultiplier, j * GridMultiplier), new Vector2(GridMultiplier, GridMultiplier)));
                   
                }
            }

            LevelObjects.Add(new Tile(_sceneryTexture, new Vector2(51, 73), false, new Vector2( 1 * GridMultiplier,     26 * GridMultiplier - 3),  new Vector2(79, 35)));   // Big Hill
            LevelObjects.Add(new Tile(_sceneryTexture, new Vector2(0, 0),   false, new Vector2( 9 * GridMultiplier - 8, 18 * GridMultiplier - 3),  new Vector2(32, 24)));   // Little Cloud
            LevelObjects.Add(new Tile(_sceneryTexture, new Vector2(80, 48), false, new Vector2(12 * GridMultiplier - 8, 26 * GridMultiplier + 8),  new Vector2(64, 24)));   // Big Bush
            LevelObjects.Add(new Tile(_sceneryTexture, new Vector2(0, 73),  false, new Vector2(16 * GridMultiplier,     26 * GridMultiplier - 3),  new Vector2(48, 35)));   // Small Hill
            LevelObjects.Add(new Tile(_sceneryTexture, new Vector2(0, 0),   false, new Vector2(20 * GridMultiplier - 8, 17 * GridMultiplier - 3),  new Vector2(32, 24)));   // Little Cloud
            LevelObjects.Add(new Tile(_sceneryTexture, new Vector2(0, 48),  false, new Vector2(24 * GridMultiplier - 8, 26 * GridMultiplier + 8),  new Vector2(32, 24)));   // Little Bush
            LevelObjects.Add(new Tile(_sceneryTexture, new Vector2(80, 0),  false, new Vector2(28 * GridMultiplier - 8, 18 * GridMultiplier - 3),  new Vector2(64, 24)));   // Big Cloud
            LevelObjects.Add(new Tile(_tubeTexture,    new Vector2(0, 0),    true, new Vector2(28 * GridMultiplier,     25 * GridMultiplier + 16), new Vector2(32, 32)));   // Tube
            LevelObjects.Add(new Tile(_sceneryTexture, new Vector2(32, 0),  false, new Vector2(37 * GridMultiplier - 8, 18 * GridMultiplier - 3),  new Vector2(48, 24)));   // Medium Cloud
            LevelObjects.Add(new Tile(_tubeTexture,    new Vector2(0, 0),    true, new Vector2(38 * GridMultiplier,     24 * GridMultiplier + 16), new Vector2(32, 32)));   // Tube
            LevelObjects.Add(new Tile(_tubeTexture,    new Vector2(0, 16),   true, new Vector2(38 * GridMultiplier,     26 * GridMultiplier + 16), new Vector2(32, 16)));   // Tube Chunk
            LevelObjects.Add(new Tile(_sceneryTexture, new Vector2(32, 48), false, new Vector2(42 * GridMultiplier - 8, 26 * GridMultiplier + 8),  new Vector2(48, 24)));   // Medium Bush
        }

        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int i = 0; i < texture.Width; i++)
                for (int j = 0; j < texture.Height; j++)
                    colors2D[i, j] = colors1D[i + j * texture.Width];

            return colors2D;
        }

        private int ColorArrayToInt(Color color)
        {
            var hexColor = color.ToHex();
            switch (hexColor)
            {
                case "#000000":
                    return 0;
                case "#7F7F7F":
                    return 1;
                case "#FF7F27":
                    return 6;
                default:
                    return 0;
            }
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
                var deadEntity = DeathList[index - 1];
                if (deadEntity.DeathCountdown > deadEntity.DeathTimeout)
                {
                    DeathList.Remove(deadEntity);
                    deadEntity.Unload();
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var levelObject in LevelObjects)
            {
                spriteBatch.Draw(levelObject.Texture, levelObject.Position, levelObject.Source, Color.White, 0.0f, new Vector2(), 1.0f, SpriteEffects.None, 0.0f);
            }

            foreach (var enemy in Enemies)
            {
                enemy.Draw(gameTime, spriteBatch);
            }

            Player.Draw(gameTime, spriteBatch);
        }
    }

}
