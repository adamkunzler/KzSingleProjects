namespace Scratch.Rasterizer
{
    public class ViewPort
    {
        public float Width { get; init; }
        public float Height { get; init; }
        public float DistanceToCamera { get; init; }

        public ViewPort() : this(1.0f, 1.0f, 1.0f)
        {
        }

        public ViewPort(float width, float height, float distanceToCamera)
        {
            Width = width;
            Height = height;
            DistanceToCamera = distanceToCamera;
        }
    }
}