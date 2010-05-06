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

namespace Castle.Facilities.EventWiring
{
	using System;

	using Castle.MicroKernel.Registration;

	public static class EventWiringRegistrationExtensions
	{
		/// <summary>
		/// Wires up the component with <see cref="EventWiringFacility"/> as event publisher.
		/// </summary>
		/// <typeparam name="TPublisher">Publisher type</typeparam>
		/// <param name="publisher">Event publisher registration</param>
		/// <param name="event">Name of the <c>event</c> on the class.</param>
		/// <param name="id">Unique identifier of the event for the facility. You will use this id to connect subscribers with the event.</param>
		/// <returns></returns>
		public static ComponentRegistration<TPublisher> PublishEvent<TPublisher>(this ComponentRegistration<TPublisher> publisher, string @event, string id)
		{
			if (publisher == null)
			{
				throw new ArgumentNullException("publisher");
			}

			if (@event == null)
			{
				throw new ArgumentNullException("event");
			}

			if (id == null)
			{
				throw new ArgumentNullException("id");
			}

			return publisher.Configuration(
				Child.ForName("publishedEvents").Eq(
					Child.ForName("event").Eq(
						Attrib.ForName("id").Eq(id),
						Attrib.ForName("name").Eq(@event))));
		}

		/// <summary>
		/// Wires up the component with <see cref="EventWiringFacility"/> as event publisher.
		/// </summary>
		/// <typeparam name="TSubscriber">Publisher type</typeparam>
		/// <param name="publisher">Event subscriber registration</param>
		/// <param name="id">Unique identifier of the event you want to subscribe to. That's the id you specified on the publisher.</param>
		/// <param name="eventHandler">Name of the method on the class that should handle the event. Notice that it's signature must be compatibile with the event.</param>
		/// <returns></returns>
		public static ComponentRegistration<TSubscriber> SubscribeEvent<TSubscriber>(this ComponentRegistration<TSubscriber> publisher, string id, string eventHandler)
		{
			if (publisher == null)
			{
				throw new ArgumentNullException("publisher");
			}

			if (eventHandler == null)
			{
				throw new ArgumentNullException("eventHandler");
			}

			if (id == null)
			{
				throw new ArgumentNullException("id");
			}

			return publisher.Configuration(
				Child.ForName("subscribedEvents").Eq(
					Child.ForName("event").Eq(
						Attrib.ForName("id").Eq(id),
						Attrib.ForName("handler").Eq(eventHandler))));
		}
	}
}