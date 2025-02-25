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

			// Store the authenticated user
			App.AuthenticatedUser = acc;

			// Open the appropriate window based on user role and skin type
			if (acc.Role == 2)
			{
				AdminWindow adminWindow = new AdminWindow();
				adminWindow.Show();
			}
			else if (acc.Role == 1 && string.IsNullOrWhiteSpace(acc.SkinType))
			{
				QuestionWindow questionWindow = new QuestionWindow
				{
					AuthenticatedUser = acc // Pass the authenticated user
				};
				questionWindow.Show();
			}
			else
			{
				MainWindow mainWindow = new MainWindow
				{
					AuthenticatedUser = acc, // Assign user
					WelcomeMessage = $"Welcome, {acc.Username}!" // Update welcome message
				};
				mainWindow.Show();
			}

			this.Hide(); // Hide the current window
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
