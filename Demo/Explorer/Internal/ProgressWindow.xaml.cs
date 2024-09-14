using System.Windows;

namespace ExplorerCtrl.Internal;

/// <summary>
/// Interaction logic for ProgresshWindow.xaml
/// </summary>
public partial class ProgresshWindow : Window
    {
        public ProgresshWindow()
        {
            InitializeComponent();
        }

        public void Update(double percentage, string file = null)
        {
            progressBar.Value = percentage;
            if (file != null)
            {
                currentFile.Text = file;
            }
        }

        public bool IsCancelPendíng { get; private set; }
        
        private void OnCancel(object sender, RoutedEventArgs e)
        {
            IsCancelPendíng = true;
        }
    }
