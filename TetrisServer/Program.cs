using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TetrisServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string filename = "lb.csv";

            if (!File.Exists(filename)) File.Create(filename).Close();

            UdpClient client = new(new IPEndPoint(IPAddress.Any, 9981));

            while (true)
            {
                var result = await client.ReceiveAsync();
                string[] line = Encoding.UTF8.GetString(result.Buffer).Split();
                string[] arguments = line.Skip(1).ToArray();

                switch (line[0])
                {
                    case "lb-get":
                        var list = new List<Player>();
                        string[] data = File.ReadAllLines(filename);

                        foreach(var d in data)
                        {
                            var dataLine = d.Split(';');
                            int id = Convert.ToInt32(dataLine[0]);
                            string username = dataLine[1];
                            int highscore = Convert.ToInt32(dataLine[2]);

                            list.Add(new Player() { Id = id, Username = username, Highscore = highscore });
                        }

                        string json = JsonConvert.SerializeObject(list);
                        byte[] buffer = Encoding.UTF8.GetBytes(json);
                        await client.SendAsync(buffer, buffer.Length, result.RemoteEndPoint);

                        break;

                    case "lb-append":
                        List<string> allTextLines = File.ReadAllLines(filename).ToList();

                        int c = 0;
                        for (int i = 0; i< allTextLines.Count; i++)
                        {
                            string[] splitedLines = allTextLines[i].Split(';');
                            if (arguments[0] == splitedLines[0])
                            {
                                c++;
                                allTextLines[i] = $"{arguments[0]};{arguments[1]};{arguments[2]}";
                            }
                        }
                        if (c == 0)
                        {
                            string append_line = $"{arguments[0]};{arguments[1]};{arguments[2]}";
                            allTextLines.Add(append_line);
                        }

                        File.Delete(filename);

                        Console.WriteLine();
                        var writer = new StreamWriter(File.Create(filename));
                        for (int i = 0; i < allTextLines.Count; i++)
                        {
                            Console.WriteLine(allTextLines[i]);
                            writer.WriteLine(allTextLines[i]);
                        }

                        writer.Flush();
                        writer.Close();
                        writer.Dispose();

                        break;
                }
            }
        }
    }
}
