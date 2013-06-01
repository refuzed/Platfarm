using System;
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
            CurrentPosition = startPosition;
        }

        public void Update(GameTime gameTime)
        {
            if (isDead)
                DeathCountdown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
                Move(gameTime);
        }

        public virtual void Move(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            CheckCollision();
            ApplyPhysics(elapsed);

            PreviousPosition = CurrentPosition;
            CurrentPosition.X += Convert.ToInt32(MovementVector.X);
            CurrentPosition.Y -= Convert.ToInt32(MovementVector.Y);
        }

        private void ApplyPhysics(float elapsed)
        {
            // Deal with falling
            if (!IsOnGround)
            {
                MovementVector.Y -= Friction.Y * elapsed;
            }
        }

        public override void CheckCollision()
        {
            base.CheckCollision();


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
