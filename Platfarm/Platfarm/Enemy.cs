using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platfarm
{
    public class Enemy : GameEntity
    {
        public Enemy(Level level, Vector2 startPosition)
        {
            Level = level;
            Texture = level.Content.Load<Texture2D>("square");
            CurrentPosition = startPosition;
            Size = new Vector2(20, 20);
            MovementVector = new Vector2(0, 0);
            Speed = new Vector2(1.0f, 3.0f);
            Friction = new Vector2(0.0f, 5.0f);
            Direction = Direction.Left;
        }

        public void Update(GameTime gameTime)
        {
            Move(gameTime);
        }

        private void Move(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            PreviousPosition = CurrentPosition;

            switch (Direction)
            {
                case Direction.Left:
                    MovementVector.X = -Speed.X;
                    break;
                case Direction.Right:
                    MovementVector.X = Speed.X;
                    break;
            }

            ApplyPhysics(elapsed);

            CurrentPosition.X += MovementVector.X;
            CurrentPosition.Y -= MovementVector.Y;

            CheckCollision();
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
                // If we hit something, reset position to last known good and cut speed in half
                if (Bound().Intersects(levelObject))
                {
                    CurrentPosition = PreviousPosition;
                    MovementVector.Y = MovementVector.Y * 0.5f;
                }

                // See if maybe we landed on our feet
                if (Bound(Direction.Down).Intersects(levelObject))
                {
                    IsOnGround = true;
                }
            }
        }

        public void Kill()
        {
            //TODO: Play a death animation
            Level.DeathList.Add(this);
        }

        public void Unload()
        {
            Level.Enemies.Remove(this);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, new Rectangle((int)CurrentPosition.X, (int)CurrentPosition.Y, (int)Size.X, (int)Size.Y), Color.Red);
        }
     }
}
