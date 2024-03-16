using Kz.Engine.DataStructures;
using Kz.Engine.Geometry2d.Utils;
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
                    var color = TraceRay(_camera, direction, 1.0f, float.MaxValue, 3);
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

        private RGBColor TraceRay(Vector3f start, Vector3f direction, float tMin, float tMax, int recursiveDepth)
        {
            var intersection = ClosestIntersection(start, direction, tMin, tMax);
            if (intersection.ClosestSphere == null) return BACKGROUND_COLOR;

            var intersectionPoint = start + (intersection.ClosestT * direction);
            var normal = intersection.ClosestSphere.GetNormal(intersectionPoint);
            
            var lightIntensity = ComputeLighting(intersectionPoint, normal, -1.0f * direction, intersection.ClosestSphere.Specular, tMax);
            var localColor = intersection.ClosestSphere.Color * lightIntensity;
            
            // recursion limit
            var reflective = intersection.ClosestSphere.Reflective;
            if(recursiveDepth <= 0 || reflective <= 0)
            {
                return localColor;
            }

            // reflections
            var reflectRay = ReflectRay(-1.0f * direction, normal);
            var reflectedColor = TraceRay(intersectionPoint, direction, 0.001f, float.PositiveInfinity, recursiveDepth - 1);

            return localColor * (1.0f - reflective) + (reflectedColor * reflective);
                
        }
        
        private (Sphere ClosestSphere, float ClosestT) ClosestIntersection(Vector3f start, Vector3f direction, float tMin, float tMax)
        {
            var closestT = float.PositiveInfinity;
            Sphere closestSphere = null!;

            foreach (var sphere in _scene.Spheres)
            {
                var intersections = sphere.Intersect(start, direction);

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

            return (closestSphere, closestT);
        }

        private Vector3f ReflectRay(Vector3f ray, Vector3f normal)
        {
            var reflect = 2.0f * normal * normal.Dot(ray) - ray;
            return reflect;
        }

        /// <summary>
        /// Calculate the amount of light at an intersection point
        /// </summary>
        /// <param name="intersection">the point of intersection on the shape</param>
        /// <param name="normal">the normal at the point of intersection</param>
        /// <param name="direction">the direction of the point of intersection to the light</param>
        /// <param name="specular">the amount of specular light</param>
        /// <returns>total amount of light at the intersection point</returns>
        private float ComputeLighting(Vector3f intersection, Vector3f normal, Vector3f direction, float specular, float tMax)
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

                // shadow
                var shadowIntersection = ClosestIntersection(intersection, lightVector, 0.001f, tMax);
                if (shadowIntersection.ClosestSphere != null && light is not AmbientLight)
                {
                    continue;
                }
                
                // diffuse
                if(n_dot_l > 0.0f)
                {
                    intensity += light.Intensity * n_dot_l / (normal.Magnitude() * lightVector.Magnitude());
                }

                // specular
                if(specular != -1.0f && light is not AmbientLight)
                {
                    var reflect = ReflectRay(lightVector, normal);
                    var r_dot_v = reflect.Dot(direction);
                    if(r_dot_v > 0.0f)
                    {
                        intensity += light.Intensity * MathF.Pow(r_dot_v / (reflect.Magnitude() * direction.Magnitude()), specular);
                    }
                }
            } // end foreach light

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