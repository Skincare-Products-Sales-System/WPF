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
            try
            {
                using var context = new SkincareProductSaleSystemContext();
                if (context.Orders == null)
                {
                    throw new InvalidOperationException("Orders DbSet is not initialized in the context. Please ensure DbSet<Order> Orders is defined in SkincareProductSaleSystemContext and the database connection is valid.");
                }
                return context.Orders.ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to retrieve orders from the database: {ex.Message}", ex);
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
