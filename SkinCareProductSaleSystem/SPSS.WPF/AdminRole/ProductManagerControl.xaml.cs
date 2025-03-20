using SPSS.BLL.Services;
using SPSS.DAL.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SPSS.WPF
{
	/// <summary>
	/// Interaction logic for ProductManagerControl.xaml
	/// </summary>
	public partial class ProductManagerControl : UserControl
	{
		private readonly ProductService _productService;
		private ObservableCollection<Product> _products;
		private Product _selectedProduct;

		public ProductManagerControl()
		{
			InitializeComponent();
			_productService = new ProductService();

			// Initialize the collection to prevent null reference
			_products = new ObservableCollection<Product>();

			// Ensure UI is completely initialized before loading data
			this.Loaded += (s, e) =>
			{
				LoadProducts();
			};
		}

		private void LoadProducts()
		{
			try
			{
				var products = _productService.GetAllProducts();
                _products.Clear(); // Xóa danh sách cũ trước khi tải mới
                if (products != null)
				{
					_products = new ObservableCollection<Product>(products);
					ProductsDataGrid.ItemsSource = _products;
					CategoryFilterComboBox.Items.Add(new ComboBoxItem { Content = "All category" });
					foreach (var category in products.Select(p => p.Category).Distinct())
					{
                        CategoryFilterComboBox.Items.Add(new ComboBoxItem { Content = category });
                    }
				}
				else
				{
					// Handle case where GetAllProducts returns null
					_products = new ObservableCollection<Product>();
					ProductsDataGrid.ItemsSource = _products;
					MessageBox.Show("No products found or unable to load products.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				_products = new ObservableCollection<Product>();
				ProductsDataGrid.ItemsSource = _products;
			}
		}

		private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			FilterProducts();
		}

		private void CategoryFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			FilterProducts();
		}

		private void FilterProducts()
		{
			// Ensure we have products to filter
			if (_products == null)
			{
				_products = new ObservableCollection<Product>();
				return;
			}

			string searchText = SearchTextBox?.Text?.ToLower() ?? "";

			// Get the selected category name from the ComboBox
			string selectedCategoryName = null;
			if (CategoryFilterComboBox != null && CategoryFilterComboBox.SelectedItem != null)
			{
				selectedCategoryName = (CategoryFilterComboBox.SelectedItem as ComboBoxItem)?.Content as string;
			}

			try
			{
				var filteredProducts = _products.Where(p =>
					(string.IsNullOrWhiteSpace(searchText) ||
					 (p.Name != null && p.Name.ToLower().Contains(searchText)) ||
					 (p.Description != null && p.Description.ToLower().Contains(searchText))) &&
                    (string.IsNullOrEmpty(selectedCategoryName) ||
					 selectedCategoryName == "All category" || // Thêm điều kiện cho "All category"
					 p.Category == selectedCategoryName)
						).ToList();

				ProductsDataGrid.ItemsSource = new ObservableCollection<Product>(filteredProducts);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error filtering products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				_selectedProduct = ProductsDataGrid.SelectedItem as Product;
				if (_selectedProduct != null)
				{
					ProductDetailsPanel.Visibility = Visibility.Visible;
					DisplayProductDetails(_selectedProduct);
				}
				else
				{
					ProductDetailsPanel.Visibility = Visibility.Collapsed;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error selecting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void DisplayProductDetails(Product product)
		{
			if (product == null) return;

			try
			{
				// Display product details
				ProductNameTextBlock.Text = product.Name ?? "No name";
				PriceTextBlock.Text = $"${product.Price:N2}";
				StockTextBlock.Text = product.Stock.ToString();
				DescriptionTextBlock.Text = product.Description ?? "No description";
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error displaying product details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void AddProduct_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				ProductFormWindow productFormWindow = new ProductFormWindow();
				productFormWindow.Closed += (s, args) => LoadProducts();
				productFormWindow.ShowDialog();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error opening product form: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void EditProduct_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Product product = (sender as Button)?.CommandParameter as Product;
				if (product != null)
				{
					ProductFormWindow productFormWindow = new ProductFormWindow(product);
					productFormWindow.Closed += (s, args) => LoadProducts();
					productFormWindow.ShowDialog();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error editing product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void DeleteProduct_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Product product = (sender as Button)?.CommandParameter as Product;
				if (product != null)
				{
					MessageBoxResult result = MessageBox.Show(
						$"Are you sure you want to delete {product.Name}?",
						"Confirm Delete",
						MessageBoxButton.YesNo,
						MessageBoxImage.Warning);
					if (result == MessageBoxResult.Yes)
					{
						try
						{
							_productService.DeleteProduct(product.ProductId);
							LoadProducts();
							if (_selectedProduct?.ProductId == product.ProductId)
							{
								ProductDetailsPanel.Visibility = Visibility.Collapsed;
							}
						}
						catch (Exception ex)
						{
							MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error processing delete request: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}