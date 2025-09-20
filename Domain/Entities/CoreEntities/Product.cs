using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.CoreEntities
{
	public class Product :BaseEntity<int>
	{
		public Category Category { get; set; }
		public string PCode { get; set; }

		public string Name { get; set; }
		public string ImageUrl { get; set; }
		public double Price { get; set; }
		public int Quantity { get; set; }
		public double DiscountRate { get; set; }

	}
}
