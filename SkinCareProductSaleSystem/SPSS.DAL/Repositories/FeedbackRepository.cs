using Microsoft.EntityFrameworkCore;
using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.DAL.Repositories
{
	public class FeedbackRepository
	{
		public List<Feedback> GetFeedbackByProductId(int productId)
		{
			using (var _context = new SkincareProductSaleSystemContext())
			{
				return _context.Feedbacks
						   .Where(f => f.ProductId == productId) // Lấy danh sách phản hồi của sản phẩm
						   .ToList();
			}
		}
		public void CreateFeedback(Feedback feedback)
		{
			using (var _context = new SkincareProductSaleSystemContext())
			{
				_context.Feedbacks .Add(feedback);
				_context.SaveChanges();

			}
		}
	}
}
