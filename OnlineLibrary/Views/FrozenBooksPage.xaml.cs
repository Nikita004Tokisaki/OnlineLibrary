using OnlineLibrary.Dialogs;
using OnlineLibrary.Helpers;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OnlineLibrary.Views
{
    public partial class FrozenBooksPage : Page
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities();
        public FrozenBooksPage() 
        { 
            InitializeComponent(); 
            booksList.ItemsSource = db.Books.Where(b => b.AuthorId == SessionHelper.CurrentUser.UserId && b.Status == "frozen").ToList(); 
        }
        private void BtnAppeal_Click(object s, RoutedEventArgs e) => new AppealDialog("book", (int)(s as Button).Tag).ShowDialog();
    }
}
