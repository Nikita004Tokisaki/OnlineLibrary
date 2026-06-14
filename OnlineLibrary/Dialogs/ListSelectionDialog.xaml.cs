using OnlineLibrary.Helpers;
using System;
using System.Linq;
using System.Windows;

namespace OnlineLibrary.Dialogs
{
    public partial class ListSelectionDialog : Window
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities(); 
        int _bookId;
        public ListSelectionDialog(int bookId) 
        { 
            InitializeComponent(); 
            _bookId = bookId; 
        }
        private void Add(string type) 
        { 
            var ub = db.UserBooks.FirstOrDefault(x => x.UserId == SessionHelper.CurrentUser.UserId && x.BookId == _bookId); 
            if (ub != null) ub.ListType = type; 
            else db.UserBooks.Add(new UserBooks { UserId = SessionHelper.CurrentUser.UserId, BookId = _bookId, ListType = type, CreatedAt = DateTime.Now, UpdatedAt = DateTime.Now }); 
            db.SaveChanges();
            DialogResult = true; 
            Close(); 
        }
        private void BtnPlanned_Click(object s, EventArgs e) => Add("planned");
        private void BtnReading_Click(object s, EventArgs e) => Add("reading");
        private void BtnCompleted_Click(object s, EventArgs e) => Add("completed");
        private void BtnAbandoned_Click(object s, EventArgs e) => Add("abandoned");
    }
}
