using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfStudy
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly string _filePath = "items.json";

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        private string _greeting = string.Empty;
        public string Greeting
        {
            get => _greeting;
            set
            {
                _greeting = value;
                OnPropertyChanged(nameof(Greeting));
            }
        }

        public ObservableCollection<Item> Items { get; set; }

        public ICommand GreetCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        private string _newItemName = string.Empty;
        public string NewItemName
        {
            get => _newItemName;
            set
            {
                _newItemName = value;
                OnPropertyChanged(nameof(NewItemName));
            }
        }
        private string _newItemDesc = string.Empty;
        public string NewItemDesc
        {
            get => _newItemDesc;
            set
            {
                _newItemDesc = value;
                OnPropertyChanged(nameof(NewItemDesc));
            }
        }

        private Item _selectedItem;
        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value != null && _selectedItem != value)
                {
                    _selectedItem = value;
                    SelectedColor = ((SolidColorBrush)_selectedItem.Color).Color;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }
        public Color NewColor { get; set; }
        private Color _selectedColor;
        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));
                OnPropertyChanged(nameof(SelectedBrush));
                if (_selectedItem != null)
                {
                    _selectedItem.Color = SelectedBrush;
                }
            }
        }
        public Brush SelectedBrush => new SolidColorBrush(SelectedColor);

        public MainViewModel()
        {
            AddItemCommand = new RelayCommand(_ => AddItem(), _ => !string.IsNullOrWhiteSpace(NewItemName));
            GreetCommand = new RelayCommand(_ => Greet(), _ => !string.IsNullOrWhiteSpace(Name));
            SaveCommand = new RelayCommand(_ => SaveItems());
            LoadCommand = new RelayCommand(_ => LoadItems());

            Items = new ObservableCollection<Item>
            {
                new() {Name = "사과", Description = "빨간 과일", Color = Brushes.Red },
                new() {Name = "바나나", Description = "노란 과일", Color = Brushes.Yellow },
                new() {Name = "포도", Description = "보라 과일", Color = Brushes.Purple }
            };

            LoadItems();
        }

        private void AddItem()
        {
            Items.Add(new Item { Name = NewItemName, Description = NewItemDesc, Color = new SolidColorBrush(NewColor) });

            NewItemName = string.Empty;
            NewItemDesc = string.Empty;
        }

        private void Greet()
        {
            Greeting = $"{Name}님, 반갑습니다!";
        }

        private void SaveItems()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Items, Formatting.Indented);
                File.WriteAllText(_filePath, json);

                MessageBox.Show("저장 완료", "notice");
            }
            catch (Exception ex)
            {
                MessageBox.Show("저장 실패: " + ex.Message);
            }
        }

        private void LoadItems()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return;
                }

                string json = File.ReadAllText(_filePath);
                var loadedItems = JsonConvert.DeserializeObject<ObservableCollection<Item>>(json);

                if (loadedItems != null)
                {
                    Items.Clear();
                    foreach (var item in loadedItems)
                    {
                        Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("불러오기 실패: " + ex.Message);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}
