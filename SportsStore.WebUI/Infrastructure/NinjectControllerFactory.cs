﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;
using Ninject;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel _ninjectKernel;

        public NinjectControllerFactory()
        {
            _ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)_ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            var mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new List<Product>
                {
                    new Product {Name = "Football", Price = 25},
                    new Product {Name = "Surf Board", Price = 179},
                    new Product {Name = "Running Shoes", Price = 95}
                }.AsQueryable());

            _ninjectKernel.Bind<IProductRepository>().ToConstant(mock.Object);
        }
    }
}