using Kz.Engine.DataStructures;

namespace Scratch.RayTracer.Lights
{
    public class DirectionalLight : ILight
    {
        public float Intensity { get; init; }

        public Vector3f Direction { get; set; }

        public DirectionalLight()
        {
            Intensity = 1.0f;
            Direction = Vector3f.Zero;
        }

        public DirectionalLight(float intensity, Vector3f direction)
        {
            Intensity = intensity;
            Direction = direction;
        }
    }
}