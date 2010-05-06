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

namespace Castle.Facilities.EventWiring.Tests
{
	using System;

	using Castle.Facilities.EventWiring.Tests.Model;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;

	using NUnit.Framework;

	[TestFixture]
	public class FluentEventWiringTestCase
	{
		private WindsorContainer container;

		[SetUp]
		public void SetUpTests()
		{
			container = new WindsorContainer();
			container.AddFacility<EventWiringFacility>();
		}

		[Test]
		public void Can_fluently_wire_events()
		{
			container.Register(Component.For<SimplePublisher>().PublishEvent("Event", "myUniqueEventId"),
							   Component.For<SimpleListener>().SubscribeEvent("myUniqueEventId", "OnPublish"));
			var publisher = container.Resolve<SimplePublisher>();
			var listener = container.Resolve<SimpleListener>();

			Assert.IsFalse(listener.Listened);

			publisher.Trigger();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);
		}

		[Test]
		public void Can_plug_subscriber_to_already_resolved_publisher()
		{
			container.Register(Component.For<SimplePublisher>().PublishEvent("Event", "myUniqueEventId"),
							   Component.For<SimpleListener>().SubscribeEvent("myUniqueEventId", "OnPublish"));
			var publisher = container.Resolve<SimplePublisher>();

			publisher.Trigger();

			var listener = container.Resolve<SimpleListener>();

			Assert.IsFalse(listener.Listened);

			publisher.Trigger();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);
		}

		[Test]
		public void Can_plug_publisher_to_already_resolved_subscriber()
		{
			container.Register(Component.For<SimplePublisher>().PublishEvent("Event", "myUniqueEventId"),
							   Component.For<SimpleListener>().SubscribeEvent("myUniqueEventId", "OnPublish"));
			var listener = container.Resolve<SimpleListener>();

			Assert.IsFalse(listener.Listened);

			var publisher = container.Resolve<SimplePublisher>();
			publisher.Trigger();

			Assert.IsTrue(listener.Listened);
			Assert.AreSame(publisher, listener.Sender);
		}

		[Test]
		public void Resolving_publisher_does_not_trigger_resolution_of_subscriber()
		{
			SimpleListener.InstancesCreated = 0;
			container.Register(Component.For<SimplePublisher>().PublishEvent("Event", "myUniqueEventId"),
							   Component.For<SimpleListener>().SubscribeEvent("myUniqueEventId", "OnPublish"));

			var publisher = container.Resolve<SimplePublisher>();
			publisher.Trigger();

			Assert.AreEqual(0, SimpleListener.InstancesCreated);
		}

		[Test]
		public void Can_publish_to_multiple_subscribers()
		{
			container.Register(Component.For<SimplePublisher>().PublishEvent("Event", "myUniqueEventId"),
			                   Component.For<SimpleListener>().Named("first").SubscribeEvent("myUniqueEventId", "OnPublish"),
			                   Component.For<SimpleListener>().Named("second").SubscribeEvent("myUniqueEventId", "OnPublish"));

			var first = container.Resolve<SimpleListener>("first");
			var publisher = container.Resolve<SimplePublisher>();
			var second = container.Resolve<SimpleListener>("second");
			publisher.Trigger();

			Assert.IsTrue(first.Listened);
			Assert.IsTrue(second.Listened);
			Assert.AreSame(publisher, first.Sender);
			Assert.AreSame(publisher, second.Sender);
		}

		[Test]
		public void Can_publisher_be_garbage_collected()
		{
			container.Register(Component.For<SimplePublisher>().LifeStyle.Transient.PublishEvent("Event", "myUniqueEventId"),
							   Component.For<SimpleListener>().LifeStyle.Transient.SubscribeEvent("myUniqueEventId", "OnPublish"));

			var listener = container.Resolve<SimpleListener>();
			var publisher = container.Resolve<SimplePublisher>();
			publisher.Trigger();

			Assert.IsTrue(listener.Listened);
			listener.Sender = null; // to let GC do its job

			var publisherReference = new WeakReference(publisher);
			container.Release(publisher);
			publisher = null;
			GC.Collect();

			Assert.IsFalse(publisherReference.IsAlive);
		}

		[Test]
		public void Can_listener_be_garbage_collected()
		{
			container.Register(Component.For<SimplePublisher>().LifeStyle.Transient.PublishEvent("Event", "myUniqueEventId"),
							   Component.For<SimpleListener>().LifeStyle.Transient.SubscribeEvent("myUniqueEventId", "OnPublish"));

			var listener = container.Resolve<SimpleListener>();
			var publisher = container.Resolve<SimplePublisher>();
			publisher.Trigger();

			Assert.IsTrue(listener.Listened);

			var listenerReference = new WeakReference(listener);
			container.Release(listener);
			listener = null;
			GC.Collect();

			Assert.IsFalse(listenerReference.IsAlive);
		}

		[Test,Ignore("This is broken - we're overriding the registration the 2nd time around")]
		public void Can_subscribe_to_multiple_publishers()
		{
			container.Register(Component.For<SimplePublisher>().Named("first").PublishEvent("Event", "first"),
			                   Component.For<SimplePublisher>().Named("second").PublishEvent("Event", "second"),
			                   Component.For<SimpleListener>()
			                   	.SubscribeEvent("first", "OnPublish")
			                   	.SubscribeEvent("second", "OnPublish"));

			var first = container.Resolve<SimplePublisher>("first");
			var listener = container.Resolve<SimpleListener>();
			var second = container.Resolve<SimplePublisher>("second");

			first.Trigger();
			Assert.AreEqual(1, listener.Count);
			Assert.AreSame(first, listener.Sender);

			second.Trigger();
			Assert.AreEqual(2, listener.Count);
			Assert.AreSame(second, listener.Sender);
		}
	}
}