using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platfarm
{
    public class Enemy : GameEntity
    {
        public Enemy(Level level, Vector2 startPosition)
        {
            Level = level;
            Texture = level.Content.Load<Texture2D>("Goomba");
            CurrentPosition = startPosition;
            Size = new Vector2(16, 16);
            Speed = new Vector2(1.0f, 3.0f);
            Direction = Direction.Left;
            Color = Color.Red;

            Sprite = new Animator();
            Animations = new[]
                {
                    new Animation(this, AnimationType.Run,   2, 0.5f, true),
                    new Animation(this, AnimationType.Death, 1, 0.5f, false)
                }.ToDictionary(x => x.AnimationType);
            Sprite.SetAnimation(Animations[AnimationType.Run]);
        }

        public void Update(GameTime gameTime)
        {
            if (isDead)
                DeathCountdown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                Move(gameTime);
        }

        private void Move(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            PreviousPosition = CurrentPosition;
            Sprite.SetAnimation(Animations[AnimationType.Run]);

            switch (Direction)
            {
                case Direction.Left:
                    Flip = SpriteEffects.FlipHorizontally;
                    MovementVector.X = -Speed.X;
                    break;
                case Direction.Right:
                    Flip = SpriteEffects.None;
                    MovementVector.X = Speed.X;
                    break;
            }

            CheckCollision();
            ApplyPhysics(elapsed);
            
            CurrentPosition.X += MovementVector.X;
            CurrentPosition.Y -= MovementVector.Y;
        }

        private void ApplyPhysics(float elapsed)
        {
            // Deal with falling
            if (!IsOnGround)
            {
                MovementVector.Y -= Friction.Y * elapsed;
            }
        }

        private void CheckCollision()
        {
            IsOnGround = false;

            foreach (var levelObject in Level.LevelObjects)
            {
                if (Bound(Direction.Left).Intersects(levelObject) || Bound(Direction.Right).Intersects(levelObject))
                {
                    CurrentPosition.X = PreviousPosition.X;
                    MovementVector.X = -MovementVector.X;
                    Direction = Direction == Direction.Left ? Direction.Right : Direction.Left;
                }

                if (Bound(Direction.Down).Intersects(levelObject))
                {
                    CurrentPosition.Y = PreviousPosition.Y;
                    MovementVector.Y = MovementVector.Y * 0.0f;
                    IsOnGround = true;
                }
            }

            if (Bound().Intersects(Level.Player.Bound(Direction.Down)) && !Level.Player.isDead && !this.isDead)
            {
                this.Kill();
                Level.Player.MovementVector.Y = 1;
            }
            else if (Level.Player.Bound(Direction.Left).Intersects(this.Bound()) 
                        || Level.Player.Bound(Direction.Right).Intersects(this.Bound()) 
                        && !Level.Player.isDead)
            {
                Level.Player.Kill();
            }
        }
        
        public override void Unload()
        {
            Level.Enemies.Remove(this);
        }
     }
}
