using System;
using System.Windows;
using HeroWars.Properties;
using HeroWars.Protocol;

namespace HeroWars.GUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class LoginWindow
    {
        public MainWindow MainWindow { get; private set; }
        private TCPClient _tcpClient;

        public LoginWindow()
        {
            InitializeComponent();
            Loaded += LoginWindow_Loaded;
        }

        void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _tcpClient = new TCPClient(this);
            if(!_tcpClient.Connect(Settings.Default.server,Settings.Default.port))
            {
                MessageBox.Show("Unable to connect to HeroWars lobby server.", "connection error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                Application.Current.Shutdown(-1);
            }
            _tcpClient.Run();
        }

        public void Login(bool result)
        {
            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(() => Login(result)));
                return;
            }
            if(!result)
            {
                MessageBox.Show("Wrong username or password. Please retry.","Unable to login",MessageBoxButton.OK,MessageBoxImage.Error);
                txtPassword.IsEnabled = true;
                txtUsername.IsEnabled = true;
                btnLogin.IsEnabled = true;
                btnReset.IsEnabled = true;
                return;
            }
            MainWindow = new MainWindow(); 
            MainWindow.Show();
            Close();
        }

        public void HandleHandshakeError()
        {
            if(!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(new Action(HandleHandshakeError));
                return;
            }
            MessageBox.Show("Unable to connect to HeroWars lobby server. Probably you have a wrong client version.", "connection error", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown(-2);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txtUsername.Text = string.Empty;
            txtPassword.Password = string.Empty;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(txtPassword.Password) || string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("You haven't filled out every field");
                return;
            }
            txtUsername.IsEnabled = false;
            txtPassword.IsEnabled = false;
            btnLogin.IsEnabled = false;
            btnReset.IsEnabled = false;
            _tcpClient.SendAuth(txtUsername.Text,txtPassword.Password);
        }
    }
}
