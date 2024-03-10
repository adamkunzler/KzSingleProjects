using Kz.Engine.DataStructures;

namespace Kz.IdlePlanetMiner
{
    public class Planet
    {
        #region ctor

        private uint _id;
        private Vector2f _position;
        private Sprite _sprite;

        public Planet(uint id, Vector2f position)
        {
            _id = id;
            _position = position;
            _sprite = new Sprite(
                $"Resources/Planets/{_id:000}.png",
                new List<uint> { 6, 9, 11 }.Contains(_id) ? new Vector2f(256, 256) : new Vector2f(128, 128),
                new Vector2f(30, 30),
                1.0f / 30.0f);
        }

        #endregion ctor

        #region Public Methods
        
        public void Update()
        {
            _sprite.Update();
        }

        public void Render()
        {
            _sprite.Render(_position);
        }

        #endregion Public Methods
    }
}