using Kz.Engine.DataStructures;

namespace Scratch.RayTracer.Shapes
{
    public class Sphere
    {
        public Vector3f Center { get; init; }

        public float Radius { get; init; }

        public RGBColor Color { get; init; }

        public float Specular { get; init; }

        public float Reflective { get; init; }


        public Sphere(Vector3f center, float radius, RGBColor color, float specular, float reflective)
        {
            Center = center;
            Radius = radius;
            Color = color;
            Specular = specular;
            Reflective = reflective;
        }

        public Vector3f GetNormal(Vector3f point)
        {
            var normal = (point - Center).Normal();
            return normal;
        }

        public (float T1, float T2) Intersect(Vector3f camera, Vector3f direction)
        {
            var r = Radius;
            var centerCamera = camera - Center;

            var a = direction.Dot(direction);
            var b = 2.0f * centerCamera.Dot(direction);
            var c = centerCamera.Dot(centerCamera) - (r * r);

            var discriminant = (b * b) - (4.0f * a * c);
            if (discriminant < 0)
            {
                return (float.PositiveInfinity, float.PositiveInfinity);
            }

            var t1 = (-b + MathF.Sqrt(discriminant)) / (2.0f * a);
            var t2 = (-b - MathF.Sqrt(discriminant)) / (2.0f * a);
            return (t1, t2);
        }
    }
}