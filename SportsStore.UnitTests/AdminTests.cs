﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;

namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Index_Contains_All_Products()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
                {
                    new Product{ProductId = 1, Name = "P1"},
                    new Product{ProductId = 2, Name = "P2"},
                    new Product{ProductId = 3, Name = "P3"}
                }.AsQueryable);

            AdminController target = new AdminController(mock.Object);

            Product[] result = ((IEnumerable<Product>)target.Index().ViewData.Model).ToArray();

            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }

        [TestMethod]
        public void Can_Edit_Product()
        {
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
                {
                    new Product{ProductId = 1, Name = "P1"},
                    new Product{ProductId = 2, Name = "P2"},
                    new Product{ProductId = 3, Name = "P3"}
                }.AsQueryable());

            AdminController target = new AdminController(mock.Object);

            var p1 = target.Edit(1).ViewData.Model as Product;
            var p2 = target.Edit(2).ViewData.Model as Product;
            var p3 = target.Edit(3).ViewData.Model as Product;

            target = new AdminController(mock.Object);
            var p4 = (Product)target.Edit(4).ViewData.Model;

            Assert.AreEqual(1, p1.ProductId);
            Assert.AreEqual(2, p2.ProductId);
            Assert.AreEqual(3, p3.ProductId);

            Assert.IsNull(p4);
        }
    }
}
