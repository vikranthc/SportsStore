using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository _repository;
        public int PageSize = 4;

        public ProductController(IProductRepository productRepository)
        {
            _repository = productRepository;
        }

        public ViewResult List(int page = 1)
        {
            return View(_repository.Products
                                   .OrderBy(p=>p.ProductId)
                                   .Skip((page-1)*PageSize)
                                   .Take(PageSize));
        }
    }
}
