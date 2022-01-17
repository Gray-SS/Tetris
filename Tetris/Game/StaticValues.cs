using ConsoleEngine.Inputs;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tetris.Game.Configurations;

namespace Tetris.Game
{
    public static class StaticValues
    {
        public static int Score { get; set; }
        public static UdpClient UdpClient { get; private set; }
        public static UserConfiguration Configuration { get; private set; }
        public static List<Player> Leaderboard { get; private set; }

        public static readonly SoundPlayer MainBradinski = new(@"Audio\main_bradinski.wav");
        public static readonly SoundPlayer MainKarinka = new(@"Audio\main_karinka.wav");
        public static readonly SoundPlayer MainLoginska = new(@"Audio\main_loginska.wav");
        public static readonly SoundPlayer MainTroika = new(@"Audio\main_troika.wav");
        public static readonly SoundPlayer MainGameOver = new(@"Audio\main_game_over.wav");
        public static readonly SoundPlayer TitleTheme = new(@"Audio\title_theme.wav");


        private static readonly string _ip = "192.168.1.16";//"10.153.116.73";

        public static void Initialize()
        {
            UdpClient = new UdpClient();
            Configuration = BaseConfiguration.Load<UserConfiguration>();
            Leaderboard = new List<Player>();
        }

        public static void UpdateData()
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes($"lb-append {Configuration.Id} {Configuration.Username} {Configuration.Highscore}");
                UdpClient.Send(buffer, buffer.Length, _ip, 9981);

                Thread.Sleep(50);

                var task = GetLeaderboards();
                task.Wait();

                Leaderboard = task.Result;
            }
            catch { }
        }

        private static async Task<List<Player>> GetLeaderboards()
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes("lb-get");
                UdpClient.Send(buffer, buffer.Length, _ip, 9981);

                var result = await UdpClient.ReceiveAsync();
                string jsonResp = Encoding.UTF8.GetString(result.Buffer);

                return JsonConvert.DeserializeObject<List<Player>>(jsonResp);
            }
            catch { return new List<Player>(); }
        }
    }
}