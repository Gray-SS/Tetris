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
        //LEADERBOARD
        #region State 3
        private UdpClient _udpClient = new();
        private List<KeyValuePair<string, int>> _leaderboard;
        #endregion

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
        
        private async Task<List<KeyValuePair<string, int>>> GetLeaderboards()
        {
            string ip = "192.168.1.16";//"10.153.116.73";
            _udpClient.Connect(ip, 9981);

            byte[] buffer = Encoding.UTF8.GetBytes("lb-get");
            _udpClient.Send(buffer, buffer.Length);

            var result = await _udpClient.ReceiveAsync();
            string jsonResp = Encoding.UTF8.GetString(result.Buffer);

            _udpClient.Close();

            return JsonConvert.DeserializeObject<List<KeyValuePair<string, int>>>(jsonResp);
        }
    }
}