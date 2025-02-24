using SPSS.BLL.Services;
using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace SPSS.WPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private UserService _userService = new();
        public LoginWindow()
        {
            InitializeComponent();
        }



        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Password;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Both username and password are required!", "Fields required", MessageBoxButton.OK, MessageBoxImage.Error);
                return; //early return, ko cần viết else
            }
            User? acc = _userService.Authenticate(username, password);

            if (acc == null)
            {
                MessageBox.Show("Invalid username or password!", "Wrong credentials", MessageBoxButton.OK, MessageBoxImage.Error);
                return; //early return, ko cần viết else
            }

            if (acc.SkinType == null || acc.SkinType == "")
            {
				App.AuthenticatedUser = acc; // Store the authenticated user in App.xaml.cs
				QuestionWindow main = new();
                main.AuthenticatedUser = acc; //GỬI ACC VỪA LOGIN THÀNH CÔNG SANG KIA LÀM BIẾN
                main.Show();
                this.Hide();
            }
            else
            {
				App.AuthenticatedUser = acc; // Store the authenticated user in App.xaml.cs

				MainWindow mainWindow = new();
				mainWindow.AuthenticatedUser = acc;  // Gán user
				mainWindow.WelcomeMessage = $"Welcome, {acc.Username}!"; // Cập nhật thông báo
				mainWindow.Show();
                this.Hide();
            }
        }
		private void btClose_Click(object sender, RoutedEventArgs e)
		{
			// Close the application
			Application.Current.Shutdown();
		}
		private void btSignup_Click(object sender, RoutedEventArgs e)
        {
            SignupWindow signupWindow = new SignupWindow();
            signupWindow.Show();
            this.Close();
        }
    }
}
