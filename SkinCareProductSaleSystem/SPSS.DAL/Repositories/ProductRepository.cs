using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.DAL.Repositories
{
	public class ProductRepository
	{
		public List<Product> GetAllProducts()
		{
			using (var _context = new SkincareProductSaleSystemContext())
			{
				return _context.Products.ToList();
			}
		}

		public void UpdateProduct(Product product)
		{
			using (var _context = new SkincareProductSaleSystemContext())
			{
				_context.Products.Update(product); //trong ram
				_context.SaveChanges(); //chính thức update trong db
			}
		}
		//public Product GetProduct(int id)
		//{
		//	using (var _context = new SkincareProductSaleSystemContext())
		//	{
		//		return _context.Products.Find(id);
		//	}
		//}
		//public List<Product> GetProductsByCategory(string category)
		//{
		//	using (var _context = new SkincareProductSaleSystemContext())
		//	{
		//		return _context.Products.Where(p => p.Category == category).ToList();
		//	}
		//}
		//public List<Product> GetProductsByRating(double rating)
		//{
		//	using (var _context = new SkincareProductSaleSystemContext())
		//	{
		//		return _context.Products.Where(p => p.Rating == rating).ToList();
		//	}
		//}
		//public List<Product> GetProductsByPriceRange(double minPrice, double maxPrice)
		//{
		//	using (var _context = new SkincareProductSaleSystemContext())
		//	{
		//		return _context.Products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
		//	}
		//}
		//public List<Product> GetProductsByStock(int stock)
		//{
		//	using (var _context = new SkincareProductSaleSystemContext())
		//	{
		//		return _context.Products.Where(p => p.Stock == stock).ToList();
		//	}
		//}
		//public List<Product> GetProductsByKeyword(string keyword)
		//{
		//	using (var _context = new SkincareProductSaleSystemContext())
		//	{
		//		return _context.Products.Where(p => p.Name.Contains(keyword) || p.Description.Contains(keyword)).ToList();
		//	}
		//}
		//public List<Product> GetProductsByPromotion(int promotionId)
		//{
		//	using (var _context = new SkincareProductSaleSystemContext())
		//	{
		//		return _context.Products.Where(p => p.Promotions.Any(promotion => promotion.PromotionId == promotionId)).ToList();
		//	}
		//}
		//public List<Product> GetProductsByFeedback(int feedbackId)
		//{
		//	using (var _context = new SkincareProductSaleSystemContext())
		//	{
		//		return _context.Products.Where(p => p.Feedbacks.Any(feedback => feedback.FeedbackId == feedback))
		//	}
		//}
	}
}
