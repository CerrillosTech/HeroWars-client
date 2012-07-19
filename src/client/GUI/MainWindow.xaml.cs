using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HeroWars.GUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void SetSessionId(string sessionId)
        {
            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => SetSessionId(sessionId)));
                return;
            }
            txtSessionId.Text = "Your session ID: " + sessionId;
        }
    }
}
