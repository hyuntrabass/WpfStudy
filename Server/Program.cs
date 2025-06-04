using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class ClientInfo
    {
        static private int s_ID = 1;
        public string NickName { get; set; } = "익명" + s_ID.ToString();
        public int id = s_ID++;
        public TcpClient Tcp { get; set; }
        public StreamReader Reader { get; set; }
        public StreamWriter Writer { get; set; }
    }

    class Program
    {
        static TcpListener listener;
        static List<ClientInfo> clients = new List<ClientInfo>();

        static void Main()
        {
            listener = new TcpListener(IPAddress.Any, 5000);
            listener.Start();
            Console.WriteLine("채팅 서버 시작 (포트 5000)");

            while (true)
            {
                TcpClient tcp = listener.AcceptTcpClient();
                NetworkStream stream = tcp.GetStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                ClientInfo client = new()
                {
                    Tcp = tcp,
                    Reader = reader,
                    Writer = writer
                };

                clients.Add(client);
                Console.WriteLine($"[{client.NickName}] 접속");

                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }
        }

        static void HandleClient(ClientInfo client)
        {
            try
            {
                while (true)
                {
                    string? message = client.Reader.ReadLine();
                    if (message == null)
                    {
                        break;
                    }

                    Console.WriteLine($"수신 [{client.NickName}]: " + message);

                    if (message.StartsWith("__SetName__:"))
                    {
                        int idx = message.IndexOf(':');
                        string name = message.Substring(idx + 1);
                        Broadcast($"*system* [{client.NickName}]님이 [{name}](으)로 이름을 변경했습니다.");
                        Console.WriteLine($"이름 변경: [{client.NickName}] -> [{name}]");
                        client.NickName = name;
                    }
                    else if (message.StartsWith("__JOIN__:"))
                    {
                        int idx = message.IndexOf(':');
                        string name = message.Substring(idx + 1);
                        Console.WriteLine($"이름 설정 [{client.NickName}] -> [{name}]");
                        Broadcast($"*system* [{name}]님이 입장했습니다.", client);
                        client.NickName = name;
                    }
                    else
                    {
                        Broadcast(message, client);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("오류: " + e.Message);
            }
            finally
            {
                Broadcast($"*system* [{client.NickName}]님이 나갔습니다.");
                clients.Remove(client);
                client.Tcp.Close();
                Console.WriteLine($"[{client.NickName}] 연결 해제");
            }
        }

        static void Broadcast(string message, ClientInfo? exclude = null)
        {
            foreach (var c in clients.ToArray())
            {
                if (exclude != null && c == exclude || !c.Tcp.Connected)
                {
                    continue;
                }

                try
                {
                    c.Writer.WriteLine(message);
                }
                catch
                {
                    clients.Remove(c);
                }
            }
        }
    }
}