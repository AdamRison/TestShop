using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestShop.Core.Models;
using TestShop.Core.ViewModels;

namespace TestShop.Core.Contracts
{
    public interface IOrderService
    {
        void CreateOrder(Order baseOrder, List<BasketItemViewModel> basketItems);
        List<Order> GetOrderList();
        Order GetOrder(string Id);
        void UpdateOrder(Order updatedOrder);
    }
}
