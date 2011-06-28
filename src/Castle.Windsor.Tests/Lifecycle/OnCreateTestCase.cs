﻿// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Tests.Lifecycle
{
	using Castle.MicroKernel.Registration;

	using NUnit.Framework;

	[TestFixture]
	public class OnCreateTestCase
	{

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			container = new DefaultKernel();
		}

		#endregion

		private IKernel container;

		[Test]
		public void CanModify_when_singleton()
		{
			container.Register(Component.For<IService>().ImplementedBy<MyService>()
			                   	.OnCreate((kernel, instance) => instance.Name+="a"));
			var service = container.Resolve<IService>();
			Assert.That(service.Name, Is.EqualTo("a"));
			service = container.Resolve<IService>();
			Assert.That(service.Name, Is.EqualTo("a"));
		}
		[Test]
		public void CanModify_when_singleton_multiple_ordered()
		{
			container.Register(Component.For<IService>().ImplementedBy<MyService>()
			                   	.OnCreate((kernel, instance) => instance.Name+="a",
			                   	          (kernel, instance) => instance.Name+="b"));
			var service = container.Resolve<IService>();
			Assert.That(service.Name, Is.EqualTo("ab"));
			service = container.Resolve<IService>();
			Assert.That(service.Name, Is.EqualTo("ab"));
		}

		[Test]
		public void CanModify_when_transient()
		{
			MyService2.staticname = "";
			container.Register(Component.For<IService2>().ImplementedBy<MyService2>()
			                   	.LifeStyle.Transient.OnCreate((kernel, instance) => instance.Name+="a"));
			var service = container.Resolve<IService2>();
			Assert.That(service.Name, Is.EqualTo("a"));
			service = container.Resolve<IService2>();
			Assert.That(service.Name, Is.EqualTo("aa"));
		}

		[Test]
		public void CanModify_when_transient_multiple_ordered()
		{
			MyService2.staticname = "";
			container.Register(Component.For<IService2>().ImplementedBy<MyService2>()
			                   	.LifeStyle.Transient.OnCreate((kernel, instance) => instance.Name+="a",
			                   	                              (kernel, instance) => instance.Name+="b"));
			var service = container.Resolve<IService2>();
			Assert.That(service.Name, Is.EqualTo("ab"));

			service = container.Resolve<IService2>();
			Assert.That(service.Name, Is.EqualTo("abab"));
		}

		[Test]
		public void CanModify_when_component_is_generic()
		{
			container.Register(Component.For(typeof(GenericService1<>))
					.OnCreate((kernel, instance) => ((IWithName)instance).Name = "a"));
			var service = container.Resolve<GenericService1<int>>();
			Assert.That(service.Name, Is.EqualTo("a"));
		}
	}

	public interface IService
	{
		string Name { get; set; }
	}

	public class MyService : IService
	{
		public MyService()
		{
			Name = "";
		}
		#region IService Members

		public string Name { get; set; }

		#endregion
	}


	public interface IService2
	{
		string Name { get; set; }
	}

	public class MyService2 : IService2
	{
		static MyService2()
		{
			staticname = "";
		}

		public static string staticname;

		#region IService2 Members

		public string Name
		{
			get { return staticname; }
			set { staticname = value; }
		}

		#endregion
	}

	public interface IWithName
	{
		string Name { get; set; }
	}

	public class GenericService1<T> : IWithName
	{
		public string Name { get; set; }
	}
}


