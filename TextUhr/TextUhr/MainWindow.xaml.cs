using System;
using System.Threading;
using System.Windows;

namespace TextUhr
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var timer = new Timer(
                e => Update(Clock.GetTime(DateTime.Now).Result),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(1));
        }

        private void Update(string update)
        {
            Dispatcher.Invoke(() => { txt_Uhr.Text = update; });
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txt_Uhr.Text);
        }

        private void Speech_btn_Click(object sender, RoutedEventArgs e)
        {
            Clock.Say(this.txt_Uhr.Text);
        }
    }
}