namespace Scratch.RayTracer
{
    public class ViewPort
    {
        public float Width { get; init; }
        public float Height { get; init; }
        public float DistanceToCamera { get; init; }

        public ViewPort()
        {
            Width = 1.0f;
            Height = 1.0f;
            DistanceToCamera = 1.0f;
        }
    }
}