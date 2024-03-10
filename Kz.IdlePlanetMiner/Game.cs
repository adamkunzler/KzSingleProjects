using Raylib_cs;

namespace Kz.IdlePlanetMiner
{
    public class Game : IGame
    {
        #region ctor

        private WindowSettings _settings;
        private RenderTexture2D _target;

        public RenderTexture2D Texture => _target;

        public Game(WindowSettings settings)
        {
            _settings = settings;
            _target = Raylib.LoadRenderTexture(_settings.ScreenWidth, _settings.ScreenHeight);
        }

        #endregion ctor

        #region IGame

        public void ProcessInputs()
        {
            // do stuff here
        }

        public void Update()
        {
            // do stuff here
        }

        public void Render()
        {
            Raylib.BeginTextureMode(_target);
            Raylib.ClearBackground(Color.Black);

            Raylib.DrawCircleLines(_settings.HalfScreenWidth, _settings.HalfScreenHeight, 50.0f, Color.Purple);

            Raylib.EndTextureMode();
        }

        public void End()
        {
            Raylib.UnloadRenderTexture(_target);
        }

        #endregion IGame
    }
}