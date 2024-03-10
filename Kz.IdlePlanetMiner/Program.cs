// https://www.raylib.com/
// https://github.com/ChrisDill/Raylib-cs
// dotnet add package Raylib-cs

using Kz.IdlePlanetMiner;
using Raylib_cs;
using Color = Raylib_cs.Color;

internal class Program
{
    public static void Main()
    {
        //
        // Initialization
        //
        var settings = new WindowSettings(1536, 1536, 2);

        Raylib.InitWindow(settings.WindowWidth, settings.WindowHeight, ".: Idle Planet Miner :.");
        Raylib.SetTargetFPS(60);

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
        game.Render();

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        RenderTextureToWindow(game.Texture, settings.WindowWidth, settings.WindowHeight);

        Raylib.EndDrawing();
    }

    private static void Update(WindowSettings settings, IGame game)
    {
        game.Update();
    }

    private static void ProcessInputs(WindowSettings settings, IGame game)
    {
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