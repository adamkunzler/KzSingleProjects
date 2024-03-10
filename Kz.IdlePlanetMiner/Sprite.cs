using Kz.Engine.DataStructures;
using Raylib_cs;
using System.Numerics;

namespace Kz.IdlePlanetMiner
{
    public class Sprite
    {
        #region ctor

        private Texture2D _texture;

        private string _filename;
        private Vector2f _size;
        private Vector2f _numFrames;
        
        private Vector2f _currentFrame;
        private float _frameTime = 0.0f;
        private float _frameSpeed = 0.0f; 

        public Sprite(string filename, Vector2f frameSize, Vector2f numFrames, float frameSpeed)
        {
            _filename = filename;
            _size = frameSize;
            _numFrames = numFrames;
            _currentFrame = Vector2f.Zero;
            _frameSpeed = frameSpeed;

            _texture = Raylib.LoadTexture(filename);
        }

        #endregion ctor


        public void Update()
        {
            _frameTime += Raylib.GetFrameTime();
            if (_frameTime >= _frameSpeed)
            {
                _currentFrame.X++;
                if (_currentFrame.X >= _numFrames.X)
                {
                    _currentFrame.X = 0;
                    _currentFrame.Y++;
                    if(_currentFrame.Y >= _numFrames.Y)
                    {
                        _currentFrame = Vector2f.Zero;
                    }
                }

                _frameTime = 0;
            }
        }

        public void Render(Vector2f position)
        {            
            var x = _currentFrame.X * _size.X;
            var y = _currentFrame.Y * _size.Y;

            var source = new Rectangle(x, y, _size.X, _size.Y);
            var dest = new Rectangle(position.X, position.Y, _size.X, _size.Y);
            var origin = new Vector2(_size.X / 2.0f, _size.Y / 2.0f);
            Raylib.DrawTexturePro(_texture, source, dest, origin, 0.0f, Color.White);            
        }
    }
}