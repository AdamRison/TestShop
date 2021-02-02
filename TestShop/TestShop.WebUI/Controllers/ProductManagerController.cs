using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestShop.Core;
using TestShop.Core.Contracts;
using TestShop.Core.Models;
using TestShop.Core.ViewModels;

namespace TestShop.WebUI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductManagerController : Controller
    {
        IRepository<Product> productRepository;
        IRepository<ProductCategory> productCategoryRepository;

        public ProductManagerController(IRepository<Product> productContext, IRepository<ProductCategory> productCategoryContext)
        {
            productRepository = productContext;
            productCategoryRepository = productCategoryContext;
        }
        // GET: ProductManager
        public ActionResult Index()
        {
            List<Product> products = productRepository.Collection().ToList();

            return View(products);
        }

        public ActionResult Create()
        {
            ProductManagerViewModel viewModel = new ProductManagerViewModel();

            viewModel.Product = new Product();
            viewModel.ProductCategories = productCategoryRepository.Collection();
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Create(Product product, HttpPostedFileBase file)
        {
            if(!ModelState.IsValid)
            {
                return View(product);
            }
            else
            {
                if(file != null)
                {
                    product.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//ProductImages//") + product.Image);
                }

                productRepository.Insert(product);
                productRepository.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string Id)
        {
            Product product = productRepository.Find(Id);

            if(product == null)
            {
                return HttpNotFound();
            }
            else
            {
                ProductManagerViewModel viewModel = new ProductManagerViewModel();
                viewModel.Product = product;
                viewModel.ProductCategories = productCategoryRepository.Collection();

                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult Edit(Product product, string Id, HttpPostedFileBase file)
        {
            Product productToEdit = productRepository.Find(Id);

            if (productToEdit == null)
            {
                return HttpNotFound();
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(product);
                }

                if (file != null)
                {
                    productToEdit.Image = product.Id + Path.GetExtension(file.FileName);
                    file.SaveAs(Server.MapPath("//Content//ProductImages//") + productToEdit.Image);
                }

                //productToEdit = new Product(product);

                productToEdit.Name = product.Name;
                productToEdit.Description = product.Description;
                productToEdit.Price = product.Price;
                productToEdit.Category = product.Category;

                productRepository.Commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult Delete(string Id)
        {
            Product productToDelete = productRepository.Find(Id);

            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(productToDelete);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            Product productToDelete = productRepository.Find(Id);

            if (productToDelete == null)
            {
                return HttpNotFound();
            }
            else
            {
                productRepository.Delete(Id);
                productRepository.Commit();
                return RedirectToAction("Index");
            }
        }
    }
}