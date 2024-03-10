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

        private List<Planet> _planets = [];

        public Game(WindowSettings settings)
        {
            _settings = settings;

            // 00 or 04
            _background = Raylib.LoadTexture("Resources/background_04.png");

            GeneratePlanets(17);
        }

        #endregion ctor

        #region IGame

        public void ProcessInputs()
        {
            // do stuff here
        }

        public void Update()
        {
            foreach(var planet in _planets)
            {
                planet.Update();
            }
        }

        public void Render()
        {
            // render background
            for(var y =-_settings.ScreenWidth; y <= _settings.ScreenWidth; y += _settings.ScreenWidth)
            {
                for (var x = -_settings.ScreenWidth; x <= _settings.ScreenWidth; x += _settings.ScreenWidth)
                {
                    Raylib.DrawTexture(_background, x, y, Color.White);
                }
            }

            // render planets
            foreach (var planet in _planets)
            {
                planet.Render();
            }
        }

        public void End()
        {
            Raylib.UnloadTexture(_background);
        }

        #endregion IGame

        private void GeneratePlanets(int numPlanetsToGenerate)
        {
            var theta = TrigUtil.DegreesToRadians(45.0f);
            var magnitude = 250.0f;
            
            var numPlanets = 0;
            var lastPlanetPosition = new Vector2f(_settings.HalfScreenWidth, _settings.HalfScreenHeight);
            
            do
            {
                var max = 90.0f;
                var min = max / 2.0f;
                var randTheta = (float)_random.NextDouble() * TrigUtil.DegreesToRadians(max) - TrigUtil.DegreesToRadians(min);
                var randMagnitude = (float)_random.NextDouble() * 20.0f - 10.0f;

                theta += TrigUtil.DegreesToRadians(67.5f) + randTheta;
                magnitude += 50.0f + randMagnitude;
                
                var polar = new Vector2f(magnitude, theta);
                var coord = polar.ToCartesian();
                
                var x = coord.X + _settings.HalfScreenWidth;
                var y = coord.Y + _settings.HalfScreenHeight;
                
                var planet = new Planet((uint)numPlanets+1, new Vector2f(x, y));
                //var planet = new Planet(11, new Vector2f(x, y), radius);
                _planets.Add(planet);

                lastPlanetPosition = new Vector2f(x, y);

                numPlanets++;
            } while (numPlanets < numPlanetsToGenerate);
        }
    }
}