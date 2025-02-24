using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPSS.WPF
{
    /// <summary>
    /// Interaction logic for QuantitySelectionWindow.xaml
    /// </summary>
    public partial class QuantitySelectionWindow : Window,INotifyPropertyChanged
    {
		private int _selectedQuantity = 1;
		private string _errorMessage;
		private bool _hasError;
		private bool _isValid = true;
		private int? stock;

		public int MaxQuantity { get; }

		public int SelectedQuantity
		{
			get => _selectedQuantity;
			set
			{
				if (_selectedQuantity != value)
				{
					_selectedQuantity = value;
					ValidateQuantity();
					OnPropertyChanged(nameof(SelectedQuantity));
				}
			}
		}

		public string ErrorMessage
		{
			get => _errorMessage;
			set
			{
				_errorMessage = value;
				OnPropertyChanged(nameof(ErrorMessage));
			}
		}

		public bool HasError
		{
			get => _hasError;
			set
			{
				_hasError = value;
				OnPropertyChanged(nameof(HasError));
			}
		}

		public bool IsValid
		{
			get => _isValid;
			set
			{
				_isValid = value;
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public QuantitySelectionWindow(int maxQuantity)
		{
			InitializeComponent();
			MaxQuantity = maxQuantity;
			DataContext = this;
		}

		private void ValidateQuantity()
		{
			if (SelectedQuantity < 1)
			{
				ErrorMessage = "Quantity cannot be less than 1";
				HasError = true;
				IsValid = false;
			}
			else if (SelectedQuantity > MaxQuantity)
			{
				ErrorMessage = $"Only {MaxQuantity} items available in stock";
				HasError = true;
				IsValid = false;
			}
			else
			{
				ErrorMessage = string.Empty;
				HasError = false;
				IsValid = true;
			}
		}

		private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
		{
			SelectedQuantity++;
		}

		private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
		{
			if (SelectedQuantity > 1)
			{
				SelectedQuantity--;
			}
		}

		private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Allow only numbers
			e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
		}

		private void Confirm_Click(object sender, RoutedEventArgs e)
		{
			if (IsValid)
			{
				DialogResult = true;
				Close();
			}
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
