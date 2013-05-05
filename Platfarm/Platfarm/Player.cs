using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platfarm
{
    public class Player : GameEntity
    {
        private bool isJumping;
        private bool disableX;

        public Player(Level level)
        {
            Level = level;
            Texture = Level.Content.Load<Texture2D>("square");
            CurrentPosition = Level.StartPosition;
            Size = new Vector2(20, 20);
            MovementVector = new Vector2(0,0);
            Speed = new Vector2(3.0f, 3.0f);
            Friction = new Vector2(2.0f, 5.0f);
            MaxSpeed = new Vector2(1.0f, 2.0f);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            GetInputs(keyboardState);
            Move(gameTime);
        }

        private void GetInputs(KeyboardState keyboardState)
        {
            Direction = Direction.None;

            if (!disableX)
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                    Direction = Direction.Left;
                if (keyboardState.IsKeyDown(Keys.Right))
                    Direction = Direction.Right;
            }

            if (!isJumping)
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    CurrentPosition.Y -= 1;
                    MovementVector.Y = Speed.Y;
                    isJumping = true;
                }
        }

        private void Move(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            PreviousPosition = CurrentPosition;

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

            CheckCollision();
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
                // If we hit something, reset position to last known good and cut speed in half
                if (Bound().Intersects(levelObject))
                {
                    CurrentPosition = PreviousPosition;
                    MovementVector.Y = MovementVector.Y*0.5f;
                    disableX = true;
                }

                // See if maybe we landed on our feet
                if (Bound(Direction.Down).Intersects(levelObject))
                {
                    IsOnGround = true;
                    isJumping = false;
                    disableX = false;
                }
            }

            foreach (var enemy in Level.Enemies)
            {
                if (Bound(Direction.Down).Intersects(enemy.Bound()))
                {
                    enemy.Kill();
                }
                else if (Bound(Direction.Left).Intersects(enemy.Bound()) ||
                         Bound(Direction.Right).Intersects(enemy.Bound()))
                {
                    this.Kill();
                }
            }

            if (Bound().Intersects(Level.ExitBox))
                CurrentPosition = Level.StartPosition;
        }

        private void Kill()
        {
            CurrentPosition = Level.StartPosition;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, (int)Size.X, (int)Size.Y), Color.Blue);
        }
    }
}
