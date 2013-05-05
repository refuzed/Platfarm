using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platfarm
{
    public class GameEntity
    {
        public Level Level;
        public Texture2D Texture;
        public Vector2 Size;
        public Vector2 CurrentPosition;
        public Vector2 PreviousPosition;
        public Vector2 MovementVector;
        public Direction Direction;
        public Vector2 Speed;
        public Vector2 MaxSpeed;
        public Vector2 Friction;
        public bool IsOnGround;

        public Rectangle Bound(Direction direction = Direction.None)
        {
            int xAdjust = 0;
            int yAdjust = 0;

            switch (direction)
            {
                case Direction.Left:
                    xAdjust = -1;
                    break;
                case Direction.Right:
                    xAdjust = 1;
                    break;
                case Direction.Up:
                    yAdjust = -1;
                    break;
                case Direction.Down:
                    yAdjust = 1;
                    break;
            }

            return new Rectangle((int)CurrentPosition.X + xAdjust, (int)CurrentPosition.Y + yAdjust, (int)Size.X, (int)Size.Y);
        }
    }
}
