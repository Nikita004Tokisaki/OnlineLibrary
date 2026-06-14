using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OnlineLibrary.Helpers;
using OnlineLibrary.Dialogs;

namespace OnlineLibrary.Views
{
    public partial class UserListsPage : Page
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities();
        string _listType = "planned";
        public UserListsPage() { InitializeComponent(); LoadBooks(); }

        private void LoadBooks()
        {
            var books = db.UserBooks.Where(ub => ub.UserId == SessionHelper.CurrentUser.UserId && ub.ListType == _listType).Select(ub => ub.Books).Where(b => b.Status == "active").ToList();
            booksPanel.Children.Clear();
            foreach (var b in books) booksPanel.Children.Add(CreateCard(b));
        }

        private Border CreateCard(Books book)
        {
            var card = new Border { Style = (Style)FindResource("BookCardStyle") };

            var stack = new StackPanel();

            stack.Children.Add(new TextBlock { Text = "📖", FontSize = 70, TextAlignment = TextAlignment.Center });

            stack.Children.Add(new TextBlock { Text = book.Title, FontWeight = FontWeights.Bold, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(10, 0, 10, 5), FontSize = 14 });

            var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10, 0, 10, 15) };

            var openBtn = new Button { Content = "Открыть", Style = (Style)FindResource("PrimaryButton"), Width = 80, Height = 30 };

            openBtn.Click += (s, e) => NavigationService.Navigate(new BookPage(book.BookId));

            var moveBtn = new Button { Content = "Переместить", Style = (Style)FindResource("PrimaryButton"), Width = 90, Height = 30, Margin = new Thickness(5, 0, 0, 0) };

            moveBtn.Click += (s, e) => { new ListSelectionDialog(book.BookId).ShowDialog(); LoadBooks(); };

            panel.Children.Add(openBtn); panel.Children.Add(moveBtn);

            stack.Children.Add(panel);

            card.Child = stack;

            return card;
        }

        private void Tab_Changed(object s, SelectionChangedEventArgs e) 
        { 
            if (tabLists.SelectedItem != null) 
            { 
                _listType = ((TabItem)tabLists.SelectedItem).Tag.ToString(); 
                LoadBooks(); 
            } 
        }
    }
}