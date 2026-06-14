using System.Windows;
using OnlineLibrary.Helpers;
using OnlineLibrary.Views;

namespace OnlineLibrary
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (SessionHelper.CurrentUser == null)
            {
                MessageBox.Show("Ошибка авторизации!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            if (SessionHelper.IsAdmin) btnAdmin.Visibility = Visibility.Visible;
            if (SessionHelper.IsAuthor) btnAuthorPage.Visibility = Visibility.Visible;
            if (SessionHelper.IsFrozen) warningBorder.Visibility = Visibility.Visible;

            MainFrame.Navigate(new CatalogPage());
        }
        private void BtnCatalog_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new CatalogPage());
        private void BtnLists_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new UserListsPage());
        private void BtnAdmin_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new AdminPage());
        private void BtnAuthorPage_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new AuthorPage());
        private void BtnProfile_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ProfilePage());
        private void BtnWarning_Click(object sender, RoutedEventArgs e) => MessageBox.Show($"Аккаунт заморожен.\nПричина: {SessionHelper.CurrentUser?.FreezeReason}\n\nОспорить можно в профиле.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
