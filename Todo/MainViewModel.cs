using Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Todo
{
    public class MainViewModel : ViewModelBase
    {
        private readonly string _filePath = "saveDat.json";
        private readonly DispatcherTimer _alarmTimer;

        public ObservableCollection<TodoItem> Items { get; } = new();

        private string _newTodoText = string.Empty;
        public string NewTodoText
        {
            get => _newTodoText;
            set
            {
                _newTodoText = value;
                OnPropertyChanged(nameof(NewTodoText));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        public MainViewModel()
        {
            AddCommand = new RelayCommand(AddItem, () => !string.IsNullOrWhiteSpace(NewTodoText));
            RemoveCommand = new RelayCommand<TodoItem>(RemoveItem);
            SaveCommand = new RelayCommand(Save);
            LoadCommand = new RelayCommand<object>(Load);

            Load(false);

            _alarmTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _alarmTimer.Tick += CheckAlarms;
            _alarmTimer.Start();
        }

        private void AddItem()
        {
            Items.Add(new TodoItem { Title = NewTodoText });
            NewTodoText = string.Empty;
        }

        private void RemoveItem(TodoItem? item)
        {
            if (item != null)
            {
                Items.Remove(item);
            }
        }

        private void CheckAlarms(object? sender, EventArgs e)
        {
            var now = DateTime.Now;

            foreach (var item in Items)
            {
                if (item.AlarmTime.HasValue && item.AlarmTime <= now)
                {
                    MessageBox.Show($"[알림] {item.Title}", "할 일 알림", MessageBoxButton.OK, MessageBoxImage.Information);

                    item.AlarmTime = null;
                }
            }
        }

        private void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(Items, Formatting.Indented);
                File.WriteAllText(_filePath, json);

                MessageBox.Show("저장 성공");
            }
            catch (Exception e)
            {
                MessageBox.Show($"저장 실패: {e.Message}");
            }
        }

        private void Load(object? param = null)
        {
            try
            {
                bool notify = true;
                if (param is bool b)
                {
                    notify = b;
                }
                else if (param is string s && bool.TryParse(s, out notify))
                {
                }

                string json = File.ReadAllText(_filePath);
                var loadedItems = JsonConvert.DeserializeObject<ObservableCollection<TodoItem>>(json);

                if (loadedItems != null)
                {
                    Items.Clear();
                    foreach (var item in loadedItems)
                    {
                        Items.Add(item);
                    }
                }

                if (notify)
                {
                    MessageBox.Show("불러오기 성공"); 
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"불러오기 실패: {e.Message}");
            }
        }
    }
}
