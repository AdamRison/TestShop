using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using TestShop.Core.Models;

namespace TestShop.DataAccess.InMemory
{
    public class ProductCategoryRepository
    {
        ObjectCache cache = MemoryCache.Default;
        List<ProductCategory> productCategories;

        public ProductCategoryRepository()
        {
            productCategories = cache["productCategories"] as List<ProductCategory>;

            if (productCategories == null)
            {
                productCategories = new List<ProductCategory>();
            }
        }

        public void Commit()
        {
            cache["productCategories"] = productCategories;
        }

        public void Insert(ProductCategory newProductCategory)
        {
            productCategories.Add(newProductCategory);
        }

        public void Update(ProductCategory updatedProductCategory)
        {
            ProductCategory oldProductCategory = productCategories.Find(productCategory => productCategory.Id == updatedProductCategory.Id);

            if (oldProductCategory != null)
            {
                oldProductCategory = updatedProductCategory;
            }
            else
            {
                throw new Exception("No product category found!");
            }
        }

        public ProductCategory Find(string Id)
        {
            ProductCategory existingProductCategory = productCategories.Find(productCategory => productCategory.Id == Id);

            if (existingProductCategory != null)
            {
                return existingProductCategory;
            }
            else
            {
                throw new Exception("No product category found!");
            }
        }

        public IQueryable<ProductCategory> Collection()
        {
            return productCategories.AsQueryable();
        }


        public void Delete(string Id)
        {
            ProductCategory existingProductCategory = productCategories.Find(productCategory => productCategory.Id == Id);

            if (existingProductCategory != null)
            {
                productCategories.Remove(existingProductCategory);
            }
            else
            {
                throw new Exception("No product category found!");
            }
        }
    }

}
