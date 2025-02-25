using SPSS.BLL.Services;
using SPSS.DAL.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SPSS.WPF
{
	/// <summary>
	/// Interaction logic for AccountManagerControl.xaml
	/// </summary>
	public partial class AccountManagerControl : UserControl
	{
		private readonly UserService _userService;
		private ObservableCollection<User> _users;
		private User _selectedUser;

		public AccountManagerControl()
		{
			InitializeComponent();
			_userService = new UserService();
			LoadUsers();
		}

		private void LoadUsers()
		{
			try
			{
				var users = _userService.GetAllUsers();
				_users = new ObservableCollection<User>(users);
				UsersDataGrid.ItemsSource = _users;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			string searchText = SearchTextBox.Text.ToLower();
			if (string.IsNullOrWhiteSpace(searchText))
			{
				LoadUsers();
				return;
			}

			var filteredUsers = _users.Where(u =>
				u.Username.ToLower().Contains(searchText) ||
				u.Email.ToLower().Contains(searchText) ||
				u.Phone.ToLower().Contains(searchText)).ToList();

			UsersDataGrid.ItemsSource = new ObservableCollection<User>(filteredUsers);
		}

		private void UsersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (UsersDataGrid.SelectedItem is User selectedUser)
			{
				_selectedUser = selectedUser;
				DisplayUserDetails(_selectedUser);
				UserDetailsBorder.Visibility = Visibility.Visible;
			}
			else
			{
				UserDetailsBorder.Visibility = Visibility.Collapsed;
			}
		}

		private void DisplayUserDetails(User user)
		{
			UsernameTextBlock.Text = user.Username;
			EmailTextBlock.Text = user.Email;
			RoleTextBlock.Text = user.Role.ToString();
			PhoneTextBlock.Text = user.Phone;
		}

		private void DeleteUser_Click(object sender, RoutedEventArgs e)
		{
			if ((sender as Button)?.CommandParameter is User user)
			{
				DeleteUser(user);
			}
		}

		private void DeleteUser(User user)
		{
			MessageBoxResult result = MessageBox.Show(
				$"Are you sure you want to delete {user.Username}?",
				"Confirm Delete",
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				try
				{
					_userService.DeleteUser(user.UserId);
					_users.Remove(user);

					if (_selectedUser?.UserId == user.UserId)
					{
						_selectedUser = null;
						UserDetailsBorder.Visibility = Visibility.Collapsed;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
	}
}
