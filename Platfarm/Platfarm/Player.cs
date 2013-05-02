using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Platfarm
{
    public class Player
    {
        private Level level;
        private Texture2D playerTexture;

        private bool isJumping;
        private bool feetOnTheGround;
        private bool disableX;

        private Direction playerDirection;
        private Vector2 currentPosition;
        private Vector2 previousPosition;

        private Vector2 size;
        private Vector2 direction;
        private Vector2 speed;
        private Vector2 maxSpeed;
        private Vector2 friction;

        public Player(Level level)
        {
            this.level = level;
            SetupPlayer();
        }

        private void SetupPlayer()
        {
            currentPosition = level.StartPosition;
            playerTexture = level.content.Load<Texture2D>("square");

            size = new Vector2(20, 20);
            direction = new Vector2(0,0);
            speed = new Vector2(3.0f, 3.0f); 
            maxSpeed = new Vector2(1.0f, 2.0f);
            friction = new Vector2(2.0f, 5.0f);
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            playerDirection = Direction.None;

            if(!disableX)
            {
                if (keyboardState.IsKeyDown(Keys.Left))
                    playerDirection = Direction.Left;
                if (keyboardState.IsKeyDown(Keys.Right))
                    playerDirection = Direction.Right;
            }

            if (!isJumping)
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    currentPosition.Y -= 1;
                    direction.Y = speed.Y;
                    isJumping = true;
                }

            LetsGetPhysical(gameTime);
        }

        private enum Direction
        {
            None, Left, Right, Up, Down
        }

        private void LetsGetPhysical(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            previousPosition = currentPosition;

            switch (playerDirection)
            {
                case Direction.Left:
                    direction.X -= speed.X * elapsed;
                    ApplyFriction(elapsed, false);
                    break;
                case Direction.Right:
                    direction.X += speed.X * elapsed;
                    ApplyFriction(elapsed, false);
                    break;
                default:
                    ApplyFriction(elapsed, true);
                    break;
            }

            // Cap run speed
            if (direction.X > maxSpeed.X) direction.X = maxSpeed.X;
            if (direction.X < -maxSpeed.X) direction.X = -maxSpeed.X;

            currentPosition.X += direction.X;
            currentPosition.Y -= direction.Y;

            CheckCollision();
        }

        private void ApplyFriction(float elapsed, bool checkNeutral)
        {
            // Skip checking min friction if we're actively trying to accelerate
            if (checkNeutral && direction.X < friction.X * elapsed && direction.X > -friction.X * elapsed)
                direction.X = 0;
            else
            {
                if (direction.X < 0)
                    direction.X += friction.X*elapsed;
                if (direction.X > 0)
                    direction.X -= friction.X*elapsed;
            }

            // Deal with falling
            if (!feetOnTheGround)
            {
                direction.Y -= friction.Y * elapsed;
            }
        }

        private void CheckCollision()
        {
            feetOnTheGround = false;

            foreach (var levelObject in level.LevelObjects)
            {
                // If we hit something, reset position to last known good and cut speed in half
                if (Bound().Intersects(levelObject))
                {
                    currentPosition = previousPosition;
                    direction.Y = direction.Y*0.5f;
                    disableX = true;
                }

                // See if maybe we landed on our feet
                if (Bound(Direction.Down).Intersects(levelObject))
                {
                    feetOnTheGround = true;
                    isJumping = false;
                    disableX = false;
                }
            }

            if (Bound().Intersects(level.ExitBox))
                currentPosition = level.StartPosition;
        }

        private Rectangle Bound(Direction direction = Direction.None)
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

            return new Rectangle((int)currentPosition.X + xAdjust, (int)currentPosition.Y + yAdjust, (int)size.X, (int)size.Y);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(playerTexture, new Rectangle((int)currentPosition.X, (int)currentPosition.Y, (int)size.X, (int)size.Y), Color.Red);
        }
    }
}
