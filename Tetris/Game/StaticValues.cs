using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tetris.Game.Configurations;

namespace Tetris.Game
{
    public static class StaticValues
    {
        public static int Score { get; set; }
        public static UdpClient UdpClient { get; private set; }
        public static UserConfiguration Configuration { get; private set; }

        public static void Initialize()
        {
            UdpClient = new UdpClient();
            Configuration = BaseConfiguration.Load<UserConfiguration>();
        }

        public static void UpdateData()
        {
            try
            {
                string ip = "10.153.116.73";
                //UdpClient.Connect(ip, 9981);

                byte[] buffer = Encoding.UTF8.GetBytes($"lb-append {Configuration.Id} {Configuration.Username} {Configuration.Highscore}");
                UdpClient.Send(buffer, buffer.Length, ip, 9981);

                //UdpClient.Close();
            }
            catch { }
        }

        public static async Task<List<Player>> GetLeaderboards()
        {
            try
            {
                string ip = "10.153.116.73";
                //UdpClient.Connect(ip, 9981);

                byte[] buffer = Encoding.UTF8.GetBytes("lb-get");
                UdpClient.Send(buffer, buffer.Length, ip, 9981);

                var result = await UdpClient.ReceiveAsync();
                string jsonResp = Encoding.UTF8.GetString(result.Buffer);

                //UdpClient.Close();

                return JsonConvert.DeserializeObject<List<Player>>(jsonResp);
            }
            catch { return new List<Player>(); }
        }
    }
}