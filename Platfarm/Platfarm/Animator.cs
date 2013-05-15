using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platfarm
{
    public class Animator
    {
        private Animation _animation;
        private int _frame;
        private float _time;

        public void SetAnimation(Animation animation)
        {
            if (_animation == animation)
                return;

            this._animation = animation;
            this._frame = 0;
            this._time = 0.0f;
        }
         
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            _time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (_time > _animation.FrameTime)
            {
                _time -= _animation.FrameTime;

                if (_animation.Loop)
                {
                    _frame = (_frame + 1) % _animation.FrameCount;
                }
                else
                {
                    _frame = Math.Min(_frame + 1, _animation.FrameCount - 1);
                }
            }
            
            var source = new Rectangle((_frame) * _animation.Height, (int)_animation.AnimationType * 16,  _animation.Height, _animation.Height);

            spriteBatch.Draw(_animation.Texture, position, source, Color.White, 0.0f, new Vector2(), 1.0f, spriteEffects, 0.0f);
        }

        private Vector2 GetOrigin()
        {
            return new Vector2(_animation.Width / 2.0f, _animation.Height);
        }
    }
}
