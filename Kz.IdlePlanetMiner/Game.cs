using Kz.Engine.DataStructures;
using Kz.Engine.Trigonometry;
using Raylib_cs;
using System.Numerics;
using System.Security.Cryptography;

namespace Kz.IdlePlanetMiner
{
    public class Game : IGame
    {
        #region ctor

        private Random _random = new Random(117);
        private WindowSettings _settings;        
                
        public RenderTexture2D Texture => throw new NotImplementedException();
        public Texture2D _background;

        private SpaceStation _spaceStation;
        private List<Planet> _planets = [];

        public Game(WindowSettings settings)
        {
            _settings = settings;

            // 00 or 04
            _background = Raylib.LoadTexture("Resources/background_04.png");

            _spaceStation = new SpaceStation(new Vector2f(_settings.HalfScreenWidth, _settings.HalfScreenHeight));

            _planets = PlanetManager.GetAllPlanets();
        }

        #endregion ctor

        #region IGame

        public void ProcessInputs()
        {
            // do stuff here
        }

        public void Update()
        {
            _spaceStation.Update();

            foreach(var planet in _planets)
            {
                planet.Update();
            }
        }

        public void PreRender()
        {
            // render background
            var xSize = 4.0f * _settings.ScreenWidth;
            var ySize = 4.0f * _settings.ScreenHeight;
            for (var y = -ySize; y <= ySize; y += _settings.ScreenHeight)
            {
                for (var x = -xSize; x <= xSize; x += _settings.ScreenWidth)
                {
                    Raylib.DrawTexture(_background, (int)x, (int)y, Color.White);
                }
            }
        }

        public void Render()
        {            
            _spaceStation.Render();

            // render planets
            foreach (var planet in _planets)
            {
                planet.Render(_settings.HalfScreenWidth, _settings.HalfScreenHeight);
            }
        }

        public void End()
        {
            Raylib.UnloadTexture(_background);
        }

        #endregion IGame

        

        
    }
}