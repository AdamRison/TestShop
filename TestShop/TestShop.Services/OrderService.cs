using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestShop.Core.Contracts;
using TestShop.Core.Models;
using TestShop.Core.ViewModels;

namespace TestShop.Services
{
    public class OrderService : IOrderService
    {
        IRepository<Order> orderRepo;
        public OrderService(IRepository<Order> orderContext)
        {
            this.orderRepo = orderContext;
        }

        public void CreateOrder(Order baseOrder, List<BasketItemViewModel> basketItems)
        {
            foreach (var item in basketItems)
            {
                baseOrder.OrderItems.Add(new OrderItem()
                {
                    ProductId = item.Id,
                    Price = item.Price,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Image = item.Image
                });
            }
            orderRepo.Insert(baseOrder);
            orderRepo.Commit();

        }
    }
}
