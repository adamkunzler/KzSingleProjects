using Kz.Engine.DataStructures;
using Raylib_cs;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace Scratch.Rasterizer
{
    public record TriangleRef(int v0, int v1, int v2, RGBColor color);

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
            _viewport = new ViewPort(1.0f, 1.0f, 1.0f);
            _camera = new Vector3f(0, 0, 0);
        }

        public void Update()
        {
        }

        private RGBColor _yellow = new RGBColor(245, 174, 45);
        private RGBColor _green = new RGBColor(80, 148, 82);
        private RGBColor _black = new RGBColor(29, 27, 24);
        
        private RGBColor _red = new RGBColor(255, 0, 0);
        private RGBColor _green2 = new RGBColor(0, 255, 0);
        private RGBColor _blue = new RGBColor(0, 0, 255);
        private RGBColor _yellow2 = new RGBColor(255, 255, 0);
        private RGBColor _purple = new RGBColor(255, 0, 255);
        private RGBColor _cyan = new RGBColor(0, 255, 255);

        public void Render()
        {
            _canvas.BeginRendering();

            //DemoShadedTriangle();
            DemoRenderCube();
            
            _canvas.EndRendering();
        }

        private void DemoShadedTriangle()
        {
            var p0 = new Vector2f(-200, -250);
            var p1 = new Vector2f(200, 50);
            var p2 = new Vector2f(20, 250);


            DrawTriangleShaded(p0, p1, p2, 0.25f, 0.1f, 1.0f, _green);
            //DrawTriangleLines(p0, p1, p2, _yellow);
            //DrawTriangle(p0, p1, p2, _yellow);
        }

        private void DemoRenderCube()
        {
            // define vertices
            var v0 = new Vector3f(1, 1, 1);
            var v1 = new Vector3f(-1, 1, 1);
            var v2 = new Vector3f(-1, -1, 1);
            var v3 = new Vector3f(1, -1, 1);
            var v4 = new Vector3f(1, 1, -1);
            var v5 = new Vector3f(-1, 1, -1);
            var v6 = new Vector3f(-1, -1, -1);
            var v7 = new Vector3f(1, -1, -1);
            var vertices = new List<Vector3f> { v0, v1, v2,v3,v4, v5, v6, v7 };

            // define triangles and colors
            var t0 = new TriangleRef(0, 1, 2, _red);
            var t1 = new TriangleRef(0, 2, 3, _red);
            var t2 = new TriangleRef(4, 0, 3, _green2);
            var t3 = new TriangleRef(4, 3, 7, _green2);
            var t4 = new TriangleRef(5, 4, 7, _blue);
            var t5 = new TriangleRef(5, 7, 6, _blue);
            var t6 = new TriangleRef(1, 5, 6, _yellow2);
            var t7 = new TriangleRef(1, 6, 2, _yellow2);
            var t8 = new TriangleRef(4, 5, 1, _purple);
            var t9 = new TriangleRef(4, 1, 0, _purple);
            var t10 = new TriangleRef(2, 6, 7, _cyan);
            var t11 = new TriangleRef(2, 7, 3, _cyan);
            var triangles = new List<TriangleRef> { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 };

            // translate to the left
            var translation = new Vector3f(-1.5f, 0.0f, 7.0f);
            for(var i = 0; i < vertices.Count; i++)
            {
                vertices[i] += translation;
            }

            RenderObject(vertices, triangles);
        }

        public void CleanUp()
        {
            _canvas.CleanUp();
        }

        #region Camera/Screen/World

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
        
        private Vector2f ViewportToCanvas(Vector2f vertex)
        {
            Vector2f canvasCoords;
            canvasCoords.X = vertex.X * (_canvas.Width / _viewport.Width);
            canvasCoords.Y = vertex.Y * (_canvas.Height/ _viewport.Height);
            return canvasCoords;
        }

        private Vector2f ProjectVertex(Vector3f vertex)
        {
            Vector2f projection;
            projection.X = vertex.X * _viewport.DistanceToCamera / vertex.Z;
            projection.Y = vertex.Y * _viewport.DistanceToCamera / vertex.Z;
            return ViewportToCanvas(projection);
        }

        #endregion Camera/Screen/World

        #region Utils

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

        private float[] Interpolate
        (
            float independent0, float dependant0, 
            float independent1, float dependant1            
        )
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

        #endregion Utils

        #region Drawing Things

        private void RenderObject(List<Vector3f> vertices, List<TriangleRef> triangles)
        {
            var projected = new List<Vector2f>();
            foreach(var v in vertices)
            {
                projected.Add(ProjectVertex(v));
            }

            foreach(var t in triangles)
            {
                RenderTriangle(t, projected);
            }
        }

        private void RenderTriangle(TriangleRef triangle, List<Vector2f> projectedVertices)
        {
            DrawTriangleLines(
                projectedVertices[triangle.v0],
                projectedVertices[triangle.v1],
                projectedVertices[triangle.v2],
                triangle.color
            );
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
            //x01 = RemoveLastElement(x01);
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
            //x01 = RemoveLastElement(x01);
            var x012 = CombineArrays(x01, x12);

            //h01 = RemoveLastElement(h01);
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