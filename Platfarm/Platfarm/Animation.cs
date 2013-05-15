using Microsoft.Xna.Framework.Graphics;

namespace Platfarm
{
    public class Animation
    {
        public AnimationType AnimationType
        {
            get { return _animationType; }
        }
        private readonly AnimationType _animationType;

        public Texture2D Texture
        {
            get { return _texture; }
        }
        private readonly Texture2D _texture;

        public int FrameCount
        {
            get { return _frameCount; }
        }
        private readonly int _frameCount;

        public float FrameTime
        {
            get { return _frameTime; }
        }
        private readonly float _frameTime;

        public bool Loop
        {
            get { return _loop; }
        }
        private readonly bool _loop;

        public int Width
        {
            get { return (int)_gameEntity.Size.X; }
        }

        public int Height
        {
            get { return (int) _gameEntity.Size.Y; }
        }

        private readonly GameEntity _gameEntity;

        public Animation(GameEntity gameEntity, AnimationType animationType, int frameCount, float frameTime, bool loop)
        {
            _gameEntity = gameEntity; 
            _texture = gameEntity.Texture;
            _animationType = animationType;
            _frameCount = frameCount;
            _frameTime = frameTime;
            _loop = loop;
        }
    }
}
