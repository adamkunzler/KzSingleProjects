using Kz.Engine.DataStructures;
using Raylib_cs;
using System.Windows.Markup;

namespace Scratch.Rasterizer
{
    public class Rasterizer
    {
        public static readonly RGBColor BACKGROUND_COLOR = new RGBColor(0, 0, 0);

        private Canvas _canvas;
        private ViewPort _viewport;
        private Vector3f _camera;

        public RenderTexture2D CanvasTexture => _canvas.Target;

        public Rasterizer(int screenWidth, int screenHeight, int scale)
        {
            _canvas = new Canvas(screenWidth / scale, screenHeight / scale);
            _camera = new Vector3f(0, 0, 0);
            _viewport = new ViewPort();
        }

        public void Update()
        {
        }

        public void Render()
        {
            _canvas.BeginRendering();

            DrawLine(new Vector2f(0, 0), new Vector2f(50, 100), new RGBColor(0, 255, 0));
            DrawLine(new Vector2f(0, 0), new Vector2f(100, 50), new RGBColor(0, 0, 255));

            _canvas.EndRendering();
        }

        public void CleanUp()
        {
            _canvas.CleanUp();
        }

        /// <summary>
        /// Convert canvas coordinates to viewport coordinates
        /// </summary>
        public Vector3f CanvasToViewPort(int canvasX, int canvasY)
        {
            var vx = canvasX * (_viewport.Width / _canvas.Width);
            var vy = canvasY * (_viewport.Height / _canvas.Height);
            var vz = _viewport.DistanceToCamera;

            return new Vector3f(vx, vy, vz);
        }

        #region Drawing Things
        
        private int[] Interpolate(float i0, float d0, float i1, float d1)
        {
            if(i0 == i1) 
            {
                return [(int)d0];
            }

            var size = (int)(i1 - i0) + 1;
            var values = new int[size];
            var a = (d1 - d0) / (i1 - i0);
            var d = d0;

            var index = 0;
            for (var i = i0; i <= i1; i++)
            {
                values[index++] = (int)d;
                d += a;
            }

            return values;
        }

        private void DrawLine(Vector2f p0, Vector2f p1, RGBColor color)
        {
            if(MathF.Abs(p1.X - p0.X) > MathF.Abs(p1.Y - p0.Y))
            {
                // line is horizontal-ish
                if(p0.X > p1.X)
                {
                    (p0, p1) = (p1, p0);
                }

                var ys = Interpolate(p0.X, p0.Y, p1.X, p1.Y);
                for(var x = p0.X; x <= p1.X; x++)
                {
                    var index = (int)(x - p0.X);
                    _canvas.PutPixel((int)x, ys[index], color);
                }
            }
            else
            {
                // line is vertical-ish
                if (p0.Y > p1.Y)
                {
                    (p0, p1) = (p1, p0);
                }

                var xs = Interpolate(p0.Y, p0.X, p1.Y, p1.X);
                for (var y = p0.Y; y <= p1.Y; y++)
                {
                    var index = (int)(y - p0.Y);
                    _canvas.PutPixel(xs[index], (int)y, color);
                }
            }
        }

        #endregion Drawing Things
    }
}