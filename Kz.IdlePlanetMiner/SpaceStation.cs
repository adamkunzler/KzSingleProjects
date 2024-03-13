using Kz.Engine.DataStructures;
using Kz.Engine.Geometry2d.Primitives;
using Kz.Engine.Raylib;
using Raylib_cs;
using System.Numerics;

namespace Kz.IdlePlanetMiner
{
    public class SpaceStation
    {
        private Vector2f _position;
        private float _rotation;
        private float _size;
        private Texture2D _texture;

        public SpaceStation(Vector2f position)
        {
            _position = position;
            _rotation = 0.0f;
            _size = 256.0f;
            _texture = Raylib.LoadTexture("Resources/spacestation.png");
        }

        public void Update()
        {
            _rotation += 0.2f;
            if (_rotation > 360.0f) _rotation = 0.0f;
        }

        public void Render()
        {
            var source = new Raylib_cs.Rectangle(0, 0, _texture.Width, _texture.Height);
            var dest = new Raylib_cs.Rectangle(_position.X, _position.Y, _size, _size);
            var origin = new Vector2(_size / 2.0f, _size / 2.0f);
            Raylib.DrawTexturePro(_texture, source, dest, origin, _rotation, Color.White);

            //var aabb = GetBoundingCircle();
            //Gfx.DrawCircle(aabb, Color.DarkGreen);
        }

        private Circle GetBoundingCircle()
        {
            return new Circle(_position.X, _position.Y, _size / 4.0f);
        }
    }
}