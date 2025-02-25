using System;
using System.Windows;
using System.Windows.Controls;
namespace SPSS.WPF
{
	public partial class AdminWindow : Window
	{
		public AdminWindow()
		{
			InitializeComponent();
			// Đặt nút Account Manager là mặc định được chọn
			AccountManagerButton.IsChecked = true;
		}

		private void AccountManagerButton_Checked(object sender, RoutedEventArgs e)
		{
			if (HeaderTextBlock != null)
			{
				HeaderTextBlock.Text = "Account Management";
				// Điều hướng đến UserControl thay vì Window
				ContentFrame.Content = new AccountManagerControl();
			}
		}

		private void ProductManagerButton_Checked(object sender, RoutedEventArgs e)
		{
			if (HeaderTextBlock != null)
			{
				HeaderTextBlock.Text = "Product Management";
				ContentFrame.Content = new ProductManagerControl();
			}
		}

		private void QuestionManagerButton_Checked(object sender, RoutedEventArgs e)
		{
			if (HeaderTextBlock != null)
			{
				HeaderTextBlock.Text = "Question Management";
				ContentFrame.Content = new QuestionManagerControl();
			}
		}

		private void LogoutButton_Click(object sender, RoutedEventArgs e)
		{
			MainWindow loginWindow = new MainWindow();
			loginWindow.Show();
			this.Close();
		}
	}
}