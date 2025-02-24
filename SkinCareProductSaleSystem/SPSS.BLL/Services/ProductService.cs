using SPSS.DAL.Models;
using SPSS.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.BLL.Services
{
	public class ProductService
	{
		private ProductRepository _productRepository = new();
		public List<Product> GetAllProducts()
		{
			return _productRepository.GetAllProducts();
		}

		public void UpdateProduct(Product product)
		{
			_productRepository.UpdateProduct(product);
		}
	}
}
