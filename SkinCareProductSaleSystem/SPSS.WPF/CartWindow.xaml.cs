using GalaSoft.MvvmLight.Command;
using SPSS.BLL.Services;
using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace SPSS.WPF
{
    /// <summary>
    /// Interaction logic for CartWindow.xaml
    /// </summary>
    public partial class CartWindow : Window, INotifyPropertyChanged
	{
		private readonly OrderService _orderService;
		private readonly OrderDetailService _orderDetailService;
		private readonly ProductService _productService;
		private ObservableCollection<CartItem> _cartItems;
		public ObservableCollection<CartItem> CartItems
		{
			get => _cartItems;
			set
			{
				_cartItems = value;
				OnPropertyChanged(nameof(CartItems));
				OnPropertyChanged(nameof(TotalAmount));
			}
		}
	
		public double? TotalAmount => CartItems.Sum(item => item.TotalPrice);
		public bool IsCheckoutEnabled => CartItems.Any();
		public ICommand IncreaseQuantityCommand { get; }
		public ICommand DecreaseQuantityCommand { get; }
		public ICommand RemoveFromCartCommand { get; }
		public ICommand CheckoutCommand { get; }

		public CartWindow()
		{
			InitializeComponent();
			CartItems = new ObservableCollection<CartItem>();
			CartItems.CollectionChanged += (s, e) =>
			{
				OnPropertyChanged(nameof(TotalAmount));
				OnPropertyChanged(nameof(IsCheckoutEnabled)); // Notify that checkout enabled status has changed
			};

			IncreaseQuantityCommand = new RelayCommand<CartItem>(IncreaseQuantity);
			DecreaseQuantityCommand = new RelayCommand<CartItem>(DecreaseQuantity);
			RemoveFromCartCommand = new RelayCommand<CartItem>(RemoveFromCart);
			CheckoutCommand = new RelayCommand(Checkout);

			DataContext = this;
			_orderDetailService = new OrderDetailService();
			_productService = new ProductService();
			_orderService = new OrderService();

		}

		private void IncreaseQuantity(CartItem item)
		{
			if (item.Quantity < item.Product.Stock)
			{
				item.Quantity++;
				OnPropertyChanged(nameof(TotalAmount));

			}
		}

		private void DecreaseQuantity(CartItem item)
		{
			if (item.Quantity > 1)
			{
				item.Quantity--;
				OnPropertyChanged(nameof(TotalAmount));
			}
		}

		private void RemoveFromCart(CartItem item)
		{
			CartItems.Remove(item);
			    OnPropertyChanged(nameof(TotalAmount)); // Update total amount when removing

		}
		protected override void OnClosing(CancelEventArgs e)
		{
			// Prevent the window from closing, just hide it
			e.Cancel = true; // Cancel the closing event
			this.Hide(); // Hide the window instead of closing it
		}
		private void Checkout()
		{
			// Display confirmation dialog
			var result = MessageBox.Show("Are you sure you want to place the order?",
										   "Confirm Order",
										   MessageBoxButton.YesNo,
										   MessageBoxImage.Question);

			// If the user clicks "No", exit the method early
			if (result == MessageBoxResult.No)
			{
				return;
			}

			try
			{
					// Create new order
					var order = new Order
					{
						UserId = App.AuthenticatedUser.UserId, // Assuming you have current user stored in App
						OrderDate = DateTime.Now,
						TotalAmount = TotalAmount,
						FinalAmount = TotalAmount, // If no discounts applied
						Status = 1 // Assuming 1 is for "New" or "Pending" status
					};

				//Check authen
				//if (App.AuthenticatedUser?.UserId == null)
				//{
				//	MessageBox.Show("User not authenticated!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				//	return;
				//}
				MessageBox.Show(order.UserId.ToString() + ", " + order.UserId + ", " +order.TotalAmount + ", " + order.FinalAmount + ", " + order.Status);

				// Create the order in database to get the OrderId
				int orderId = _orderService.CreateOrderAsync(order);

					// Now that the order is created, assign the OrderId to OrderDetails
					foreach (var item in CartItems)
					{
						var orderDetail = new OrderDetail
						{
							OrderId = order.OrderId, // Set the generated OrderId
							ProductId = item.Product.ProductId,
							Quantity = item.Quantity,
							UnitPrice = item.Product.Price,
							TotalPrice = item.TotalPrice
						};

					// Save each OrderDetail
					_orderDetailService.CreateOrderDetail(orderDetail);

					// Decrease the stock of the product
					item.Product.Stock -= item.Quantity;
					_productService.UpdateProduct(item.Product); // Update the product in the 
				}

					// Show success message
					MessageBox.Show("Order placed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

					// Clear the cart
					CartItems.Clear();
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error creating order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
