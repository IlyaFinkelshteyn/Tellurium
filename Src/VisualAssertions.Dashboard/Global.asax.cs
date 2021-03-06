﻿using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate;
using NHibernate.Context;
using Tellurium.MvcPages;
using Tellurium.VisualAssertions.Dashboard.Mvc;
using Tellurium.VisualAssertions.Infrastructure.Persistence;

namespace Tellurium.VisualAssertions.Dashboard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static WindsorContainer container;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            container = new WindsorContainer();
            container.Register(
               Classes.FromAssemblyContaining<WebAssemblyIdentity>()
                   .BasedOn<Controller>()
                   .LifestyleTransient(),
               Classes.FromAssemblyContaining<WebAssemblyIdentity>()
                    .Where(type => type.GetInterfaces().Any())
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(),
                Classes.FromAssemblyContaining<MvcPagesAssemblyIdentity>()
                    .Where(type => type.GetInterfaces().Any())
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(), 
                Classes.FromAssemblyContaining<VisualAssertionsAssemblyIdentity>()
                    .Where(type => type.GetInterfaces().Any())
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton(),
                Component.For<ISessionFactory>()
                    .UsingFactoryMethod(kernel=> PersistanceEngine.CreateSessionFactory<WebSessionContext>())
                    .LifestyleSingleton()
                    );

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container));
        }

        protected void Application_End()
        {
            container.Dispose();
        }
    }
}
