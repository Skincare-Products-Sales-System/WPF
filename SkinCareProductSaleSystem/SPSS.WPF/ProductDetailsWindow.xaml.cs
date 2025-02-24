using SPSS.BLL.Services;
using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ProductDetailsWindow.xaml
    /// </summary>
    public partial class ProductDetailsWindow : Window
    {
		private readonly ProductService _productService;
		private readonly FeedbackService _feedbackService;
		private readonly UserService _userService;
		private Product _product;

		public ProductDetailsWindow(Product product)
		{
			InitializeComponent();

			_productService = new ProductService();
			_feedbackService = new FeedbackService();
			_userService = new UserService();
			_product = product;

			LoadProductDetails();
			LoadFeedback();
		}

		private void LoadProductDetails()
		{
			if (_product == null)
			{
				MessageBox.Show("Error loading product details.", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
				return;
			}

			txtProductName.Text = _product.Name;
			txtCategory.Text = _product.Category;
			txtPrice.Text = _product.Price?.ToString("C");
			txtStock.Text = _product.Stock.ToString();
			txtDescription.Text = _product.Description;
		}

		private void LoadFeedback()
		{
			try
			{
				var feedbacks = _feedbackService.GetFeedbackByProductId(_product.ProductId);
				var feedbackViewModels = feedbacks.Select(f => new FeedbackViewModel
				{
					UserName = GetUserName(f.UserId),
					Rating = f.Rating,
					Comment = f.Comment,
					CreatedAt = f.CreatedAt
				}).ToList();

				FeedbackList.ItemsSource = feedbackViewModels;

				// Calculate average rating
				if (feedbackViewModels.Count > 0)
				{
					double averageRating = feedbackViewModels.Average(f => f.Rating ?? 0);
					txtAverageRating.Text = averageRating.ToString("F1"); // Format to one decimal place
				}
				else
				{
					txtAverageRating.Text = "No ratings yet";
				}
				// Cập nhật Visibility của NoFeedbackMessage
				NoFeedbackMessage.Visibility = feedbackViewModels.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error loading feedback: {ex.Message}", "Error",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private string GetUserName(int? userId)
		{
			try
			{
				var user = _userService.GetUserById(userId);
				return user?.Username ?? "Anonymous";
			}
			catch
			{
				return "Anonymous";
			}
		}
		public class FeedbackViewModel
		{	
			public string UserName { get; set; }
			public double? Rating { get; set; }
			public string Comment { get; set; }
			public DateTime? CreatedAt { get; set; }
		}
	}
}
