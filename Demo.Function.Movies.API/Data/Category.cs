using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.Function.Movies.API.Data
{
    public class Movie
    {
        public int id { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public string CategoryId { get; set; }
    }

    public class Order
    {
        public int id { get; set; }

        public DateTime OrderDate { get; set; }

        public IEnumerable<OrderDetail> Details { get; set; }
    }

    public class OrderDetail
    {
        public OrderDetail()
        { 
        }
        //public OrderDetail(string orderDetailId, string productId, string unitPrice, string quantity, string email)
        //{
        //    OrderDetailId = orderDetailId ?? throw new ArgumentNullException(nameof(orderDetailId));
        //    ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        //    UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        //    Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        //    Email = email ?? throw new ArgumentNullException(nameof(email));
        //}

        public string OrderDetailId { get; set; }

        public long ProductId { get; set; }

        public decimal UnitPrice { get; set; }

        public long Quantity { get; set; }

        public string Email { get; set; }

        public DateTime OrderDate { get; set; }

        public string CategoryId { get; set; }

        public string AggregationId { get; set; }
    }
}
