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

        private RGBColor _yellow = new RGBColor(245, 174, 45);
        private RGBColor _green = new RGBColor(80, 148, 82);
        private RGBColor _black = new RGBColor(29, 27, 24);

        public void Render()
        {
            _canvas.BeginRendering();

            var p0 = new Vector2f(-200, -250);
            var p1 = new Vector2f(200, 50);
            var p2 = new Vector2f(20, 250);


            DrawTriangleShaded(p0, p1, p2, 0.25f, 0.1f, 1.0f, _green);
            //DrawTriangleLines(p0, p1, p2, _yellow);

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
        
        private T[] RemoveLastElement<T>(T[] source)
        {
            var result = new T[source.Length - 1];
            Array.Copy(source, 0, result, 0, source.Length - 1);
            return result;
        }

        private T[] CombineArrays<T>(T[] a, T[] b)
        {
            var result = new T[a.Length + b.Length];

            Array.Copy(a, 0, result, 0, a.Length);
            Array.Copy(b, 0, result, a.Length, b.Length);

            return result;
        }

        private float[] Interpolate(float independent0, float dependant0, float independent1, float dependant1)
        {
            if(independent0 == independent1) 
            {
                return [dependant0];
            }

            var size = (int)(independent1 - independent0) + 1;
            var values = new float[size];
            var a = (dependant1 - dependant0) / (independent1 - independent0);
            var d = dependant0;

            var index = 0;
            for (var i = independent0; i <= independent1; i++)
            {
                values[index++] = d;
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
                    _canvas.PutPixel((int)x, (int)ys[index], color);
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
                    _canvas.PutPixel((int)xs[index], (int)y, color);
                }
            }
        }

        private void DrawTriangleLines(Vector2f p0, Vector2f p1, Vector2f p2, RGBColor color)
        {
            DrawLine(p0, p1, color);
            DrawLine(p1, p2, color);
            DrawLine(p2, p0, color);
        }

        private void DrawTriangle(Vector2f p0, Vector2f p1, Vector2f p2, RGBColor color)
        {
            // sort Ys
            if (p1.Y < p0.Y) (p1, p0) = (p0, p1);
            if (p2.Y < p0.Y) (p2, p0) = (p0, p2);
            if (p2.Y < p1.Y) (p2, p1) = (p1, p2);

            // get x values
            var x01 = Interpolate(p0.Y, p0.X, p1.Y, p1.X);
            var x12 = Interpolate(p1.Y, p1.X, p2.Y, p2.X);
            var x02 = Interpolate(p0.Y, p0.X, p2.Y, p2.X);

            // cleanup repeated x-value
            x01 = RemoveLastElement(x01);
            var x012 = CombineArrays(x01, x12);

            // choose x-left and x-right
            var m = (int)MathF.Floor(x02.Length / 2.0f);
            var xLeft = (x02[m] < x012[m]) ? x02 : x012;
            var xRight = (x02[m] < x012[m]) ? x012 : x02;

            // draw the line segments
            for(var y = p0.Y; y <= p2.Y; y++)
            {
                for(var x = xLeft[(int)(y - p0.Y)]; x <= xRight[(int)(y - p0.Y)]; x++)
                {
                    _canvas.PutPixel((int)x, (int)y, color);
                }
            }
        }

        private void DrawTriangleShaded
        (
            Vector2f p0, Vector2f p1, Vector2f p2, 
            float h0, float h1, float h2,
            RGBColor color
        )
        {
            // sort Ys
            if (p1.Y < p0.Y) (p1, p0) = (p0, p1);
            if (p2.Y < p0.Y) (p2, p0) = (p0, p2);
            if (p2.Y < p1.Y) (p2, p1) = (p1, p2);

            // get x values
            var x01 = Interpolate(p0.Y, p0.X, p1.Y, p1.X);
            var h01 = Interpolate(p0.Y, h0, p1.Y, h1);

            var x12 = Interpolate(p1.Y, p1.X, p2.Y, p2.X);
            var h12 = Interpolate(p1.Y, h1, p2.Y, h2);

            var x02 = Interpolate(p0.Y, p0.X, p2.Y, p2.X);
            var h02 = Interpolate(p0.Y, h0, p2.Y, h2);

            // cleanup repeated values
            x01 = RemoveLastElement(x01);
            var x012 = CombineArrays(x01, x12);

            h01 = RemoveLastElement(h01);
            var h012 = CombineArrays(h01, h12);

            // choose x-left and x-right
            var m = (int)MathF.Floor(x02.Length / 2.0f);
            var xLeft = (x02[m] < x012[m]) ? x02 : x012;
            var xRight = (x02[m] < x012[m]) ? x012 : x02;
            var hLeft = (x02[m] < x012[m]) ? h02 : h012;
            var hRight = (x02[m] < x012[m]) ? h012 : h02;

            // draw the line segments
            for (var y = p0.Y; y <= p2.Y; y++)
            {
                var yMinusP0Y = (int)(y - p0.Y);
                var xl = xLeft[yMinusP0Y];
                var xr = xRight[yMinusP0Y];
                var hSegment = Interpolate(
                    xl, 
                    hLeft[yMinusP0Y], 
                    xr, 
                    hRight[yMinusP0Y]);

                for (var x = xl; x <= xr; x++)
                {
                    var shadedColor = color * hSegment[(int)(x - xl)];
                    _canvas.PutPixel((int)x, (int)y, shadedColor);
                }
            }
        }

        #endregion Drawing Things
    }
}