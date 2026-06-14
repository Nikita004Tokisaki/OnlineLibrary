using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OnlineLibrary.Helpers;

namespace OnlineLibrary.Views
{
    public partial class AuthorPage : Page
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities();
        public AuthorPage() 
        { 
            InitializeComponent(); 
            LoadBooks(); 
        }
        private void LoadBooks() => booksList.ItemsSource = db.Books.Where(b => b.AuthorId == SessionHelper.CurrentUser.UserId).ToList();
        private void BtnAddBook_Click(object s, RoutedEventArgs e) => NavigationService.Navigate(new AddEditBookPage(null));
        private void BtnFrozen_Click(object s, RoutedEventArgs e) => NavigationService.Navigate(new FrozenBooksPage());
        private void BtnEdit_Click(object s, RoutedEventArgs e) => NavigationService.Navigate(new AddEditBookPage((int)(s as Button).Tag));
        private void BtnOpen_Click(object s, RoutedEventArgs e) => NavigationService.Navigate(new BookPage((int)(s as Button).Tag));
    }
}