using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OnlineLibrary.Views
{
    public partial class CatalogPage : Page
    {
        private OnlineLibraryEntities db = new OnlineLibraryEntities();

        public CatalogPage()
        {
            InitializeComponent();

            if (booksPanel == null)
            {
                MessageBox.Show("Ошибка: booksPanel не найден в XAML!");
                return;
            }

            LoadBooks();
        }

        private void LoadBooks()
        {
            if (booksPanel == null) return;

            var query = db.Books.Where(b => b.Status == "active").ToList();

            if (!string.IsNullOrEmpty(txtSearch?.Text))
                query = query.Where(b => b.Title.Contains(txtSearch.Text) || b.Users.DisplayName.Contains(txtSearch.Text)).ToList();

            if (cmbSort?.SelectedIndex == 1)
                query = query.OrderByDescending(b => b.AvgRating).ToList();
            else
                query = query.OrderBy(b => b.Title).ToList();

            if (cmbGenre?.SelectedItem != null && cmbGenre.SelectedIndex > 0)
            {
                string genre = cmbGenre.SelectedItem.ToString();
                query = query.Where(b => b.Genres.Any(g => g.GenreName == genre)).ToList();
            }

            booksPanel.Children.Clear();

            if (query.Count == 0)
            {
                booksPanel.Children.Add(new TextBlock
                {
                    Text = "📭 Книги не найдены",
                    FontSize = 14,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    Margin = new Thickness(20)
                });
            }
            else
            {
                foreach (var book in query)
                    booksPanel.Children.Add(CreateBookCard(book));
            }
        }

        private Border CreateBookCard(Books book)
        {
            var card = new Border { Style = (Style)FindResource("BookCardStyle") };
            var stack = new StackPanel();

            stack.Children.Add(new TextBlock { Text = "📖", FontSize = 70, TextAlignment = TextAlignment.Center, Margin = new Thickness(10, 15, 10, 5) });

            stack.Children.Add(new TextBlock { Text = book.Title, FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(10, 0, 10, 5), FontSize = 14 });

            stack.Children.Add(new TextBlock { Text = book.Users?.DisplayName ?? "Неизвестный", Foreground = System.Windows.Media.Brushes.Gray, Margin = new Thickness(10, 0, 10, 5), FontSize = 12 });

            stack.Children.Add(new TextBlock { Text = $"★ {book.AvgRating:F1}", Foreground = System.Windows.Media.Brushes.Gold, Margin = new Thickness(10, 0, 10, 10), FontSize = 13 });

            var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 0, 10, 15) };
            var openBtn = new Button { Content = "Открыть", Style = (Style)FindResource("PrimaryButton"), Width = 80, Height = 30, FontSize = 12 };
            openBtn.Click += (s, e) => NavigationService.Navigate(new BookPage(book.BookId));
            var addBtn = new Button { Content = "В список", Style = (Style)FindResource("PrimaryButton"), Width = 80, Height = 30, Margin = new Thickness(5, 0, 0, 0), FontSize = 12 };
            addBtn.Click += (s, e) => new Dialogs.ListSelectionDialog(book.BookId).ShowDialog();

            panel.Children.Add(openBtn);
            panel.Children.Add(addBtn);
            stack.Children.Add(panel);
            card.Child = stack;
            return card;
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e) => LoadBooks();
        private void CmbSort_Changed(object sender, SelectionChangedEventArgs e) => LoadBooks();
        private void CmbGenre_Changed(object sender, SelectionChangedEventArgs e) => LoadBooks();

        private void CmbGenre_Loaded(object sender, RoutedEventArgs e)
        {
            if (cmbGenre != null)
            {
                cmbGenre.Items.Clear();
                cmbGenre.Items.Add("Все жанры");
                foreach (var g in db.Genres.Select(g => g.GenreName).OrderBy(g => g))
                    cmbGenre.Items.Add(g);
                cmbGenre.SelectedIndex = 0;
            }
        }
    }
}
