using System.Collections.Specialized;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();

            if (DataContext is MainViewModel vm)
            {
                var messages = vm.Messages;

                // 메시지 컬렉션이 변경될 때마다 스크롤
                messages.CollectionChanged += Messages_CollectionChanged;
            }
        }

        private void MessageBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is MainViewModel vm && !string.IsNullOrWhiteSpace(vm.InputText))
                {
                    vm.SendCommand.Execute(null);
                    e.Handled = true; // 줄바꿈 방지
                }
            }
        }

        private void Messages_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // 렌더링 이후 스크롤을 확실히 하기 위해 Background 우선순위로 처리
                Dispatcher.InvokeAsync(() =>
                {
                    if (ChatList.Items.Count > 0)
                        ChatList.ScrollIntoView(ChatList.Items[ChatList.Items.Count - 1]);
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }
}