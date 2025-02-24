using GalaSoft.MvvmLight.Command;
using SPSS.BLL.Services;
using SPSS.DAL.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SPSS.WPF
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		private readonly ProductService _productService;
		private List<Product> _allProducts;
		private List<Product> _filteredProducts;
		private int _currentPage;
		private const int PageSize = 6;
		private string _welcomeMessage;
		private User _authenticatedUser;
		private string _searchText;
		private Category _selectedCategory;
		private CartWindow _cartWindow;


		// Commands
		private readonly ICommand _nextPageCommand;
		private readonly ICommand _previousPageCommand;
		private readonly ICommand _addToCartCommand;
		private readonly ICommand _clearFiltersCommand;
		private readonly ICommand _viewProductDetail;

		public string SearchText
		{
			get => _searchText;
			set
			{
				_searchText = value;
				OnPropertyChanged(nameof(SearchText));
				ApplyFilters();
			}
		}

		public ObservableCollection<Category> Categories { get; private set; }

		public Category SelectedCategory
		{
			get => _selectedCategory;
			set
			{
				_selectedCategory = value;
				OnPropertyChanged(nameof(SelectedCategory));
				ApplyFilters();
			}
		}

		public string WelcomeMessage
		{
			get => _welcomeMessage;
			set
			{
				_welcomeMessage = value;
				OnPropertyChanged(nameof(WelcomeMessage));
			}
		}

		public User AuthenticatedUser
		{
			get => _authenticatedUser;
			set
			{
				_authenticatedUser = value;
				WelcomeMessage = _authenticatedUser != null
					? $"Welcome, {_authenticatedUser.Username}!"
					: "Welcome, Guest!";
				OnPropertyChanged(nameof(AuthenticatedUser));
			}
		}

		public ObservableCollection<Product> PagedProducts { get; set; }

		// Updated Command properties to use backing fields
		public ICommand NextPageCommand => _nextPageCommand;
		public ICommand PreviousPageCommand => _previousPageCommand;
		public ICommand AddToCartCommand => _addToCartCommand;
		public ICommand ClearFiltersCommand => _clearFiltersCommand;
		public ICommand ViewDetailProduct => _viewProductDetail;

		public bool CanGoNext => (_currentPage + 1) * PageSize < _filteredProducts.Count;
		public bool CanGoPrevious => _currentPage > 0;
		public string CurrentPageDisplay => $"Page {_currentPage + 1} of {((_filteredProducts.Count - 1) / PageSize) + 1}";

		public event PropertyChangedEventHandler PropertyChanged;

		public MainWindow()
		{
			InitializeComponent();

			_productService = new ProductService();
			_allProducts = _productService.GetAllProducts();
			_filteredProducts = new List<Product>(_allProducts);
			PagedProducts = new ObservableCollection<Product>();

			InitializeCategories();

			// Initialize commands using the backing fields
			_nextPageCommand = new RelayCommand(NextPage, () => CanGoNext);
			_previousPageCommand = new RelayCommand(PreviousPage, () => CanGoPrevious);
			_addToCartCommand = new RelayCommand<Product>(AddToCart);
			_clearFiltersCommand = new RelayCommand(ClearFilters);
			_viewProductDetail = new RelayCommand<Product>(OnViewDetails);

			_cartWindow = new CartWindow();
			LoadPage();
			DataContext = this;
		}

		private void InitializeCategories()
		{
			var uniqueCategories = _allProducts
				.Select(p => p.Category)
				.Distinct()
				.OrderBy(c => c)
				.ToList();

			Categories = new ObservableCollection<Category>
			{
				new Category { Name = "All Categories" }
			};

			foreach (var categoryName in uniqueCategories)
			{
				Categories.Add(new Category { Name = categoryName });
			}

			SelectedCategory = Categories.First();
		}

		private void ApplyFilters()
		{
			_filteredProducts = _allProducts;

			if (!string.IsNullOrWhiteSpace(SearchText))
			{
				_filteredProducts = _filteredProducts
					.Where(p => p.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase))
					.ToList();
			}

			if (SelectedCategory != null && SelectedCategory.Name != "All Categories")
			{
				_filteredProducts = _filteredProducts
					.Where(p => p.Category == SelectedCategory.Name)
					.ToList();
			}

			_currentPage = 0;
			LoadPage();
		}
		private void OnViewDetails(Product product)
		{
			// Implement your view details logic here
			// For example, open a new window with product details
			var detailsWindow = new ProductDetailsWindow(product);
			detailsWindow.Show();
		}
		private void ClearFilters()
		{
			SearchText = string.Empty;
			SelectedCategory = Categories.First();
			_filteredProducts = new List<Product>(_allProducts);
			_currentPage = 0;
			LoadPage();
		}

		private void LoadPage()
		{
			PagedProducts.Clear();
			var pageItems = _filteredProducts
				.Skip(_currentPage * PageSize)
				.Take(PageSize)
				.ToList();

			foreach (var item in pageItems)
			{
				PagedProducts.Add(item);
			}

			OnPropertyChanged(nameof(CanGoNext));
			OnPropertyChanged(nameof(CanGoPrevious));
			OnPropertyChanged(nameof(CurrentPageDisplay));
		}

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			ClearFilters();
		}

		private void NextPage()
		{
			if (CanGoNext)
			{
				_currentPage++;
				LoadPage();
				RefreshCommands();
			}
		}

		private void PreviousPage()
		{
			if (CanGoPrevious)
			{
				_currentPage--;
				LoadPage();
				RefreshCommands();
			}
		}

		private void RefreshCommands()
		{
			(NextPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
			(PreviousPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
		}

		private void AddToCart(Product product)
		{
			// Show quantity selection dialog
			var quantityDialog = new QuantitySelectionWindow(product.Stock);
			if (quantityDialog.ShowDialog() == true)
			{
				var quantity = quantityDialog.SelectedQuantity;

				// Find existing cart item
				var existingItem = _cartWindow.CartItems.FirstOrDefault(i => i.Product.ProductId == product.ProductId);

				if (existingItem != null)
				{
					// Update quantity if item exists
					existingItem.Quantity += quantity;
				}
				else
				{
					// Add new item if it doesn't exist
					_cartWindow.CartItems.Add(new CartItem
					{
						Product = product,
						Quantity = quantity
					});
				}

				// Show the cart window
				if (!_cartWindow.IsVisible)
				{
					_cartWindow.Show();
				}
			}
		}

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		//View History
		private void MenuItem_Click_1(object sender, RoutedEventArgs e)
		{
			var user = App.AuthenticatedUser;
			var viewOrderHistoryWindow = new ViewOrderHistoryWindow(user);
			viewOrderHistoryWindow.Show();
		}

		//Log out
		private void MenuItem_Click_2(object sender, RoutedEventArgs e)
		{
			// Reset the AuthenticatedUser
			App.AuthenticatedUser = null;

			// Optionally, you can open the LoginWindow again
			var loginWindow = new LoginWindow();
			loginWindow.Show();

			// Close the current window
			this.Close();
		}

	}

	public class Category
	{
		public string Name { get; set; }
	}
}