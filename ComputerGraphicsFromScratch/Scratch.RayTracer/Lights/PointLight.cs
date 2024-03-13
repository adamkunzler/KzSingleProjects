using Kz.Engine.DataStructures;

namespace Scratch.RayTracer.Lights
{
    public class PointLight : ILight
    {
        public float Intensity { get; init; }

        public Vector3f Position { get; init; }

        public PointLight()
        {
            Intensity = 1.0f;
            Position = Vector3f.Zero;
        }

        public PointLight(float intensity, Vector3f position)
        {
            Intensity = intensity;
            Position = position;
        }
    }
}