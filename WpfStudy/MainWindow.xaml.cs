using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfStudy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Button? _btn_del;
        readonly Brush _bursh;

        public MainViewModel _viewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            //_bursh = resultText.Foreground;

            _viewModel = new MainViewModel();
            _viewModel.Name = string.Empty;
            this.DataContext = _viewModel;
        }

        private void DeleteButton(object sender, RoutedEventArgs e)
        {
            //resultText.Text = null;
            //nameBox.Text = null;

            //buttonPanel.Children.Remove(_btn_del);
            _btn_del = null;
            _viewModel.Name = string.Empty;
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            _viewModel.Name = string.Empty;
        }

        private Point _dragStartPoint;

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
        }

        private void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(null);
                Vector diff = _dragStartPoint - position;

                if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    if (ItemListBox.SelectedItem == null)
                        return;

                    DragDrop.DoDragDrop(ItemListBox, ItemListBox.SelectedItem, DragDropEffects.Move);
                }
            }
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Item)))
            {
                var draggedItem = e.Data.GetData(typeof(Item)) as Item;
                var target = ((FrameworkElement)e.OriginalSource).DataContext as Item;

                if (draggedItem == null || target == null || draggedItem == target)
                    return;

                var items = ((MainViewModel)this.DataContext).Items;
                int oldIndex = items.IndexOf(draggedItem);
                int newIndex = items.IndexOf(target);

                if (oldIndex >= 0 && newIndex >= 0)
                {
                    items.Move(oldIndex, newIndex);
                }
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            //if (nameBox.Text.Length == 0)
            //{
            //    resultText.Text = "이름이 없습니다.";
            //    resultText.Foreground = Brushes.Red;
            //    return;
            //}
            //_viewModel.Name = nameBox.Text;
            //resultText.Foreground = _bursh;
            //resultText.Text = $"{nameBox.Text}님 환영합니다!";

            //if (nameBox.Text == "정유진")
            //{
            //    resultText.Text = "공주님 환영합니다!!!!!!";
            //    resultText.Foreground = Brushes.Pink;
            //}

            //if (_btn_del == null)
            //{
            //    _btn_del = new Button();
            //    _btn_del.Content = "삭제";
            //    _btn_del.Margin = new Thickness(10, 0, 0, 0);
            //    _btn_del.Width = 100;
            //    buttonPanel.Children.Add(_btn_del);
            //    _btn_del.Click += DeleteButton;
            //}
        }
    }
}