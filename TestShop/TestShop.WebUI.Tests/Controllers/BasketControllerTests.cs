using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestShop.Core.Contracts;
using TestShop.Core.Models;
using TestShop.Core.ViewModels;
using TestShop.Services;
using TestShop.WebUI.Controllers;
using TestShop.WebUI.Tests.Mocks;

namespace TestShop.WebUI.Tests.Controllers
{
    [TestClass]
    public class BasketControllerTests
    {
        [TestMethod]
        public void CanAddBasketItem()
        {
            //Setup
            IRepository<Basket> basketRepo = new MockContext<Basket>();
            IRepository<Product> productRepo = new MockContext<Product>();
            IRepository<Order> orderRepo = new MockContext<Order>();

            var httpContext = new MockHttpContext();

            IBasketService basketService = new BasketService(productRepo, basketRepo);
            IOrderService orderService = new OrderService(orderRepo);
            var controller = new BasketController(basketService, orderService);
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            //Act
            //basketService.AddToBasket(httpContext, "1");

            controller.AddToBasket("1");

            Basket basket = basketRepo.Collection().FirstOrDefault();

            //Assert
            Assert.IsNotNull(basket);
            Assert.AreEqual(1, basket.BasketItems.Count);
            Assert.AreEqual("1", basket.BasketItems.ToList().FirstOrDefault().ProductId);
        }

        [TestMethod]
        public void CanGetSummaryViewModel()
        {
            //Setup
            IRepository<Basket> basketRepo = new MockContext<Basket>();
            IRepository<Product> productRepo = new MockContext<Product>();
            IRepository<Order> orderRepo = new MockContext<Order>();

            productRepo.Insert(new Product() { Id = "1", Price = 10.00m });
            productRepo.Insert(new Product() { Id = "2", Price = 5.00m });

            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2 });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1 });
            basketRepo.Insert(basket);

            IBasketService basketService = new BasketService(productRepo, basketRepo);
            IOrderService orderService = new OrderService(orderRepo);

            var controller = new BasketController(basketService, orderService);
            var httpContext = new MockHttpContext();
            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket") { Value = basket.Id });
            controller.ControllerContext = new System.Web.Mvc.ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            var result = controller.BasketSummary() as PartialViewResult;
            var basketSummary = (BasketSummaryViewModel)result.ViewData.Model;

            Assert.AreEqual(3, basketSummary.BasketCount);
            Assert.AreEqual(25.00m, basketSummary.BasketTotal);

        }

        [TestMethod]
        public void CanCheckOutAndCreateOrder()
        {
            IRepository<Product> productRepo = new MockContext<Product>();
            productRepo.Insert(new Product() { Id = "1", Price = 10.00m });
            productRepo.Insert(new Product() { Id = "2", Price = 5.00m });

            IRepository<Basket> basketRepo = new MockContext<Basket>();
            Basket basket = new Basket();
            basket.BasketItems.Add(new BasketItem() { ProductId = "1", Quantity = 2, BasketId = basket.Id });
            basket.BasketItems.Add(new BasketItem() { ProductId = "2", Quantity = 1, BasketId = basket.Id });

            basketRepo.Insert(basket);

            IBasketService basketService = new BasketService(productRepo, basketRepo);

            IRepository<Order> orderRepo = new MockContext<Order>();
            IOrderService orderService = new OrderService(orderRepo);

            var controller = new BasketController(basketService, orderService);
            var httpContext = new MockHttpContext();
            httpContext.Request.Cookies.Add(new System.Web.HttpCookie("eCommerceBasket")
            {
                Value = basket.Id
            });

            controller.ControllerContext = new ControllerContext(httpContext, new System.Web.Routing.RouteData(), controller);

            //Act
            Order order = new Order();
            controller.CheckOut(order);

            //Assert
            Assert.AreEqual(2, order.OrderItems.Count);
            Assert.AreEqual(0, basket.BasketItems.Count);

            Order orderInRepo = orderRepo.Find(order.Id);
            Assert.AreEqual(2, orderInRepo.OrderItems.Count);
        }

        private object BasketController(IBasketService basketService, IOrderService orderService)
        {
            throw new NotImplementedException();
        }
    }
}
