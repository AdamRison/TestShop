using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TestShop.Core.Contracts;
using TestShop.Core.Models;

namespace TestShop.Services
{
    public class BasketService
    {
        IRepository<Product> productRepository;
        IRepository<Basket> basketRepository;

        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> productContext, IRepository<Basket> basketContext)
        {
            this.productRepository = productContext;
            this.basketRepository = basketContext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            HttpCookie chocolateChip = httpContext.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();

            if (chocolateChip != null)
            {
                string basketId = chocolateChip.Value;
                if (!string.IsNullOrEmpty(basketId))
                {
                    basket = basketRepository.Find(basketId);
                }
                else if (createIfNull)
                {

                    basket = CreateNewBasket(httpContext);
                }
            }
            else if (createIfNull)
            {
                basket = CreateNewBasket(httpContext);
            }

            return basket;
        }

        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketRepository.Insert(basket);
            basketRepository.Commit();

            HttpCookie oatmealRaisin = new HttpCookie(BasketSessionName);
            oatmealRaisin.Value = basket.Id;
            oatmealRaisin.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(oatmealRaisin);

            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

            if(item == null)
            {
                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };

                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quantity = item.Quantity + 1;
            }

            basketRepository.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if(item != null)
            {
                basket.BasketItems.Remove(item);
                basketRepository.Commit();
            }
        }
    }
}
