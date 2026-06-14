using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OnlineLibrary.Helpers;

namespace OnlineLibrary.Views
{
    public partial class LoginPage : Page
    {
        private OnlineLibraryEntities db = new OnlineLibraryEntities();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!");
                return;
            }

            var user = db.Users.FirstOrDefault(u => u.Login == login && u.PasswordHash == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль!");
                return;
            }

            SessionHelper.CurrentUser = user;

            var loginWindow = Window.GetWindow(this);
            var mainWindow = new MainWindow();
            mainWindow.Show();
            loginWindow?.Close();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            var regWindow = new Window
            {
                Title = "Регистрация",
                Content = new RegisterPage(),
                Width = 450,
                Height = 580,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };
            regWindow.ShowDialog();
        }
    }
}