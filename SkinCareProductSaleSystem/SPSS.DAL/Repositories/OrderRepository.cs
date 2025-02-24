using Microsoft.EntityFrameworkCore;
using SPSS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.DAL.Repositories
{
	public class OrderRepository
	{
		public List<Order> GetAllOrdersAsync()
		{
			using (var context = new SkincareProductSaleSystemContext())
			{
				return  context.Orders.ToList();
			}
		}

		public List<Order> GetOrderByUserId(int userId)
		{
			using(var context = new SkincareProductSaleSystemContext())
			{
				return context.Orders.Where(x => x.UserId == userId).ToList();
			}
		}

		public  int CreateOrderAsync(Order order)
		{
			using (var context = new SkincareProductSaleSystemContext())
			{
				context.Orders.Add(order);
				context.SaveChanges();
				return order.OrderId; // Lấy OrderId sau khi lưu
			}
		}
	}
}
