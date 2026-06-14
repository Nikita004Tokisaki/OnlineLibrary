using System.Windows;

namespace OnlineLibrary.Dialogs
{
    public partial class FreezeUserDialog : Window
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities(); 
        int _userId;
        public FreezeUserDialog(int userId) 
        { 
            InitializeComponent(); 
            _userId = userId; 
        }
        private void BtnFreeze_Click(object s, RoutedEventArgs e) 
        { 
            var u = db.Users.Find(_userId); 
            u.Status = "frozen"; 
            u.FreezeReason = txtReason.Text; 
            db.SaveChanges(); MessageBox.Show("Пользователь заморожен!"); 
            Close(); 
        }
        private void BtnCancel_Click(object s, RoutedEventArgs e) => Close();
    }
}
