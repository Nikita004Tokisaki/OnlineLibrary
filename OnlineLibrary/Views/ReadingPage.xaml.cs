using System.IO;
using System.Windows;
using System.Windows.Controls;
using OnlineLibrary.Helpers;

namespace OnlineLibrary.Views
{
    public partial class ReadingPage : Page
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities();
        public ReadingPage(int bookId)
        {
            InitializeComponent();
            var b = db.Books.Find(bookId);
            txtTitle.Text = b.Title;
            txtContent.Text = SessionHelper.IsFrozen ? "Аккаунт заморожен!" : File.Exists(b.ContentPath) ? File.ReadAllText(b.ContentPath) : "Текст не найден";
        }
        private void BtnBack_Click(object s, RoutedEventArgs e) => NavigationService.GoBack();
    }
}