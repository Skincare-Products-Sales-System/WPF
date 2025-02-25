using Microsoft.Win32;
using SPSS.BLL.Services;
using SPSS.DAL.Models;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SPSS.WPF
{
	/// <summary>
	/// Interaction logic for ProductFormWindow.xaml
	/// </summary>
	public partial class ProductFormWindow : Window
	{
		private readonly ProductService _productService;
		private Product _product;
		private bool _isEditMode;

		public ProductFormWindow()
		{
			InitializeComponent();
			_productService = new ProductService();
			_isEditMode = false;
		}

		public ProductFormWindow(Product product) : this()
		{
			_product = product;
			_isEditMode = true;
			WindowTitleText.Text = "Edit Product";
			LoadProductData();
		}

		private void LoadProductData()
		{
			if (_product != null)
			{
				ProductNameTextBox.Text = _product.Name;
				CategoryTextBox.Text = _product.Category;
				PriceTextBox.Text = _product.Price?.ToString() ?? "0"; // Use "0" or any default string
				StockQuantityTextBox.Text = _product.Stock.ToString();
				DescriptionTextBox.Text = _product.Description;
			}
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			if (ValidateForm())
			{
				SaveProduct();
				DialogResult = true;
				Close();
			}
		}

		private bool ValidateForm()
		{
			// Check required fields
			if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
			{
				MessageBox.Show("Product name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				ProductNameTextBox.Focus();
				return false;
			}

			if (string.IsNullOrWhiteSpace(CategoryTextBox.Text))
			{
				MessageBox.Show("Category is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				CategoryTextBox.Focus();
				return false;
			}

			if (string.IsNullOrWhiteSpace(PriceTextBox.Text) || !decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
			{
				MessageBox.Show("Please enter a valid price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				PriceTextBox.Focus();
				return false;
			}

			if (string.IsNullOrWhiteSpace(StockQuantityTextBox.Text) || !int.TryParse(StockQuantityTextBox.Text, out int stock) || stock < 0)
			{
				MessageBox.Show("Please enter a valid stock quantity.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
				StockQuantityTextBox.Focus();
				return false;
			}

			return true;
		}

		private void SaveProduct()
		{
			try
			{
				if (!_isEditMode)
				{
					_product = new Product();
				}

				_product.Name = ProductNameTextBox.Text.Trim();
				_product.Category = CategoryTextBox.Text.Trim();

				_product.Price = double.Parse(PriceTextBox.Text);
				_product.Stock = int.Parse(StockQuantityTextBox.Text);
				_product.Description = DescriptionTextBox.Text.Trim();

				if (_isEditMode)
				{
					_productService.UpdateProduct(_product);
				}
				else
				{
					_productService.AddProduct(_product);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}