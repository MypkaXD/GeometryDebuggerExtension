using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.InteropServices;
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
using Xceed.Wpf.Toolkit;
using SharpGL;
using System.Windows.Interop;

namespace LearningWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            // Создание нашего OpenGL Hwnd 'контроля'...
            ControlHost host = new ControlHost(this.ActualWidth/2, this.Height);

            Console.WriteLine(this.ActualWidth / 2 + " " + this.Height);

            // ... и присоединяем его к контейнеру:
            ControlHostElement.Child = host;
        }

    }


}