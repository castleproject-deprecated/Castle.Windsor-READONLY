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
	}
}