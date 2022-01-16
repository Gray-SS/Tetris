using ConsoleEngine;
using ConsoleEngine.Graphics;
using ConsoleEngine.Inputs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tetris.Game.GameStates;

namespace Tetris.Game
{
    public class TetrisEngine : Engine
    {
        public TetrisEngine() : base() { }

        public override void Load()
        {
            StaticValues.Initialize();

            if (StaticValues.Configuration.FirstExecute)
            {
                Random r = new();
                StaticValues.Configuration.FirstExecute = false;

                string username = "user";
                for (int i = 0; i < 5; i++) username += r.Next(10);
                StaticValues.Configuration.Username = username;
            }

            GameStateManager.AppendState(new TetrisGameState(this));
            GameStateManager.AppendState(new OptionsMenuGameState(this));
            GameStateManager.AppendState(new GameOverGameState(this));
            GameStateManager.AppendState(new LeaderboardGameState(this));
            GameStateManager.AppendState(new MainMenuGameState(this));

            GameStateManager.Load();
            GameStateManager.SetCurrentState<MainMenuGameState>();
        }

        public override void Update(float dt)
        {
            GameStateManager.Update(dt);
        }

        public override void Draw()
        {
            Graphics.Clear(CColor.Black);
            Graphics.DrawText(ConsoleWidth - 10, 1, $"FPS: {FPS}", CColor.Red, null);

            GameStateManager.Draw();
        }
    }
}