using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platfarm
{
    public class Tile
    {
        public Texture2D Texture { get; set; }

        public Vector2 ImageIndex { get; set; }

        public bool Collide { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Size  { get; set; }

        public int Scale { get; set; }

        public Rectangle Bound
        {
            get { return new Rectangle((int)Position.X * Scale, (int)Position.Y * Scale, (int)Size.X * Scale, (int)Size.Y * Scale); }
        }

        public Rectangle Source
        {
            get { return new Rectangle((int) ImageIndex.X, (int) ImageIndex.Y, (int) Size.X, (int) Size.Y); }
        }

        public Tile(Texture2D texture, Vector2 imageIndex, bool collide, Vector2 position, Vector2 size, int scale)
        {
            Texture = texture;
            ImageIndex = imageIndex;
            Collide = collide;
            Position = position;
            Size = size;
            Scale = scale;
        }
    }
}
