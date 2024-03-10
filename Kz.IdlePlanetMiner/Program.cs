﻿// https://www.raylib.com/
// https://github.com/ChrisDill/Raylib-cs
// dotnet add package Raylib-cs

using Kz.Engine.DataStructures;
using Kz.Engine.General;
using Kz.IdlePlanetMiner;
using Raylib_cs;
using System.Numerics;
using Color = Raylib_cs.Color;

internal class Program
{
    private static Camera2D _camera;
    private static Vector2 _dragStart = new Vector2(0, 0);
    private static bool _isDragging = false;

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
        #region Camera Zoom

        var zoomMin = 0.1f;
        var zoomMax = 10.0f;
        var zoomSpeed = Utils.RangeMap(_camera.Zoom, zoomMin, zoomMax, 0.001f, 0.5f);
        
        _camera.Zoom += Raylib.GetMouseWheelMove() * zoomSpeed;
        if (_camera.Zoom > zoomMax) _camera.Zoom = zoomMax;
        else if (_camera.Zoom < zoomMin) _camera.Zoom = zoomMin;

        #endregion Camera Zoom

        #region Camera Pan

        // Update
        if (Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            _dragStart = Raylib.GetMousePosition();
            _isDragging = true;
        }

        if (Raylib.IsMouseButtonDown(MouseButton.Left) && _isDragging)
        {
            var dragEnd = Raylib.GetMousePosition();
            var dragDelta = Vector2.Subtract(_dragStart, dragEnd) * (1.0f /_camera.Zoom);
            _camera.Target = Vector2.Add(_camera.Target, dragDelta);
            _dragStart = dragEnd;
        }

        if (Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            _isDragging = false;
        }

        #endregion Camera Pan

        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            // do something...
        }

        game.ProcessInputs();
    }
}