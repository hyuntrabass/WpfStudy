using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<string> Messages { get; set; } = new();

        private string _serverIp = "127.0.0.1";
        public string ServerIp
        {
            get => _serverIp;
            set
            {
                _serverIp = value;
                OnPropertyChanged(nameof(ServerIp));
            }
        }

        private string _nickName;
        public string NickName
        {
            get => _nickName;
            set
            {
                _nickName = value;
                OnPropertyChanged(nameof(NickName));
            }
        }

        private string _inputText;
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        public bool isConnected = false;

        public ICommand ConnectCommand { get; }
        public ICommand SendCommand { get; }
        public ICommand SetNameCommand { get; }

        public MainViewModel()
        {
            ConnectCommand = new RelayCommand(ConnectToServer, () => !string.IsNullOrWhiteSpace(ServerIp));
            SendCommand = new RelayCommand(SendMessage, () => !string.IsNullOrWhiteSpace(InputText) && isConnected);
            SetNameCommand = new RelayCommand(SetNickName, () => !string.IsNullOrWhiteSpace(NickName) && isConnected);

            // 서버 연결
            //ChatClient.Instance.Connect();

            // 수신 이벤트 연결
            ChatClient.Instance.MessageReceived += (msg) =>
            {
                Messages.Add(msg);
            };
        }

        private void ConnectToServer()
        {
            if (ChatClient.Instance.Connect(ServerIp, 5000, NickName))
            {
                Messages.Add("서버에 연결되었습니다.");
                isConnected = true;
            }
            else
            {
                isConnected = false;
            }
        }

        private void SetNickName()
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            ChatClient.Instance.Send($"__SetName__:{NickName}");
        }

        private void SendMessage()
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            string finalMessage = $"[{time}] {NickName}: {InputText}";
            Messages.Add($"[{time}] 나: {InputText}");
            ChatClient.Instance.Send(finalMessage);
            InputText = string.Empty;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
