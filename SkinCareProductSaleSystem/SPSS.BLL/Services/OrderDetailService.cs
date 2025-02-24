using SPSS.DAL;
using SPSS.DAL.Models;
using SPSS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.BLL.Services
{
	public class OrderDetailService
	{
		private OrderDetailRepository _repository = new();

		public void CreateOrderDetail(OrderDetail obj)
		{
			_repository.CreateOrderDetail(obj);
		}
		public List<OrderDetail> GetOrdersDetailsByOrderId(int id)
		{
			return _repository.GetOrderDetailByOrderId(id);
		}
	}
}
