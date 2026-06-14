using OnlineLibrary.Helpers;
using System;
using System.Windows;

namespace OnlineLibrary.Dialogs
{
    public partial class AppealDialog : Window
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities(); string _type; int? _bookId;
        public AppealDialog(string type, int? bookId = null) 
        { 
            InitializeComponent(); 
            _type = type; 
            _bookId = bookId; 
            txtTitle.Text = type == "user" ? "Оспаривание заморозки" : "Оспаривание заморозки книги"; 
        }
        private void BtnSubmit_Click(object s, RoutedEventArgs e) 
        { 
            if (_type == "user") db.UnfreezeRequests.Add(new UnfreezeRequests { UserId = SessionHelper.CurrentUser.UserId, AppealText = txtAppeal.Text, Status = "pending", CreatedAt = DateTime.Now }); 
            else db.BookFreezeAppeals.Add(new BookFreezeAppeals { BookId = _bookId.Value, AuthorId = SessionHelper.CurrentUser.UserId, AppealText = txtAppeal.Text, Status = "pending", CreatedAt = DateTime.Now }); 
            db.SaveChanges(); 
            MessageBox.Show("Заявка отправлена!"); 
            Close(); 
        }
        private void BtnCancel_Click(object s, RoutedEventArgs e) => Close();
    }
}
