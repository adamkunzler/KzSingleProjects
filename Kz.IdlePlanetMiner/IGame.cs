using Raylib_cs;

namespace Kz.IdlePlanetMiner
{
    public interface IGame
    {
        RenderTexture2D Texture { get; }

        void Update();

        void PreRender();

        void Render();

        void ProcessInputs(float cameraZoom);

        void End();
    }
}