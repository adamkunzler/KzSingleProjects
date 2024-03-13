using Scratch.RayTracer.Lights;
using Scratch.RayTracer.Shapes;

namespace Scratch.RayTracer
{
    public class Scene
    {
        public List<ILight> Lights { get; set; } = new List<ILight>();

        public List<Sphere> Spheres { get; set; } = new List<Sphere>();
    }
}