// https://www.raylib.com/
// https://github.com/ChrisDill/Raylib-cs
// dotnet add package Raylib-cs

using Kz.IdlePlanetMiner;
using Raylib_cs;
using System.Numerics;
using Color = Raylib_cs.Color;

internal class Program
{
    private static Camera2D _camera;
    public static void Main()
    {
        //
        // Initialization
        //
        var settings = new WindowSettings(1536, 1536, 1);

        Raylib.InitWindow(settings.WindowWidth, settings.WindowHeight, ".: Idle Planet Miner :.");
        Raylib.SetTargetFPS(60);

        _camera.Target = new Vector2(settings.HalfScreenWidth, settings.HalfScreenHeight);
        _camera.Offset = new Vector2(settings.HalfScreenWidth, settings.HalfScreenHeight);
        _camera.Rotation = 0.0f;
        _camera.Zoom = 2.5f;

        //
        // Setup Game
        //
        var game = new Game(settings);

        //
        // MAIN RENDER LOOP
        //
        while (!Raylib.WindowShouldClose())    // Detect window close button or ESC key
        {            
            ProcessInputs(settings, game);

            Update(settings, game);

            Render(settings, game);
        }

        game.End();
        Raylib.CloseWindow();
    }

    private static void Render(WindowSettings settings, IGame game)
    {        
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);
        
        Raylib.BeginMode2D(_camera);
        game.Render();        
        Raylib.EndMode2D();
        
        Raylib.DrawFPS(10, 10);
        Raylib.DrawText($"Zoom: {_camera.Zoom:0.00}", 10, 35, 20, Color.RayWhite);
        
        Raylib.EndDrawing();
    }

    private static void Update(WindowSettings settings, IGame game)
    {
        game.Update();
    }

    private static void ProcessInputs(WindowSettings settings, IGame game)
    {
        // Camera zoom controls
        _camera.Zoom += Raylib.GetMouseWheelMove() * 0.05f;
        if (_camera.Zoom > 5.0f) _camera.Zoom = 5.0f;
        else if (_camera.Zoom < 0.5f) _camera.Zoom = 0.5f;


        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            // do something...
        }

        game.ProcessInputs();
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
}