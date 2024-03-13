namespace Scratch.RayTracer.Lights
{
    public class AmbientLight : ILight
    {
        public float Intensity { get; init; }

        public AmbientLight()
        {
            Intensity = 1.0f;
        }

        public AmbientLight(float intensity)
        {
            Intensity = Intensity;
        }
    }
}