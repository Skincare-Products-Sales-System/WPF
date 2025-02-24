using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPSS.DAL.Models
{
	public class CartItem : INotifyPropertyChanged
	{
		private int _quantity;
		public Product Product { get; set; }

		public int Quantity
		{
			get => _quantity;
			set
			{
				if (_quantity != value)
				{
					_quantity = value;
					OnPropertyChanged(nameof(Quantity));
					OnPropertyChanged(nameof(TotalPrice));
				}
			}
		}

		public double? TotalPrice => Product.Price * Quantity;

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
