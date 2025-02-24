using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using SPSS.BLL.Services;
using SPSS.DAL.Models;

namespace SPSS.WPF
{
	public partial class ViewOrderHistoryWindow : Window, INotifyPropertyChanged
	{
		private readonly OrderService _orderService;
		private readonly OrderDetailService _orderDetailService;
		private readonly FeedbackService _feedbackService;

		private readonly User _currentUser;
		private Order _selectedOrder;
		private ObservableCollection<Order> _orders;
		private ObservableCollection<OrderDetail> _selectedOrderDetails;

		public ViewOrderHistoryWindow(User currentUser)
		{
			InitializeComponent();
			DataContext = this;

			_currentUser = currentUser;
			_orderService = new OrderService();
			_orderDetailService = new OrderDetailService();
			_feedbackService = new FeedbackService();

			SubmitFeedbackCommand = new RelayCommand<OrderDetail>(SubmitFeedback);

			LoadOrders();
		}
		private double _selectedRating;
		public double SelectedRating
		{
			get => _selectedRating;
			set
			{
				// Check if the value is a valid double and within range
				if (value < 1 || value > 5)
				{
					// Optionally, you can handle invalid values here
					return;
				}
				_selectedRating = value;
				OnPropertyChanged(nameof(SelectedRating));
			}
		}
		private string _feedbackComment;
		public string FeedbackComment
		{
			get => _feedbackComment;
			set
			{
				_feedbackComment = value;
				OnPropertyChanged(nameof(FeedbackComment));
			}
		}
		public ObservableCollection<Order> Orders
		{
			get => _orders;
			set
			{
				_orders = value;
				OnPropertyChanged(nameof(Orders));
			}
		}

		public Order SelectedOrder
		{
			get => _selectedOrder;
			set
			{
				_selectedOrder = value;
				LoadOrderDetails();
				OnPropertyChanged(nameof(SelectedOrder));
			}
		}

		public ObservableCollection<OrderDetail> SelectedOrderDetails
		{
			get => _selectedOrderDetails;
			set
			{
				_selectedOrderDetails = value;
				OnPropertyChanged(nameof(SelectedOrderDetails));
			}
		}

		public ICommand SubmitFeedbackCommand { get; }

		private void LoadOrders()
		{
			var userOrders = _orderService.GetOrdersByUserId(_currentUser.UserId)
										.OrderByDescending(o => o.OrderDate);
			Orders = new ObservableCollection<Order>(userOrders);

			if (Orders.Any())
			{
				SelectedOrder = Orders.First();
			}
		}

		private void LoadOrderDetails()
		{
			if (SelectedOrder != null)
			{
				var details = _orderDetailService.GetOrdersDetailsByOrderId(SelectedOrder.OrderId);
				SelectedOrderDetails = new ObservableCollection<OrderDetail>(details);
			}
			else
			{
				SelectedOrderDetails = new ObservableCollection<OrderDetail>();
			}
		}

		private void SubmitFeedback(OrderDetail orderDetail)
		{
			try
			{
				if (SelectedRating < 1 || SelectedRating > 5)
				{
					MessageBox.Show("Please enter a rating between 1 and 5.", "Invalid Rating",
						MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}
				Console.WriteLine($"Comment: {FeedbackComment}"); // Check this in your output window

				_feedbackService.AddFeedback(new Feedback
				{
					UserId = App.AuthenticatedUser.UserId,
					ProductId = orderDetail.ProductId,
					Rating = SelectedRating,
					Comment = FeedbackComment,
					CreatedAt = DateTime.Now,
				});

				MessageBox.Show("Feedback submitted successfully!", "Success",
					MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error submitting feedback: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed(e.Text, sender);
		}

		private bool IsTextAllowed(string text, object sender)
		{
			// Allow numbers, a decimal point, and handle the conversion
			string newText = (sender as TextBox).Text + text;
			return double.TryParse(newText, out double result) && result >= 1 && result <= 5;
		}

		private void RatingTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Decimal || e.Key == Key.OemPeriod) // Allow decimal point
			{
				var textBox = sender as TextBox;
				if (textBox != null && textBox.Text.Contains('.')) // Prevent more than one decimal
				{
					e.Handled = true;
				}
			}
		}
		private void RatingTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = sender as TextBox;
			if (double.TryParse(textBox.Text, out double rating))
			{
				SelectedRating = rating;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}