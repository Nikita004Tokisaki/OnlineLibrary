using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OnlineLibrary.Helpers;
using OnlineLibrary.Dialogs;

namespace OnlineLibrary.Views
{
    public partial class ProfilePage : Page
    {
        private OnlineLibraryEntities db = new OnlineLibraryEntities();

        public ProfilePage()
        {
            InitializeComponent();

            var u = SessionHelper.CurrentUser;
            if (u == null)
            {
                MessageBox.Show("Пользователь не авторизован!");
                return;
            }

            if (txtName != null) txtName.Text = u.DisplayName;
            if (txtLogin != null) txtLogin.Text = u.Login;
            if (txtEmail != null) txtEmail.Text = u.Email;
            if (txtRole != null)
            {
                txtRole.Text = u.Role == "admin" ? "Администратор" : u.Role == "author" ? "Автор" : "Пользователь";
            }

            if (warningBorder != null && SessionHelper.IsFrozen)
            {
                warningBorder.Visibility = Visibility.Visible;
                if (txtReason != null) txtReason.Text = $"Причина: {u.FreezeReason ?? "Не указана"}";
            }

            if (authorBorder != null && SessionHelper.IsUser && !SessionHelper.IsFrozen)
            {
                authorBorder.Visibility = Visibility.Visible;
            }

            if (reviewsList != null)
            {
                var reviews = db.Reviews
                    .Where(r => r.UserId == u.UserId)
                    .Select(r => new
                    {
                        BookTitle = r.Books.Title,
                        r.Rating,
                        r.Content,
                        r.CreatedAt
                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToList();

                reviewsList.ItemsSource = reviews;
            }
        }

        private void BtnAuthorRequest_Click(object sender, RoutedEventArgs e)
        {
            if (SessionHelper.CurrentUser == null) return;

            var existingRequest = db.AuthorRequests
                .FirstOrDefault(r => r.UserId == SessionHelper.CurrentUser.UserId && r.Status == "pending");

            if (existingRequest != null)
            {
                MessageBox.Show("Вы уже подали заявку! Ожидайте решения администратора.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            db.AuthorRequests.Add(new AuthorRequests
            {
                UserId = SessionHelper.CurrentUser.UserId,
                Reason = "Хочу стать автором",
                Status = "pending",
                CreatedAt = DateTime.Now
            });

            db.SaveChanges();
            MessageBox.Show("Заявка на роль автора отправлена!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnAppeal_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AppealDialog("user", null);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }
    }
}