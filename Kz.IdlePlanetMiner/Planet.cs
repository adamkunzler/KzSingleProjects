using Kz.Engine.DataStructures;
using System.Numerics;

namespace Kz.IdlePlanetMiner
{
    public record PlanetResource(string Name, uint Yield);

    public class Planet
    {
        #region ctor

        private uint _id = 0;
        private Vector2f _position = Vector2f.Zero;
        private Sprite _sprite = null!;
        private float _rotation = 0.0f;

        public uint Group { get; init; }
        public string Name { get; init; } = string.Empty;
        public BigInteger UnlockPrice { get; init; }        
        public List<PlanetResource> Resources { get; init; } = [];
        public float MiningRate { get; private set; } = 1.0f;
        public float ShipSpeed { get; private set; } = 1.0f;
        public float CargoCapacity { get; private set; } = 1.0f;

        public Planet()
        {            
        }

        public void Init(uint id, Vector2f position, float rotation)
        {
            _id = id;
            _position = position;
            _rotation = rotation;
            //_sprite = new Sprite(
            //    $"Resources/Planets/{_id:000}.png",                
            //    new List<uint> { 6, 9, 11 }.Contains(_id) ? new Vector2f(256, 256) : new Vector2f(128, 128),
            //    new Vector2f(30, 30),
            //    1.0f / 30.0f);

            _sprite = new Sprite(
                "Resources/Planets/test.png",
                new Vector2f(128, 128),
                new Vector2f(19, 19),
                1.0f / 30.0f);
        }

        public override string ToString()
        {
            return $"{_id} - {Group} - {Name}";
        }

        #endregion ctor

        #region Public Methods

        public void Update()
        {
            _sprite.Update();
        }

        public void Render(float halfScreenWidth, float halfScreenHeight)
        {
            var toWorld = new Vector2f(_position.X + halfScreenWidth, _position.Y + halfScreenHeight);
            _sprite.Render(toWorld, _rotation);

            var halfWidth = (Name.Length * 20.0f) / 4.0f;
            Raylib_cs.Raylib.DrawText(Name, (int)toWorld.X - (int)halfWidth, (int)toWorld.Y - 100, 20, Raylib_cs.Color.Red);
        }

        #endregion Public Methods
    }
}