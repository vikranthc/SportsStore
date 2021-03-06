﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;

namespace SportsStore.UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            // Arrange
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
                {
                    new Product {ProductId = 1, Name = "P1"},
                    new Product {ProductId = 2, Name = "P2"},
                    new Product {ProductId = 3, Name = "P3"},
                    new Product {ProductId = 4, Name = "P4"},
                    new Product {ProductId = 5, Name = "P5"}
                }.AsQueryable());

            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            var result = (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert
            var prodArray = result.Products.ToArray();
            Assert.IsTrue(prodArray.Length == 2);
            Assert.AreEqual(prodArray[0].Name, "P4");
            Assert.AreEqual(prodArray[1].Name, "P5");
        }

        [TestMethod]
        public void Can_Generate_Page_Links()
        {
            // Arrange - Define an HTML helper - we need to do this
            // in order to apply the extension method
            HtmlHelper myHelper = null;

            // Arrange - create PagingInfo data
            var pagingInfo = new PagingInfo
                {
                    CurrentPage = 2,
                    TotalItems = 28,
                    ItemsPerPage = 10
                };

            // Arrange - set up the delegate using a lambda expression
            Func<int, string> pageUrlDelegate = i => "Page" + i;

            // Act
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

            // Assert
            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a>" + @"<a class=""selected"" href=""Page2"">2</a>" + @"<a href=""Page3"">3</a>");
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
                {
                    new Product {ProductId = 1, Name = "P1"},
                    new Product {ProductId = 2, Name = "P2"},
                    new Product {ProductId = 3, Name = "P3"},
                    new Product {ProductId = 4, Name = "P4"},
                    new Product {ProductId = 5, Name = "P5"}
                }.AsQueryable());

            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            var result = (ProductsListViewModel)controller.List(null, 2).Model;

            // Assert
            PagingInfo pagingInfo = result.PagingInfo;
            Assert.AreEqual(pagingInfo.CurrentPage, 2);
            Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
            Assert.AreEqual(pagingInfo.TotalItems, 5);
            Assert.AreEqual(pagingInfo.Totalpages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            // Arrange
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
                {
                    new Product {ProductId = 1, Name = "P1", Category = "Cat1"},
                    new Product {ProductId = 2, Name = "P2", Category = "Cat2"},
                    new Product {ProductId = 3, Name = "P3", Category = "Cat1"},
                    new Product {ProductId = 4, Name = "P4", Category = "Cat2"},
                    new Product {ProductId = 5, Name = "P5", Category = "Cat3"}
                }.AsQueryable());

            var controller = new ProductController(mock.Object);
            controller.PageSize = 3;

            // Act
            var result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

            // Assert
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Arrange
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[]
                {
                    new Product {ProductId = 1, Name = "P1", Category = "Apples"}, 
                    new Product {ProductId = 2, Name = "P2", Category = "Apples"}, 
                    new Product {ProductId = 3, Name = "P3", Category = "Plums"}, 
                    new Product {ProductId = 4, Name = "P4", Category = "Oranges"}, 
                }.AsQueryable());

            var target = new NavController(mock.Object);

            // Act 
            var results = ((IEnumerable<string>)target.Menu().Model).ToArray();

            // Assert
            Assert.AreEqual(results.Length, 3);
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }

        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // Arrange
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[] { 
                    new Product {ProductId = 1, Name = "P1", Category = "Apples"}, 
                    new Product {ProductId = 4, Name = "P2", Category = "Oranges"}, 
                    }.AsQueryable());

            var target = new NavController(mock.Object);
            string categoryToSelect = "Apples";

            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Product_Count()
        {
            // Arrange 
            // - create the mock repository 
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new[] { 
                    new Product {ProductId = 1, Name = "P1", Category = "Cat1"}, 
                    new Product {ProductId = 2, Name = "P2", Category = "Cat2"}, 
                    new Product {ProductId = 3, Name = "P3", Category = "Cat1"}, 
                    new Product {ProductId = 4, Name = "P4", Category = "Cat2"}, 
                    new Product {ProductId = 5, Name = "P5", Category = "Cat3"} 
                    }.AsQueryable());

            var target = new ProductController(mock.Object);
            target.PageSize = 3;

            int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

            // Assert 
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart()
        {
            // Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();
            CartController target = new CartController(null, mock.Object);

            // Act
            ViewResult result = target.Checkout(cart, shippingDetails);

            // Check that the order hasn't been passed on to the processor
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            // Check that the method is returning the default view
            Assert.AreEqual("", result.ViewName);
            // Check that we are passing an invaild model to the view
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);

        }

        [TestMethod]
        public void Cannot_Chekout_Invalid_ShippingDetails()
        {
            // Arrange
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            CartController target = new CartController(null, mock.Object);
            target.ModelState.AddModelError("error", "error");
            // Act
            ViewResult result = target.Checkout(cart, new ShippingDetails());

            // Assert
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Never());
            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order()
        {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            CartController target = new CartController(null,mock.Object);

            ViewResult result = target.Checkout(cart, new ShippingDetails());

            mock.Verify(m=>m.ProcessOrder(It.IsAny<Cart>(), It.IsAny<ShippingDetails>()), Times.Once());
            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
