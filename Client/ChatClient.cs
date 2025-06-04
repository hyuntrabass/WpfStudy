using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class ChatClient
    {
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;

        public static ChatClient Instance { get; } = new ChatClient();

        public event Action<string> MessageReceived;

        private ChatClient() { }

        public bool Connect(string host = "127.0.0.1", int port = 5000, string nickName = "익명")
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(host, port);
                _reader = new StreamReader(_client.GetStream(), Encoding.UTF8);
                _writer = new StreamWriter(_client.GetStream(), Encoding.UTF8) { AutoFlush = true };

                _writer.WriteLine($"__JOIN__:{nickName}");

                Thread listener = new Thread(Listen);
                listener.IsBackground = true;
                listener.Start();

                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("서버 연결 실패: " + e.Message);
                return false;
            }
        }

        private void Listen()
        {
            try
            {
                while (_client.Connected)
                {
                    string? message = _reader.ReadLine();
                    if (!string.IsNullOrEmpty(message))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageReceived?.Invoke(message);
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("서버 연결 종료됨: " + e.Message);
                });
            }
        }

        public void Send(string message)
        {
            try
            {
                _writer.WriteLine(message);
            }
            catch (Exception e)
            {
                MessageBox.Show("메세지 전송 실패" + e.Message);
            }
        }
    }
}
