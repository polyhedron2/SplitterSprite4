using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForWPF
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private TextBlock textBlock;

        public MainWindow()
        {
            InitializeComponent();

            var content = new ContentControl();
            Canvas.SetLeft(content, 100);
            Canvas.SetTop(content, 100);
            textBlock = new TextBlock();

            //textBlock.Text = "hoge";
            string sample = Vanilla.Shared.Text();
            textBlock.Text = sample;

            textBlock.FontSize = 40;
            content.Content = textBlock;
            Canvas.Children.Add(content);
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                textBlock.Text = "!";
            }
            else if (e.Key == Key.None)
            {
                textBlock.Text = "NONE";
            }
            else
            {
                textBlock.Text += e.Key + "!";
            }
        }
    }
}
