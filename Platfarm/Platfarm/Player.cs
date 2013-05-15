using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platfarm
{
    public class Player : GameEntity
    {
        private bool _isJumping;

        public Player(Level level)
        {
            Level = level;
            Texture = Level.Content.Load<Texture2D>("Mario");
            CurrentPosition = Level.StartPosition;
            Size = new Vector2(16, 16);
            Color = Color.Blue;

            Sprite = new Animator();
            Animations = new[]
                {
                    new Animation(this, AnimationType.Stand, 1, 0.5f, false),
                    new Animation(this, AnimationType.Run,   2, 0.25f, true),
                    new Animation(this, AnimationType.Jump,  2, 0.5f, false),
                    new Animation(this, AnimationType.Death, 1, 0.5f, false)
                }.ToDictionary(x => x.AnimationType);
            Sprite.SetAnimation(Animations[AnimationType.Stand]);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            if (isDead)
            {
                DeathCountdown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                GetInputs(keyboardState);
                Move(gameTime);
            }
        }

        private void GetInputs(KeyboardState keyboardState)
        {
            Direction = Direction.None;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                Direction = Direction.Left;
                Flip = SpriteEffects.FlipHorizontally;
                if (!_isJumping)
                    Sprite.SetAnimation(Animations[AnimationType.Run]);
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                Direction = Direction.Right;
                Flip = SpriteEffects.None;
                if (!_isJumping)
                    Sprite.SetAnimation(Animations[AnimationType.Run]);
            }
            if (!keyboardState.IsKeyDown(Keys.Right) && !keyboardState.IsKeyDown(Keys.Left))
            {
                Direction = Direction.None;
                if (!_isJumping)
                    Sprite.SetAnimation(Animations[AnimationType.Stand]);
            }

            if (!_isJumping)
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    CurrentPosition.Y -= 1;
                    MovementVector.Y = Speed.Y;
                    _isJumping = true;
                    Sprite.SetAnimation(Animations[AnimationType.Jump]);
                }
        }

        private void Move(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            PreviousPosition = CurrentPosition;

            CheckCollision();

            switch (Direction)
            {
                case Direction.Left:
                    MovementVector.X -= Speed.X * elapsed;
                    ApplyPhysics(elapsed, false);
                    break;
                case Direction.Right:
                    MovementVector.X += Speed.X * elapsed;
                    ApplyPhysics(elapsed, false);
                    break;
                default:
                    ApplyPhysics(elapsed, true);
                    break;
            }

            // Cap run speed
            if (MovementVector.X > MaxSpeed.X) MovementVector.X = MaxSpeed.X;
            if (MovementVector.X < -MaxSpeed.X) MovementVector.X = -MaxSpeed.X;


            CurrentPosition.X += MovementVector.X;
            CurrentPosition.Y -= MovementVector.Y;

        }

        private void ApplyPhysics(float elapsed, bool checkNeutral)
        {
            // Skip checking min friction if we're actively trying to accelerate
            if (checkNeutral && MovementVector.X < Friction.X * elapsed && MovementVector.X > -Friction.X * elapsed)
                MovementVector.X = 0;
            else
            {
                if (MovementVector.X < 0)
                    MovementVector.X += Friction.X*elapsed;
                if (MovementVector.X > 0)
                    MovementVector.X -= Friction.X*elapsed;
            }

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
                    MovementVector.X = 0.0f;
                }

                if (Bound(Direction.Up).Intersects(levelObject))
                {
                    CurrentPosition.Y = PreviousPosition.Y;
                    MovementVector.Y = -1.0f;
                }

                if (Bound(Direction.Down).Intersects(levelObject))
                {
                    CurrentPosition.Y = PreviousPosition.Y;
                    MovementVector.Y = 0.0f;
                    IsOnGround = true;
                    _isJumping = false;
                }
            }

            if (Bound().Intersects(Level.ExitBox))
                CurrentPosition = Level.StartPosition;
        }

        public override void Unload()
        {
            Level.Player = new Player(Level);
        }
    }
}
