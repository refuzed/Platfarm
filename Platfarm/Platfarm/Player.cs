using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platfarm
{
    public class Player : GameEntity
    {
        public Player(Level level)
            : base(level)
        {
            Texture = Level.Content.Load<Texture2D>("Mario");
            CurrentPosition = Level.StartPosition;
            Size = new Vector2(16, 16);
            Speed = new Vector2(5.0f, 3.3f);

            Sprite = new Animator();
            Animations = new[]
                {
                    new Animation(this, AnimationType.Stand, 1, 0.5f, false),
                    new Animation(this, AnimationType.Run,   2, 0.25f, true),
                    new Animation(this, AnimationType.Jump,  2, 0.5f, false),
                    new Animation(this, AnimationType.Death, 1, 0.5f, false),
                    new Animation(this, AnimationType.Skid,  1, 0.5f, false)
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
                Level.DrawWindowX = this.CurrentPosition.X > 200 ? ((int)this.CurrentPosition.X - 200) * Level.Scale : 0;
            }
        }

        private void GetInputs(KeyboardState keyboardState)
        {
            Direction = Direction.None;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                Direction = Direction.Left;
                Flip = SpriteEffects.FlipHorizontally;
                if (!IsJumping)
                    Sprite.SetAnimation(Animations[AnimationType.Run]);
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                Direction = Direction.Right;
                Flip = SpriteEffects.None;
                if (!IsJumping)
                    Sprite.SetAnimation(Animations[AnimationType.Run]);
            }
            if (!keyboardState.IsKeyDown(Keys.Right) && !keyboardState.IsKeyDown(Keys.Left))
            {
                Direction = Direction.None;
                if (!IsJumping)
                {
                    if (MovementVector.X != 0.0f)
                        Sprite.SetAnimation(Animations[AnimationType.Skid]);
                    else
                        Sprite.SetAnimation(Animations[AnimationType.Stand]);
                }
            }

            if (!IsJumping)
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    CurrentPosition.Y -= 1;
                    MovementVector.Y = Speed.Y;
                    IsJumping = true;
                    Sprite.SetAnimation(Animations[AnimationType.Jump]);
                }
        }

        private void Move(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

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

            PreviousPosition = CurrentPosition;
            CurrentPosition.X += Convert.ToInt32(MovementVector.X);
            CurrentPosition.Y -= Convert.ToInt32(MovementVector.Y);

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

        public override void Unload()
        {
            Level.Player = new Player(Level);
            base.Unload();
        }
    }
}
