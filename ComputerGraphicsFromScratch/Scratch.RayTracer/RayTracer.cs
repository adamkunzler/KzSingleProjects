using Kz.Engine.DataStructures;
using Raylib_cs;
using Scratch.RayTracer.Lights;
using Scratch.RayTracer.Shapes;

namespace Scratch.RayTracer
{
    public class RayTracer
    {
        public static readonly RGBColor BACKGROUND_COLOR = new RGBColor(0, 0, 0);

        private Canvas _canvas;
        private ViewPort _viewport;
        private Vector3f _camera;
        private Scene _scene;

        public RenderTexture2D CanvasTexture => _canvas.Target;

        public RayTracer(int screenWidth, int screenHeight, int scale, Scene scene)
        {
            _canvas = new Canvas(screenWidth / scale, screenHeight / scale);
            _camera = new Vector3f(0, 0, 0);
            _viewport = new ViewPort();

            _scene = scene;
        }

        public void Update()
        {
        }

        public void Render()
        {
            _canvas.BeginRendering();

            for (var y = _canvas.Top; y < _canvas.Bottom; y++)
            {
                for (var x = _canvas.Left; x < _canvas.Right; x++)
                {
                    var direction = CanvasToViewPort(x, y);
                    var color = TraceRay(direction, 1.0f, float.MaxValue);
                    _canvas.PutPixel(x, y, color);
                }
            }

            _canvas.EndRendering();
        }

        public void CleanUp()
        {
            _canvas.CleanUp();
        }

        #region Rendering

        private RGBColor TraceRay(Vector3f direction, float tMin, float tMax)
        {
            var closestT = float.PositiveInfinity;
            Sphere closestSphere = null!;

            foreach (var sphere in _scene.Spheres)
            {
                var intersections = sphere.Intersect(_camera, direction);

                if (intersections.T1 >= tMin && intersections.T1 <= tMax && intersections.T1 < closestT)
                {
                    closestT = intersections.T1;
                    closestSphere = sphere;
                }

                if (intersections.T2 >= tMin && intersections.T2 <= tMax && intersections.T2 < closestT)
                {
                    closestT = intersections.T2;
                    closestSphere = sphere;
                }
            }

            if (closestSphere == null) return BACKGROUND_COLOR;

            var intersectionPoint = _camera + (closestT * direction);
            var normal = closestSphere.GetNormal(intersectionPoint);
            var lightIntensity = ComputeLighting(intersectionPoint, normal);
            var color = closestSphere.Color * lightIntensity;
            return color;
                
        }

        private float ComputeLighting(Vector3f intersection, Vector3f normal)
        {
            var intensity = 0.0f;
            var lightVector = Vector3f.Zero; // intersection point to light
            var n_dot_l = -1.0f;

            foreach (var light in _scene.Lights)
            {
                switch (light)
                {
                    case AmbientLight l:
                        intensity += l.Intensity;
                        break;

                    case PointLight l:
                        lightVector = l.Position - intersection;
                        n_dot_l = normal.Dot(lightVector);
                        break;

                    case DirectionalLight l:
                        lightVector = l.Direction;
                        n_dot_l = normal.Dot(lightVector);
                        break;
                }

                if(n_dot_l > 0.0f)
                {
                    intensity += light.Intensity * n_dot_l / (normal.Magnitude() * lightVector.Magnitude());
                }
            }

            return intensity;
        }

        #endregion Rendering

        #region Translation Methods

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

        #endregion Translation Methods
    }
}