using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Function.Movies.API.Data
{
    public class OrderDetail
    {
        public OrderDetail(string orderDetailId, string productId, string unitPrice, string quantity, string email)
        {
            OrderDetailId = orderDetailId ?? throw new ArgumentNullException(nameof(orderDetailId));
            ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
            UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
            Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }

        public string OrderDetailId { get; set; }

        public string ProductId { get; set; }

        public string UnitPrice { get; set; }

        public string Quantity { get; set; }

        public string Email { get; set; }

    }
    public class Category
    {
        public int id { get; set; }

        public string CategoryName { get; set; }

        public string Description { get; set; }
    }
}
