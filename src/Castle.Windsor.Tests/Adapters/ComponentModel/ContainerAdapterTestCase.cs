// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Windsor.Tests.Adapters.ComponentModel
{
#if (!SILVERLIGHT)
	using System;
	using System.ComponentModel;
	using System.ComponentModel.Design;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Context;
	using Castle.Windsor.Adapters.ComponentModel;
	using Castle.Windsor.Tests.Components;

	using NUnit.Framework;

	[TestFixture]
	public class ContainerAdapterTestCase
	{
		private int calledCount;
		private IContainerAdapter container;
		private bool disposed;

		[Test]
		[ExpectedException(typeof(ArgumentException),
			ExpectedMessage = "There is a component already registered for the given key myComponent")]
		public void AddDuplicateComponent()
		{
			container.Add(new Component(), "myComponent");
			container.Add(new Component(), "myComponent");
		}

		[Test]
		[ExpectedException(typeof(ArgumentException),
			ExpectedMessage = "A service for type 'Castle.Windsor.Tests.Components.ICalcService' already exists")]
		public void AddExistingServiceInstance()
		{
			container.AddService(typeof(ICalcService), new CalculatorService());
			container.AddService(typeof(ICalcService), new CalculatorService());
		}

		[Test]
		[ExpectedException(typeof(ArgumentException),
			ExpectedMessage = "A service for type 'Castle.Windsor.IWindsorContainer' already exists")]
		public void AddIntrinsicService()
		{
			container.AddService(typeof(IWindsorContainer), new WindsorContainer());
		}

		[Test]
		public void AddNamedComponent()
		{
			IComponent component = new Component();

			container.Add(component, "myComponent");
			var site = component.Site;
			Assert.IsNotNull(site);
			Assert.AreSame(container, site.Container);
			Assert.AreEqual(1, container.Components.Count);
			Assert.IsNotNull(container.Components["myComponent"]);
			Assert.IsTrue(container.Container.Kernel.HasComponent("myComponent"));
			Assert.IsNotNull(site.GetService(typeof(IHandler)));

			var component2 = container.Components["myComponent"];
			Assert.AreSame(component2, component);
			Assert.AreSame(component2, container.Container.Resolve<object>("myComponent"));
			Assert.AreSame(container, component2.Site.Container);
		}

		[Test]
		public void AddPromotedServiceCreatorCallback()
		{
			var child = new ContainerAdapter();
			container.Add(child);

			ServiceCreatorCallback callback = CreateCalculatorService;

			child.AddService(typeof(ICalcService), callback, true);

			var service = (ICalcService)child.GetService(typeof(ICalcService));
			Assert.IsNotNull(service);

			var promotedService = (ICalcService)container.GetService(typeof(ICalcService));
			Assert.IsNotNull(service);

			Assert.AreSame(service, promotedService);

			container.Remove(child);
			Assert.IsNull(child.GetService(typeof(ICalcService)));
			Assert.AreSame(container.GetService(typeof(ICalcService)), service);
		}

		[Test]
		public void AddPromotedServiceInstance()
		{
			var child = new ContainerAdapter();
			container.Add(child);

			ICalcService service = new CalculatorService();

			child.AddService(typeof(ICalcService), service, true);

			Assert.AreSame(child.GetService(typeof(ICalcService)), service);
			Assert.AreSame(container.GetService(typeof(ICalcService)), service);

			container.Remove(child);
			Assert.IsNull(child.GetService(typeof(ICalcService)));
			Assert.AreSame(container.GetService(typeof(ICalcService)), service);
		}

		[Test]
		public void AddServiceCreatorCallback()
		{
			ServiceCreatorCallback callback = CreateCalculatorService;

			container.AddService(typeof(ICalcService), callback);

			var service = (ICalcService)container.GetService(typeof(ICalcService));

			Assert.IsNotNull(service);
			Assert.AreSame(service, container.Container.Resolve<ICalcService>());

			service = (ICalcService)container.GetService(typeof(ICalcService));
			Assert.AreEqual(1, calledCount);
		}

		[Test]
		public void AddServiceInstance()
		{
			ICalcService service = new CalculatorService();

			container.AddService(typeof(ICalcService), service);

			Assert.AreSame(container.GetService(typeof(ICalcService)), service);
			Assert.AreSame(container.Container.Resolve<ICalcService>(), service);
		}

		[Test]
		public void AddUnamedComponent()
		{
			IComponent component = new Component();

			container.Add(component);
			var site = component.Site;
			Assert.IsNotNull(site);
			Assert.AreSame(container, site.Container);
			Assert.AreEqual(1, container.Components.Count);
			Assert.IsNotNull(site.GetService(typeof(IHandler)));

			var component2 = container.Components[0];
			Assert.AreSame(component, component2);
			Assert.AreSame(container, component2.Site.Container);
		}

		public void ChainContainers()
		{
			ICalcService service = new CalculatorService();
			container.AddService(typeof(ICalcService), service);

			IContainerAdapter adapter = new ContainerAdapter(container);

			Assert.AreSame(service, container.GetService(typeof(ICalcService)));
		}

		[Test]
		public void ComponentLifecyle()
		{
			var component = new TestComponent();
			Assert.IsFalse(component.IsSited);
			Assert.IsFalse(component.IsDisposed);

			container.Add(component);
			Assert.IsTrue(component.IsSited);

			container.Dispose();
			Assert.IsTrue(component.IsDisposed);
		}

		[Test]
		public void ContainerLifecyle()
		{
			container.Disposed += Container_Disposed;
			Assert.IsFalse(disposed);

			container.Dispose();
			Assert.IsTrue(disposed);
		}

		[TearDown]
		public void Dispose()
		{
			container.Dispose();
		}

		[Test]
		public void DisposeWindsorContainer()
		{
			container.Disposed += Container_Disposed;
			Assert.IsFalse(disposed);

			container.Container.Dispose();
			Assert.IsTrue(disposed);
		}

		[Test]
		public void GetComponentHandlers()
		{
			IComponent component = new Component();

			container.Add(component);

			var handlers = container.Container.Kernel.GetHandlers(typeof(IComponent));
			Assert.AreEqual(1, handlers.Length);
			Assert.AreEqual(handlers[0].Resolve(CreationContext.Empty), component);
		}

		[Test]
		public void GetExistingServiceFromKernel()
		{
			var adapter = new ContainerAdapter(new WindsorContainer()
			                                   	.Register(Castle.MicroKernel.Registration.Component.For<ICalcService>()
			                                   	          	.ImplementedBy<CalculatorService>()));

			var service = (ICalcService)adapter.GetService(typeof(ICalcService));

			Assert.IsNotNull(service);
		}

		[Test]
		public void GetIntrinsicServices()
		{
			Assert.IsNotNull(container.GetService(typeof(IContainer)));
			Assert.IsNotNull(container.GetService(typeof(IServiceContainer)));
			Assert.IsNotNull(container.GetService(typeof(IWindsorContainer)));
			Assert.IsNotNull(container.GetService(typeof(IKernel)));
		}

		[SetUp]
		public void Init()
		{
			calledCount = 0;
			disposed = false;
			container = new ContainerAdapter();
		}

		[Test]
		public void RemoveComponentFromWindsor()
		{
			IComponent component = new Component();

			container.Add(component, "myComponent");
			var site = component.Site as IContainerAdapterSite;

			container.Container.Kernel.RemoveComponent(site.EffectiveName);
			Assert.AreEqual(0, container.Components.Count);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot remove an instrinsic service")]
		public void RemoveInstrinsicService()
		{
			container.RemoveService(typeof(IWindsorContainer));
		}

		[Test]
		public void RemoveNamedComponent()
		{
			IComponent component = new Component();

			container.Add(component, "myComponent");
			var site = component.Site;
			Assert.IsNotNull(site);

			container.Remove(component);
			Assert.IsNull(component.Site);
			Assert.AreEqual(0, container.Components.Count);
			Assert.IsFalse(container.Container.Kernel.HasComponent("myComponent"));
		}

		[Test]
		public void RemovePromotedServiceInstance()
		{
			var child = new ContainerAdapter();
			container.Add(child);

			ICalcService service = new CalculatorService();

			child.AddService(typeof(ICalcService), service, true);
			Assert.IsNotNull(child.GetService(typeof(ICalcService)));

			child.RemoveService(typeof(ICalcService), true);
			Assert.IsNull(child.GetService(typeof(ICalcService)));
			Assert.IsNull(container.GetService(typeof(ICalcService)));
		}

		[Test]
		public void RemoveServiceInstance()
		{
			ICalcService service = new CalculatorService();

			container.AddService(typeof(ICalcService), service);
			container.RemoveService(typeof(ICalcService));
			Assert.IsNull(container.GetService(typeof(ICalcService)));
			Assert.IsFalse(container.Container.Kernel.HasComponent(typeof(ICalcService)));
		}

		[Test]
		public void RemoveUnnamedComponent()
		{
			IComponent component = new Component();

			container.Add(component);
			var site = component.Site as IContainerAdapterSite;
			Assert.IsNotNull(site);

			container.Remove(component);
			Assert.IsNull(component.Site);
			Assert.AreEqual(0, container.Components.Count);
			Assert.IsFalse(container.Container.Kernel.HasComponent(site.EffectiveName));
		}

		private void Container_Disposed(object source, EventArgs args)
		{
			disposed = true;
		}

		private object CreateCalculatorService(IServiceContainer container, Type serviceType)
		{
			++calledCount;
			return new CalculatorService();
		}
	}
#endif
}