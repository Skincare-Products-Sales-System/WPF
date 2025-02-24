using SPSS.DAL.Models;
using SPSS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.BLL.Services
{
	public class OrderService
	{
		private  OrderRepository _repository = new OrderRepository();

		public int CreateOrderAsync(Order order)
		{
			 return _repository.CreateOrderAsync(order);
		}

		public  List<Order> GetAllOrdersAsync()
		{
			return  _repository.GetAllOrdersAsync();
		}

		public List<Order> GetOrdersByUserId(int id)
		{
			return _repository.GetOrderByUserId(id);
		}
	}
}
