using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using SPSS.BLL.Services;
using SPSS.DAL.Models;

namespace SPSS.WPF
{
	public partial class SignupWindow : Window, INotifyPropertyChanged
	{
		private UserService _userService;
		private string _username;
		private string _password;
		private string _email;
		private string _phone;

		// Error messages
		private string _usernameError;
		private string _passwordError;
		private string _emailError;
		private string _phoneError;

		public SignupWindow()
		{
			_userService = new UserService();
			InitializeComponent();
			DataContext = this;
			SignUpCommand = new RelayCommand(SignUp, CanSignUp);
		}

		// Properties with validation
		public string Username
		{
			get => _username;
			set
			{
				_username = value;
				ValidateUsername();
				OnPropertyChanged(nameof(Username));
			}
		}

		public string Password
		{
			get => _password;
			set
			{
				_password = value;
				ValidatePassword();
				OnPropertyChanged(nameof(Password));
			}
		}

		public string Email
		{
			get => _email;
			set
			{
				_email = value;
				ValidateEmail();
				OnPropertyChanged(nameof(Email));
			}
		}

		public string Phone
		{
			get => _phone;
			set
			{
				_phone = value;
				ValidatePhone();
				OnPropertyChanged(nameof(Phone));
			}
		}

		// Error properties
		public string UsernameError
		{
			get => _usernameError;
			set
			{
				_usernameError = value;
				OnPropertyChanged(nameof(UsernameError));
				OnPropertyChanged(nameof(UsernameErrorVisibility));
				SignUpCommand.RaiseCanExecuteChanged();
			}
		}

		public string PasswordError
		{
			get => _passwordError;
			set
			{
				_passwordError = value;
				OnPropertyChanged(nameof(PasswordError));
				OnPropertyChanged(nameof(PasswordErrorVisibility));
				SignUpCommand.RaiseCanExecuteChanged();
			}
		}

		public string EmailError
		{
			get => _emailError;
			set
			{
				_emailError = value;
				OnPropertyChanged(nameof(EmailError));
				OnPropertyChanged(nameof(EmailErrorVisibility));
				SignUpCommand.RaiseCanExecuteChanged();
			}
		}

		public string PhoneError
		{
			get => _phoneError;
			set
			{
				_phoneError = value;
				OnPropertyChanged(nameof(PhoneError));
				OnPropertyChanged(nameof(PhoneErrorVisibility));
				SignUpCommand.RaiseCanExecuteChanged();
			}
		}

		// Error Visibility properties
		public Visibility UsernameErrorVisibility => string.IsNullOrEmpty(UsernameError) ? Visibility.Collapsed : Visibility.Visible;
		public Visibility PasswordErrorVisibility => string.IsNullOrEmpty(PasswordError) ? Visibility.Collapsed : Visibility.Visible;
		public Visibility EmailErrorVisibility => string.IsNullOrEmpty(EmailError) ? Visibility.Collapsed : Visibility.Visible;
		public Visibility PhoneErrorVisibility => string.IsNullOrEmpty(PhoneError) ? Visibility.Collapsed : Visibility.Visible;

		// Commands
		public RelayCommand SignUpCommand { get; private set; }

		// Validation methods
		private void ValidateUsername()
		{
			if (string.IsNullOrEmpty(Username))
				UsernameError = "Username is required";
			else if (Username.Length < 2 || Username.Length > 25)
				UsernameError = "Username must be between 2 and 25 characters";
			else
				UsernameError = null;
		}

		private void ValidatePassword()
		{
			if (string.IsNullOrEmpty(Password))
				PasswordError = "Password is required";
			else if (Password.Length < 6)
				PasswordError = "Password must be at least 6 characters";
			else
				PasswordError = null;
		}

		private void ValidateEmail()
		{
			string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
			if (string.IsNullOrEmpty(Email))
				EmailError = "Email is required";
			else if (!Regex.IsMatch(Email, emailPattern))
				EmailError = "Invalid email format";
			else
				EmailError = null;
		}

		private void ValidatePhone()
		{
			if (string.IsNullOrEmpty(Phone))
				PhoneError = "Phone is required";
			else if (!Regex.IsMatch(Phone, @"^\d{10}$"))
				PhoneError = "Phone must be exactly 10 digits";
			else
				PhoneError = null;
		}

		private bool CanSignUp()
		{
			return !string.IsNullOrEmpty(Username) &&
				   !string.IsNullOrEmpty(Password) &&
				   !string.IsNullOrEmpty(Email) &&
				   !string.IsNullOrEmpty(Phone) &&
				   string.IsNullOrEmpty(UsernameError) &&
				   string.IsNullOrEmpty(PasswordError) &&
				   string.IsNullOrEmpty(EmailError) &&
				   string.IsNullOrEmpty(PhoneError);
		}

		private void SignUp()
		{
			try
			{
				var user = new User
				{
					Username = Username,
					Password = Password,
					Email = Email,
					Phone = Phone,
				};

				_userService.SignUp(user);
				MessageBox.Show("Account created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

				LoginWindow loginWindow = new LoginWindow();
				loginWindow.Show();
				Close();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error creating account: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			Password = ((PasswordBox)sender).Password;
		}

		private void LoginLink_Click(object sender, RoutedEventArgs e)
		{
			var loginWindow = new LoginWindow();
			loginWindow.Show();
			Close();
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}