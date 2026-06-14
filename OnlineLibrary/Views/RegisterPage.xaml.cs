using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OnlineLibrary.Views
{
    public partial class RegisterPage : Page
    {
        private OnlineLibraryEntities db = new OnlineLibraryEntities();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;
            string confirm = txtConfirm.Password;
            string email = txtEmail.Text.Trim();
            string displayName = txtDisplayName.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(displayName))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            if (db.Users.Any(u => u.Login == login))
            {
                MessageBox.Show("Логин занят!");
                return;
            }

            if (db.Users.Any(u => u.Email == email))
            {
                MessageBox.Show("Email уже используется!");
                return;
            }

            var newUser = new Users
            {
                Login = login,
                PasswordHash = password, 
                Email = email,
                DisplayName = displayName,
                Role = "user",
                Status = "active",
                FreezeReason = null,
                CreatedAt = DateTime.Now
            };

            db.Users.Add(newUser);
            db.SaveChanges();

            MessageBox.Show("Регистрация успешна!");
            Window.GetWindow(this)?.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }
    }
}