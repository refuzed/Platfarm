using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platfarm
{
    public class GameEntity
    {
        public Level Level { get; set; }
        public Texture2D Texture { get; set; }
        public Direction Direction { get; set; }
        public Vector2 Size;
        public Vector2 CurrentPosition;
        public Vector2 PreviousPosition;
        public Vector2 MovementVector;
        public Vector2 Speed;
        public Vector2 MaxSpeed;
        public Vector2 Friction;
        public bool IsOnGround { get; set; }
        public bool IsJumping { get; set; }
        public float DeathCountdown { get; set; }
        public float DeathTimeout { get; set; }
        public bool isDead { get; set; }
        public Vector2 DrawPosition { get { return CurrentPosition * Level.Scale - Level.DrawWindow.Location.ToVector2(); } }

        public Animator Sprite { get; set; }
        public Dictionary<AnimationType, Animation> Animations { get; set; }
        public SpriteEffects Flip { get; set; }
        
        public GameEntity(Level level)
        {
            Level = level;
            MovementVector = new Vector2(0, 0);
            Speed = new Vector2(3.0f, 3.0f);
            Friction = new Vector2(2.0f, 5.0f);
            MaxSpeed = new Vector2(1.0f, 2.0f);
            DeathTimeout = 1.0f;
        }

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

            return new Rectangle(Convert.ToInt32(CurrentPosition.X) * Level.Scale + xAdjust * Convert.ToInt32(Speed.X), 
                                 Convert.ToInt32(CurrentPosition.Y) * Level.Scale + yAdjust * Convert.ToInt32(Speed.Y), 
                                 Convert.ToInt32(Size.X) * Level.Scale, 
                                 Convert.ToInt32(Size.Y) * Level.Scale);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Sprite.Draw(gameTime, spriteBatch, DrawPosition, Flip, Level.Scale);
        }

        public virtual void Kill()
        {
            isDead = true;
            Sprite.SetAnimation(Animations[AnimationType.Death]);
            Level.DeathList.Add(this);
        }

        public virtual void CheckCollision()
        {
            IsOnGround = false;

            foreach (var levelObject in Level.LevelObjects.Where(x => x.Collide))
            {
                if (Bound(Direction.Down).Intersects(levelObject.Bound))
                {
                    CurrentPosition.Y = PreviousPosition.Y;
                    MovementVector.Y *= 0.35f;
                    IsOnGround = true;
                    IsJumping = false;
                }

                if (Bound(Direction.Left).Intersects(levelObject.Bound) || Bound(Direction.Right).Intersects(levelObject.Bound))
                {
                    CurrentPosition.X = PreviousPosition.X;
                    MovementVector.X *= -0.15f;
                    Direction = Direction == Direction.Right ? Direction.Left : Direction.Right;
                }

                if (Bound(Direction.Up).Intersects(levelObject.Bound))
                {
                    CurrentPosition.Y = PreviousPosition.Y;
                    MovementVector.Y = -0.25f;
                }
            }
        }

        public virtual void Unload()
        {
            Level.DeathList.Remove(this);
        }
    }
}
