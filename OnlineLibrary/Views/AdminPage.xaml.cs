using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OnlineLibrary.Helpers;
using OnlineLibrary.Dialogs;

namespace OnlineLibrary.Views
{
    public partial class AdminPage : Page
    {
        private OnlineLibraryEntities db = new OnlineLibraryEntities();

        public AdminPage()
        {
            InitializeComponent();
            LoadComplaints();
            LoadAuthorRequests();
            LoadUnfreezeRequests();
            LoadFrozenObjects();
            LoadUsers();
        }
        private void LoadComplaints()
        {
            var complaints = db.Complaints.Where(c => c.Status == "pending").ToList();

            var result = complaints.Select(c => new
            {
                c.ComplaintId,
                TypeText = c.Type == "book" ? "Жалоба на книгу" : c.Type == "author" ? "Жалоба на автора" : "Жалоба на отзыв",
                TargetInfo = c.Type == "book" ? db.Books.FirstOrDefault(b => b.BookId == c.TargetId)?.Title ?? "Книга не найдена"
                            : c.Type == "author" ? db.Users.FirstOrDefault(u => u.UserId == c.TargetId)?.DisplayName ?? "Автор не найден"
                            : $"Отзыв #{c.TargetId}",
                c.Reason,
                ComplainantName = c.Users?.DisplayName ?? "Неизвестный"
            }).ToList();

            complaintsList.ItemsSource = result;
        }

        private void LoadAuthorRequests()
        {
            var requests = db.AuthorRequests.Where(r => r.Status == "pending")
                .Select(r => new
                {
                    r.RequestId,
                    UserName = r.Users.DisplayName,
                    r.Reason
                }).ToList();

            authorRequests.ItemsSource = requests;
        }

        private void LoadUnfreezeRequests()
        {
            var requests = db.UnfreezeRequests.Where(r => r.Status == "pending")
                .Select(r => new
                {
                    r.RequestId,
                    UserName = r.Users.DisplayName,
                    r.AppealText
                }).ToList();

            unfreezeRequests.ItemsSource = requests;
        }

        private void LoadFrozenObjects()
        {
            frozenBooks.ItemsSource = db.Books.Where(b => b.Status == "frozen").ToList();
            frozenUsers.ItemsSource = db.Users.Where(u => u.Status == "frozen").ToList();
            frozenReviews.ItemsSource = db.Reviews.Where(r => r.Status == "frozen")
                .Select(r => new
                {
                    r.ReviewId,
                    Content = r.Content.Length > 100 ? r.Content.Substring(0, 100) + "..." : r.Content
                }).ToList();
        }

        private void LoadUsers()
        {
            var users = db.Users.Select(u => new
            {
                u.UserId,
                u.Login,
                u.DisplayName,
                u.Email,
                u.Role,
                FreezeBtnVis = u.Status == "active" && u.UserId != SessionHelper.CurrentUser.UserId ? Visibility.Visible : Visibility.Collapsed
            }).ToList();

            usersList.ItemsSource = users;
        }

        private void AcceptComplaint_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var c = db.Complaints.Find((int)btn.Tag);
            if (c == null) return;

            c.Status = "accepted";
            c.ProcessedAt = DateTime.Now;
            c.AdminId = SessionHelper.CurrentUser.UserId;

            if (c.Type == "book")
            {
                var b = db.Books.Find(c.TargetId);
                if (b != null) b.Status = "frozen";
            }
            if (c.Type == "review")
            {
                var r = db.Reviews.Find(c.TargetId);
                if (r != null) r.Status = "frozen";
            }
            db.SaveChanges();
            LoadComplaints();
            LoadFrozenObjects();
            MessageBox.Show("Жалоба принята, объект заморожен.");
        }

        private void RejectComplaint_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var c = db.Complaints.Find((int)btn.Tag);
            if (c == null) return;

            c.Status = "rejected";
            c.ProcessedAt = DateTime.Now;
            c.AdminId = SessionHelper.CurrentUser.UserId;
            db.SaveChanges();
            LoadComplaints();
            MessageBox.Show("Жалоба отклонена.");
        }

        private void AcceptAuthor_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var r = db.AuthorRequests.Find((int)btn.Tag);
            if (r == null) return;

            r.Status = "accepted";
            r.ProcessedAt = DateTime.Now;
            r.AdminId = SessionHelper.CurrentUser.UserId;

            var user = db.Users.Find(r.UserId);
            if (user != null) user.Role = "author";

            db.SaveChanges();
            LoadAuthorRequests();
            LoadUsers();
            MessageBox.Show("Пользователю назначена роль автора.");
        }

        private void RejectAuthor_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var r = db.AuthorRequests.Find((int)btn.Tag);
            if (r == null) return;

            r.Status = "rejected";
            r.ProcessedAt = DateTime.Now;
            r.AdminId = SessionHelper.CurrentUser.UserId;
            db.SaveChanges();
            LoadAuthorRequests();
            MessageBox.Show("Заявка отклонена.");
        }

        private void AcceptUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var r = db.UnfreezeRequests.Find((int)btn.Tag);
            if (r == null) return;

            r.Status = "accepted";
            r.ProcessedAt = DateTime.Now;
            r.AdminId = SessionHelper.CurrentUser.UserId;

            var u = db.Users.Find(r.UserId);
            if (u != null)
            {
                u.Status = "active";
                u.FreezeReason = null;
            }

            db.SaveChanges();
            LoadUnfreezeRequests();
            LoadFrozenObjects();
            LoadUsers();
            MessageBox.Show("Пользователь разморожен.");
        }

        private void RejectUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var r = db.UnfreezeRequests.Find((int)btn.Tag);
            if (r == null) return;

            r.Status = "rejected";
            r.ProcessedAt = DateTime.Now;
            r.AdminId = SessionHelper.CurrentUser.UserId;
            db.SaveChanges();
            LoadUnfreezeRequests();
            MessageBox.Show("Заявка отклонена.");
        }

        private void UnfreezeBook_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var b = db.Books.Find((int)btn.Tag);
            if (b != null)
            {
                b.Status = "active";
                db.SaveChanges();
                LoadFrozenObjects();
                MessageBox.Show("Книга разморожена.");
            }
        }

        private void UnfreezeUser_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var u = db.Users.Find((int)btn.Tag);
            if (u != null)
            {
                u.Status = "active";
                u.FreezeReason = null;
                db.SaveChanges();
                LoadFrozenObjects();
                LoadUsers();
                MessageBox.Show("Пользователь разморожен.");
            }
        }

        private void UnfreezeReview_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var r = db.Reviews.Find((int)btn.Tag);
            if (r != null)
            {
                r.Status = "active";
                db.SaveChanges();
                LoadFrozenObjects();
                MessageBox.Show("Отзыв разморожен.");
            }
        }

        private void SaveRole_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var parent = btn.Parent as StackPanel;
            if (parent == null) return;

            var cmb = parent.Children[0] as ComboBox;
            if (cmb == null) return;

            var selectedItem = cmb.SelectedItem as ComboBoxItem;
            if (selectedItem == null) return;

            var u = db.Users.Find((int)btn.Tag);
            if (u != null)
            {
                u.Role = selectedItem.Tag.ToString();
                db.SaveChanges();
                LoadUsers();
                MessageBox.Show("Роль сохранена.");
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            int userId = (int)btn.Tag;
            var dialog = new ChangePasswordDialog(userId);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        private void FreezeUser_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            int userId = (int)btn.Tag;
            var dialog = new FreezeUserDialog(userId);
            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
            LoadFrozenObjects();
            LoadUsers();
        }
    }
}