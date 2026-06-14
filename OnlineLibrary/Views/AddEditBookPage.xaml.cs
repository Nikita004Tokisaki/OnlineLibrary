using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using OnlineLibrary.Helpers;

namespace OnlineLibrary.Views
{
    public partial class AddEditBookPage : Page
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities();
        int? _bookId;

        public AddEditBookPage(int? bookId)
        {
            InitializeComponent();
            _bookId = bookId;
            txtTitle.Text = bookId.HasValue ? "Редактирование" : "Новая книга";

            if (bookId.HasValue)
            {
                var b = db.Books.Find(bookId);
                if (b != null)
                {
                    txtBookTitle.Text = b.Title;
                    txtDescription.Text = b.Description;
                    txtContentPath.Text = b.ContentPath;
                    txtGenres.Text = string.Join(", ", b.Genres.Select(g => g.GenreName));
                }
            }
        }

        private void BrowseContent_Click(object s, RoutedEventArgs e)
        {
            var d = new OpenFileDialog { Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*" };
            if (d.ShowDialog() == true)
                txtContentPath.Text = d.FileName;
        }

        private void BtnSave_Click(object s, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBookTitle.Text))
            {
                MessageBox.Show("Введите название книги!");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtContentPath.Text))
            {
                MessageBox.Show("Укажите путь к файлу с текстом!");
                return;
            }

            Books book;
            if (_bookId.HasValue)
            {
                book = db.Books.Find(_bookId);
                if (book == null) return;
            }
            else
            {
                book = new Books
                {
                    AuthorId = SessionHelper.CurrentUser.UserId,
                    CreatedAt = DateTime.Now,
                    Status = "active",
                    AvgRating = 0
                };
                db.Books.Add(book);
            }

            book.Title = txtBookTitle.Text;
            book.Description = txtDescription.Text;
            book.ContentPath = txtContentPath.Text;
            db.SaveChanges();

            book.Genres.Clear();
            db.SaveChanges();

            if (!string.IsNullOrWhiteSpace(txtGenres.Text))
            {
                var genreNames = txtGenres.Text.Split(',')
                    .Select(g => g.Trim())
                    .Where(g => !string.IsNullOrEmpty(g))
                    .ToList();

                foreach (var genreName in genreNames)
                {
                    var genre = db.Genres.FirstOrDefault(ge => ge.GenreName == genreName);
                    if (genre == null)
                    {
                        genre = new Genres { GenreName = genreName };
                        db.Genres.Add(genre);
                        db.SaveChanges();
                    }
                    book.Genres.Add(genre);
                }
                db.SaveChanges();
            }

            MessageBox.Show("Сохранено!");
            NavigationService.GoBack();
        }

        private void BtnCancel_Click(object s, RoutedEventArgs e) => NavigationService.GoBack();
    }
}