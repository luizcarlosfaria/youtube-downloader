using AopAlliance.Intercept;
using Spring.Aop.Framework;
using Spring.Objects.Factory;
using Spring.Objects.Factory.Config;
using System;

namespace DevWeek.Architecture.Services;

	public class ServiceProxyFactory : IInitializingObject, IConfigurableFactoryObject, IMethodInterceptor
	{
		public bool IsSingleton { get { return true; } }

		public IObjectDefinition ProductTemplate { get; set; }

		private object Proxy { get; set; }

		public Type ObjectType { get; set; }

		protected object Service { get; set; }

		protected Type ServiceInterface { get; set; }

		public void AfterPropertiesSet()
		{
			this.Proxy = this.CreateProxy();
		}

		private object CreateProxy()
		{
			var proxy = new ProxyFactory();
			proxy.AddInterface(this.ServiceInterface);
			proxy.AddAdvice(this);
			proxy.Target = this.Service;
			return proxy.GetProxy();
		}

		public object GetObject()
		{
			return this.Proxy;
		}

		public virtual object Invoke(IMethodInvocation invocation)
		{
			return invocation.Proceed();
		}
	}