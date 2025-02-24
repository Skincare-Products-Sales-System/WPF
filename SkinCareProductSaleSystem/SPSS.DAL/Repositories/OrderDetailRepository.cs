using Microsoft.EntityFrameworkCore;
using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.DAL.Repositories
{
	public class OrderDetailRepository
	{
		public void CreateOrderDetail(OrderDetail obj)
		{
			using (var _context = new SkincareProductSaleSystemContext())
			{
				_context.OrderDetails.Add(obj);
				_context.SaveChanges();
			}
		}

		public List<OrderDetail> GetOrderDetailByOrderId(int id)
		{
			using (var _context = new SkincareProductSaleSystemContext())
			{
				return _context.OrderDetails
							   .Include(x => x.Product) // Use the Include method to include the Product
							   .Where(x => x.OrderId == id)
							   .ToList();
			}
		}
	}
}
