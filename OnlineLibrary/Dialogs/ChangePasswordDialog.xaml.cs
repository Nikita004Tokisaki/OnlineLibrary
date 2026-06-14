using System.Windows;


namespace OnlineLibrary.Dialogs
{
    public partial class ChangePasswordDialog : Window
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities();
        int _userId;

        public ChangePasswordDialog(int userId)
        {
            InitializeComponent();
            _userId = userId;
        }

        private void BtnSave_Click(object s, RoutedEventArgs e)
        {
            if (txtPassword.Password != txtConfirm.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            if (string.IsNullOrEmpty(txtPassword.Password))
            {
                MessageBox.Show("Введите пароль!");
                return;
            }

            if (txtPassword.Password.Length < 4)
            {
                MessageBox.Show("Пароль должен быть не менее 4 символов!");
                return;
            }

            var u = db.Users.Find(_userId);
            if (u != null)
            {
                u.PasswordHash = txtPassword.Password;
                db.SaveChanges();
                MessageBox.Show("Пароль изменён!");
                Close();
            }
            else
            {
                MessageBox.Show("Пользователь не найден!");
            }
        }

        private void BtnCancel_Click(object s, RoutedEventArgs e) => Close();
    }
}
