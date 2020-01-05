using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TestShop.Core;
using TestShop.Core.Models;

namespace TestShop.DataAccess.InMemory
{
    public class ProductRepository
    {
        ObjectCache cache = MemoryCache.Default;
        List<Product> products;

        public ProductRepository()
        {
            products = cache["products"] as List<Product>;

            if(products == null)
            {
                products = new List<Product>();
            }
        }

        public void Commit()
        {
            cache["products"] = products;
        }

        public void Insert(Product newProduct)
        {
            products.Add(newProduct);
        }

        public void Update(Product updatedProduct)
        {
            Product oldProduct = products.Find(product => product.Id == updatedProduct.Id);

            if(oldProduct != null)
            {
                oldProduct = updatedProduct;
            }
            else
            {
                throw new Exception("No product found!");
            }
        }

        public Product Find(string Id)
        {
            Product existingProduct = products.Find(product => product.Id == Id);

            if (existingProduct != null)
            {
                return existingProduct;
            }
            else
            {
                throw new Exception("No product found!");
            }
        }

        public IQueryable<Product> Collection()
        {
            return products.AsQueryable();
        }


        public void Delete(string Id)
        {
            Product existingProduct = products.Find(product => product.Id == Id);

            if (existingProduct != null)
            {
                products.Remove(existingProduct);
            }
            else
            {
                throw new Exception("No product found!");
            }
        }
    }
}
