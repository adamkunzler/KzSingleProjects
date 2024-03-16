using Kz.Engine.DataStructures;
using Raylib_cs;

namespace Scratch.Rasterizer
{
    public class Canvas
    {
        #region ctor

        public int Width { get; init; }
        public int Height { get; init; }

        private int _left => -Width / 2;
        public int Left => _left;

        private int _right => Width / 2;
        public int Right => _right;

        private int _top => -Height / 2;
        public int Top => _top;

        private int _bottom => Height / 2;
        public int Bottom => _bottom;

        private RenderTexture2D _target;
        public RenderTexture2D Target => _target;

        public Canvas(int width, int height)
        {
            Width = width;
            Height = height;

            _target = Raylib.LoadRenderTexture(width, height);
        }

        #endregion ctor

        public void PutPixel(int canvasX, int canvasY, RGBColor color)
        {
            var coords = CanvasToScreen(canvasX, canvasY);
            Raylib.DrawPixel((int)coords.X, (int)coords.Y, color.ToColor());
        }

        public void BeginRendering()
        {
            Raylib.BeginTextureMode(_target);
            Raylib.ClearBackground(Color.Black);
        }

        public void EndRendering()
        {
            Raylib.EndTextureMode();
        }

        /// <summary>
        /// Convert canvas coordinates to screen coordinates
        /// </summary>
        public Vector2f CanvasToScreen(int canvasX, int canvasY)
        {
            var screenX = Right + canvasX;
            var screenY = Bottom - canvasY;
            return new Vector2f(screenX, screenY);
        }

        public void CleanUp()
        {
            Raylib.UnloadRenderTexture(_target);
        }
    }
}