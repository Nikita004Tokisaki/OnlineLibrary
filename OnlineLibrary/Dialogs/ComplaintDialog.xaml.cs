using OnlineLibrary.Helpers;
using System;
using System.Windows;

namespace OnlineLibrary.Dialogs
{
    public partial class ComplaintDialog : Window
    {
        OnlineLibraryEntities db = new OnlineLibraryEntities(); 
        string _type; 
        int _targetId;
        public ComplaintDialog(string type, int targetId, string targetName) 
        { 
            InitializeComponent(); 
            _type = type; 
            _targetId = targetId; 
            txtTitle.Text = $"Жалоба на {targetName}"; 
        }
        private void BtnSubmit_Click(object s, RoutedEventArgs e) 
        { 
            db.Complaints.Add(new Complaints { Type = _type, TargetId = _targetId, ComplainantId = SessionHelper.CurrentUser.UserId, Reason = txtReason.Text, Status = "pending", CreatedAt = DateTime.Now }); 
            db.SaveChanges(); 
            MessageBox.Show("Жалоба отправлена!"); 
            Close(); 
        }
        private void BtnCancel_Click(object s, RoutedEventArgs e) => Close();
    }
}
