using System;
using System.Linq;
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
        public int Scale { get; set; }
        public Rectangle DrawWindow { get { return _drawWindow; } set { _drawWindow = value; } }
        public int DrawWindowX { get { return _drawWindow.X; } set { _drawWindow.X = value; } }

        private Rectangle _drawWindow;

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

            StartPosition = new Vector2(50, 50);
            Scale = 2;

            var graphicsDevice = Content.GetGraphicsDevice();
            DrawWindow = new Rectangle(100, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);

            LoadLevelObjects();

            Enemies = new List<Enemy>
                {
                    //new Turtle(this, new Vector2(random.Next(225,475), 200)),
                    //new Goomba(this, new Vector2(random.Next(225,475), 200)),
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
                        LevelObjects.Add(BuildTile(colorArray[i, j], i, j));
                }
            }
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

        private Tile BuildTile(Color color, int i, int j)
        {
            Texture2D texture;
            Vector2 imageIndex;
            bool collide;
            Vector2 position = new Vector2(i * GridMultiplier, j * GridMultiplier);
            Vector2 tileSize;

            var hexColor = color.ToHex();
            switch (hexColor)
            {
                // Basic Level Tiles
                case "#000000":
                case "#404040":
                case "#FF0000":
                case "#FF6A00":
                case "#FFD800":
                case "#B6FF00":
                case "#4CFF00":
                case "#00FF21":
                    texture = _tileTexture;
                    imageIndex = new Vector2(0, GridMultiplier * ColorToInt(color));
                    collide = true;
                    tileSize = new Vector2(GridMultiplier, GridMultiplier);
                    break;
                case "#606060": // Small Cloud
                    texture = _sceneryTexture;
                    imageIndex = new Vector2(0, 0);
                    position.X -= 8;
                    position.Y -= 3;
                    collide = false;
                    tileSize = new Vector2(32, 24);
                    break;
                case "#808080": // Medium Cloud
                    texture = _sceneryTexture;
                    imageIndex = new Vector2(32, 0);
                    position.X -= 8;
                    position.Y -= 3;
                    collide = false;
                    tileSize = new Vector2(48, 24);
                    break;
                case "#7F0000": // Big Cloud
                    texture = _sceneryTexture;
                    imageIndex = new Vector2(80, 0);
                    position.X -= 8;
                    position.Y -= 3;
                    collide = false;
                    tileSize = new Vector2(64, 24);
                    break;
                case "#7F3300": // Small Bush
                    texture = _sceneryTexture;
                    imageIndex = new Vector2(0, 48);
                    position.X -= 8;
                    position.Y += 8;
                    collide = false;
                    tileSize = new Vector2(32, 24);
                    break;
                case "#7F6A00": // Medium Bush
                    texture = _sceneryTexture;
                    imageIndex = new Vector2(32, 48);
                    position.X -= 8;
                    position.Y += 8;
                    collide = false;
                    tileSize = new Vector2(48, 24);
                    break;
                case "#5B7F00": // Big Bush
                    texture = _sceneryTexture;
                    imageIndex = new Vector2(80, 48);
                    position.X -= 8;
                    position.Y += 8;
                    collide = false;
                    tileSize = new Vector2(64, 24);
                    break;
                case "#267F00":  // Small Hill
                    texture = _sceneryTexture;
                    imageIndex = new Vector2(0, 74);
                    position.Y -= 3;
                    collide = false;
                    tileSize = new Vector2(48, 36);
                    break;
                case "#007F0E":  // Big Hill
                    texture = _sceneryTexture;
                    imageIndex = new Vector2(51, 74);
                    position.Y -= 3;
                    collide = false;
                    tileSize = new Vector2(79, 36);
                    break;
                case "#A0A0A0": // Tube Part
                    texture = _tubeTexture;
                    imageIndex = new Vector2(0, 16);
                    position.Y += 16;
                    collide = true;
                    tileSize = new Vector2(32, 16);
                    break;
                case "#303030": // Tube Top
                    texture = _tubeTexture;
                    imageIndex = new Vector2(0, 0);
                    position.Y += 16;
                    collide = true;
                    tileSize = new Vector2(32, 32);
                    break;
                default:
                    throw new Exception("zONO");
            }

            return new Tile(texture, imageIndex, collide, position, tileSize, Scale);
        }

        private int ColorToInt(Color color)
        {
            var hexColor = color.ToHex();
            switch (hexColor)
            {
                case "#000000":
                    return 0;
                case "#404040":
                    return 1;
                case "#FF0000":
                    return 2;
                case "#FF6A00":
                    return 3;
                case "#FFD800":
                    return 4;
                case "#B6FF00":
                    return 5;
                case "#4CFF00":
                    return 6;
                case "#00FF21":
                    return 7;
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
                    deadEntity.Unload();
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var levelObject in LevelObjects)
            {
                spriteBatch.Draw(levelObject.Texture, levelObject.Position * Scale - DrawWindow.Location.ToVector2(), levelObject.Source, Color.White, 0.0f, new Vector2(), Scale, SpriteEffects.None, 0.0f);
            }

            foreach (var enemy in Enemies)
            {
                enemy.Draw(gameTime, spriteBatch);
            }

            Player.Draw(gameTime, spriteBatch);
        }
    }

}
