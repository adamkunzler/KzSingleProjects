// https://www.raylib.com/
// https://github.com/ChrisDill/Raylib-cs
// dotnet add package Raylib-cs

using Kz.Engine.DataStructures;
using Raylib_cs;
using Scratch.RayTracer;
using Scratch.RayTracer.Shapes;
using Color = Raylib_cs.Color;

internal class Program
{
    public static void Main()
    {
        var screenWidth = 1024;
        var screenHeight = 1024;
        var scale = 4;

        //
        // Initialization
        //
        Raylib.InitWindow(screenWidth, screenHeight, ".: Computer Graphics from Scratch - RayTracer :.");
        Raylib.SetTargetFPS(60);

        var scene = TestScene();
        var raytracer = new RayTracer(screenWidth, screenHeight, scale, scene);

        //
        // MAIN RENDER LOOP
        //
        while (!Raylib.WindowShouldClose())    // Detect window close button or ESC key
        {
            //
            // PROCESS INPUTS
            //
            //TODO

            //
            // UPDATE STUFF
            //
            raytracer.Update();

            //
            // RENDER STUFF
            //
            raytracer.Render();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            RenderTextureToWindow(raytracer.CanvasTexture, screenWidth, screenHeight);
            Raylib.DrawFPS(10, 10);
            Raylib.EndDrawing();
        }

        raytracer.CleanUp();
        Raylib.CloseWindow();
    }

    private static void RenderTextureToWindow(RenderTexture2D target, int windowWidth, int windowHeight)
    {
        var src = new Rectangle(0, 0, target.Texture.Width, -target.Texture.Height);
        var dest = new Rectangle(0, 0, windowWidth, windowHeight);
        Raylib.DrawTexturePro(
            target.Texture,
            src,
            dest,
            new System.Numerics.Vector2(0.0f, 0.0f),
            0,
            Color.White);
    }

    private static Scene TestScene()
    {
        var scene = new Scene();

        // lights
        scene.Lights.Add(new Scratch.RayTracer.Lights.AmbientLight(0.2f));
        scene.Lights.Add(new Scratch.RayTracer.Lights.PointLight(0.6f, new Vector3f(2.0f, 1.0f, 0.0f)));
        scene.Lights.Add(new Scratch.RayTracer.Lights.DirectionalLight(0.2f, new Vector3f(1.0f, 4.0f, 4.0f)));

        // "sun"
        scene.Spheres.Add(new Sphere(new Vector3f(0, -5001.0f, 0.0f), 5000, new RGBColor(245, 245, 245)));

        // objects
        scene.Spheres.Add(new Sphere(new Vector3f(0, -1, 5), 1, new RGBColor(255, 0, 0)));
        scene.Spheres.Add(new Sphere(new Vector3f(2, 0, 6), 1, new RGBColor(0, 0, 255)));
        scene.Spheres.Add(new Sphere(new Vector3f(-2, 0, 6), 1, new RGBColor(0, 255, 0)));

        return scene;
    }
}