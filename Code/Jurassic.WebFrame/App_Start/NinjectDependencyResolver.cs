using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using Jurassic.CommonModels;
using System.Web.Mvc;

namespace Jurassic.WebFrame
{
    /// <summary>
    /// 使用Ninject进行服务的注入
    /// </summary>
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private Ninject.IKernel kernel;
        public NinjectDependencyResolver(Action<IKernel> addBindings)
        {
            this.kernel = SiteManager.Kernel;
            addBindings(kernel);
        }

        public virtual object GetService(Type serviceType)
        {
            return this.kernel.TryGet(serviceType);
        }

        public virtual IEnumerable<object> GetServices(Type serviceType)
        {
            return this.kernel.GetAll(serviceType);
        }
    }
}