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
	using System.Collections.Generic;

	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;

	public class EventWiringContributor : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (IsPublisherWithInlineSubscribers(model))
			{
				WireConfigEvents(model);
			}
		}

		private void ExtractAndAddEventInfo(IDictionary<string, List<WireInfo>> subscribers2Evts, string subscriberKey, IConfiguration subscriber, ComponentModel model)
		{
			List<WireInfo> wireInfoList;
			if (subscribers2Evts.TryGetValue(subscriberKey, out wireInfoList) == false)
			{
				wireInfoList = new List<WireInfo>();
				subscribers2Evts[subscriberKey] = wireInfoList;
			}

			var eventName = subscriber.Attributes["event"];
			if (string.IsNullOrEmpty(eventName))
			{
				throw new EventWiringException("You must supply an 'event' " +
				                               "attribute which is the event name on the publisher you want to subscribe." +
				                               " Check node 'subscriber' for component " + model.Name + "and id = " + subscriberKey);
			}

			var handlerMethodName = subscriber.Attributes["handler"];
			if (string.IsNullOrEmpty(handlerMethodName))
			{
				throw new EventWiringException("You must supply an 'handler' attribute " +
				                               "which is the method on the subscriber that will handle the event." +
				                               " Check node 'subscriber' for component " + model.Name + "and id = " + subscriberKey);
			}

			wireInfoList.Add(new WireInfo(eventName, handlerMethodName));
		}

		private void AddSubscriberDependecyToModel(string subscriberKey, ComponentModel model)
		{
			var subscriber = new DependencyModel(DependencyType.ServiceOverride, subscriberKey, null, false);
			if (!model.Dependencies.Contains(subscriber))
			{
				model.Dependencies.Add(subscriber);
			}
		}

		private static string GetSubscriberKey(IConfiguration subscriber)
		{
			var subscriberKey = subscriber.Attributes["id"];
			if (string.IsNullOrEmpty(subscriberKey))
			{
				throw new EventWiringException("The subscriber node must have a valid Id assigned");
			}

			return subscriberKey;
		}

		private bool IsPublisherWithInlineSubscribers(ComponentModel model)
		{
			return model.Configuration != null &&
			       model.Configuration.Children["subscribers"] != null;
		}
		private void WireConfigEvents(ComponentModel model)
		{
			var subscribersNode = model.Configuration.Children["subscribers"];
			if (subscribersNode.Children.Count < 1)
			{
				throw new EventWiringException(
					"The subscribers node must have at least one subsciber child. Check node subscribers of the "
					+ model.Name + " component");
			}

			var subscribers2Events = new Dictionary<string, List<WireInfo>>();
			foreach (var subscriber in subscribersNode.Children)
			{
				var subscriberKey = GetSubscriberKey(subscriber);
				AddSubscriberDependecyToModel(subscriberKey, model);
				ExtractAndAddEventInfo(subscribers2Events, subscriberKey, subscriber, model);
			}

			model.ExtendedProperties[EventWiringFacility.SubscriberList] = subscribers2Events;
		}
	}
}