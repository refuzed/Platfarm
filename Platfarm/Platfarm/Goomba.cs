using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platfarm
{
    public class Goomba : Enemy
    {
        public Goomba(Level level, Vector2 startPosition) : base(level, startPosition)
        {
            Texture = level.Content.Load<Texture2D>("Goomba");
            Size = new Vector2(16, 16);
            Speed = new Vector2(1.0f, 3.0f);
            Direction = Direction.Left;
            Sprite = new Animator();
            Animations = new[]
                {
                    new Animation(this, AnimationType.Run,   2, 0.5f, true),
                    new Animation(this, AnimationType.Death, 1, 0.5f, false)
                }.ToDictionary(x => x.AnimationType);
            Sprite.SetAnimation(Animations[AnimationType.Run]);
        }

        public override void Move(GameTime gameTime)
        {
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

            base.Move(gameTime);
        }
    }
}
