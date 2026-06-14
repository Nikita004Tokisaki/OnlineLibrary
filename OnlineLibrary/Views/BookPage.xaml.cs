using OnlineLibrary.Dialogs;
using OnlineLibrary.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace OnlineLibrary.Views
{
    public partial class BookPage : Page
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities();
        int _bookId; 
        Books _book;

        public BookPage(int bookId)
        {
            InitializeComponent();
            _bookId = bookId; 
            LoadBook(); 
            LoadReviews();
            if (SessionHelper.IsAdmin) 
            { 
                btnFreeze.Visibility = Visibility.Visible; 
                btnFreeze.Visibility = Visibility.Visible; 
            }
        }

        private void BtnBack_Click(object s, RoutedEventArgs e) => NavigationService.GoBack();

        private void LoadBook()
        {
            _book = db.Books.Find(_bookId);
            txtTitle.Text = _book.Title;
            txtAuthor.Text = _book.Users.DisplayName;
            txtRating.Text = $"★ {_book.AvgRating:F1}";
            txtDescription.Text = _book.Description ?? "";
            txtGenres.Text = _book.Genres.Any() ? string.Join(", ", _book.Genres.Select(g => g.GenreName)) : "Жанры не указаны";
        }

        private void LoadReviews() => reviewsList.ItemsSource = db.Reviews.Where(r => r.BookId == _bookId && r.Status == "active").Select(r => new { r.ReviewId, UserName = r.Users.DisplayName, r.Rating, r.Content, r.CreatedAt }).OrderByDescending(r => r.CreatedAt).ToList();

        private void BtnRead_Click(object s, RoutedEventArgs e) => NavigationService.Navigate(new ReadingPage(_bookId));
        private void BtnAddToList_Click(object s, RoutedEventArgs e) => new ListSelectionDialog(_bookId).ShowDialog();
        private void BtnComplainBook_Click(object s, RoutedEventArgs e) => new ComplaintDialog("book", _bookId, _book.Title).ShowDialog();
        private void BtnComplainAuthor_Click(object s, RoutedEventArgs e) => new ComplaintDialog("author", _book.AuthorId, _book.Users.DisplayName).ShowDialog();
        private void BtnFreeze_Click(object s, RoutedEventArgs e) 
        {
            _book.Status = "frozen"; 
            db.SaveChanges(); 
            btnFreeze.Visibility = Visibility.Collapsed; 
            MessageBox.Show("Книга заморожена!"); 
        }

        private void BtnSubmitReview_Click(object s, RoutedEventArgs e)
        {
            if (SessionHelper.IsFrozen) 
            { 
                MessageBox.Show("Аккаунт заморожен!"); 
                return; 
            }
            int rating = int.Parse((cmbRating.SelectedItem as ComboBoxItem).Content.ToString());
            db.Reviews.Add(new Reviews { BookId = _bookId, UserId = SessionHelper.CurrentUser.UserId, Rating = rating, Content = txtReview.Text, Status = "active", CreatedAt = DateTime.Now });
            db.SaveChanges();
            _book.AvgRating = db.Reviews.Where(r => r.BookId == _bookId).Average(r => (double?)r.Rating) ?? 0;
            db.SaveChanges();
            LoadReviews(); 
            LoadBook(); 
            txtReview.Text = "";
            MessageBox.Show("Отзыв добавлен!");
        }

        private void BtnComplainReview_Click(object s, RoutedEventArgs e) => new ComplaintDialog("review", (int)(s as Button).Tag, "отзыв").ShowDialog();
        private void BtnFreezeReview_Click(object s, RoutedEventArgs e) 
        { 
            var r = db.Reviews.Find((int)(s as Button).Tag); 
            r.Status = "frozen"; 
            db.SaveChanges(); 
            LoadReviews(); 
        }
    }
}
