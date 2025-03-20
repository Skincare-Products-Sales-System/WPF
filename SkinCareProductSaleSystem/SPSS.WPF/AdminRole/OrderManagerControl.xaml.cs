using SPSS.BLL.Services;
using SPSS.DAL.Models;
using SPSS.DAL.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SPSS.WPF.AdminRole
{
    public partial class OrderManagerControl : UserControl
    {
        private readonly OrderService _orderService = new OrderService();
        private ObservableCollection<Order> _orders;
        private Order _selectedOrder;

        public OrderManagerControl()
        {
            InitializeComponent(); // Phải được gọi trước
            _orderService = new OrderService();
            _orders = new ObservableCollection<Order>();

            // Chỉ gọi LoadOrders sau khi giao diện đã sẵn sàng
            Loaded += (s, e) => LoadOrders();
        }


        private void LoadOrders()
        {
            Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    if (_orders == null)
                    {
                        _orders = new ObservableCollection<Order>();
                    }
                    else
                    {
                        _orders.Clear(); // Xóa dữ liệu cũ trước khi thêm mới
                    }

                    var orders = _orderService.GetAllOrdersAsync();
                    if (orders != null && orders.Any())
                    {
                        foreach (var order in orders)
                        {
                            if (order != null)
                            {
                                _orders.Add(order);
                            }
                        }

                        OrdersDataGrid.ItemsSource = _orders;
                    }
                    else
                    {
                        OrdersDataGrid.ItemsSource = _orders;
                        MessageBox.Show("No orders found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading orders: {ex.Message}\nStackTrace: {ex.StackTrace}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _orders?.Clear();
                    OrdersDataGrid.ItemsSource = _orders;
                }
            });
        }




        private void FilterOrders()
        {
            if (_orders == null || !_orders.Any())
            {
                LoadOrders();
                return;
            }

            string searchText = SearchTextBox?.Text?.Trim() ?? "";
            int? selectedStatus = null;
            if (StatusFilterComboBox?.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                selectedStatus = Convert.ToInt32(selectedItem.Tag);
            }

            try
            {
                var filteredOrders = _orders.Where(o =>
                    (string.IsNullOrEmpty(searchText) ||
                     o.OrderId.ToString().Contains(searchText) ||
                     o.UserId.ToString().Contains(searchText)) &&
                    (!selectedStatus.HasValue || o.Status == selectedStatus)
                ).ToList();

                OrdersDataGrid.ItemsSource = new ObservableCollection<Order>(filteredOrders);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering orders: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterOrders();
        }

        private void StatusFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterOrders();
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _selectedOrder = OrdersDataGrid?.SelectedItem as Order;
                if (_selectedOrder != null)
                {
                    OrderDetailsPanel.Visibility = Visibility.Visible;
                    DisplayOrderDetails(_selectedOrder);
                }
                else
                {
                    OrderDetailsPanel.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DisplayOrderDetails(Order order)
        {
            if (order == null) return;

            try
            {
                OrderIdTextBlock.Text = $"Order ID: {order.OrderId}";
                UserIdTextBlock.Text = $"User ID: {order.UserId}";
                OrderDateTextBlock.Text = $"Order Date: {order.OrderDate:dd/MM/yyyy HH:mm}";
                TotalAmountTextBlock.Text = $"Total Amount: ${order.TotalAmount:N2}";
                FinalAmountTextBlock.Text = $"Final Amount: ${order.FinalAmount:N2}";
                StatusTextBlock.Text = $"Status: {ConvertStatusToString(order.Status)}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying order details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ConvertStatusToString(int? status)
        {
            return status switch
            {
                0 => "Chưa Thanh Toán",
                1 => "Đã Thanh Toán",
                _ => "Unknown"
            };
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadOrders();
            SearchTextBox.Text = "";
            StatusFilterComboBox.SelectedIndex = 0; // Reset to "All Statuses"
        }
    }

    // Converter for Status in DataGrid (optional, if you want to use it)
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int status)
            {
                return status switch
                {
                    0 => "Chưa Thanh Toán",
                    1 => "Đã Thanh Toán",
                    _ => "Unknown"
                };
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}