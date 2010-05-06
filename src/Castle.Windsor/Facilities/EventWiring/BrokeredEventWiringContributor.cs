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
	using System.Collections.Generic;
	using System.Reflection;

	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;

	public class BrokeredEventWiringContributor : IContributeComponentModelConstruction
	{
		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (IsPublisher(model))
			{
				RegisterPublisher(model);
			}

			if (IsSubscriber(model))
			{
				RegisterSubscriber(model);
			}
		}

		private static EventInfo ExtractEventInfo(ComponentModel model, string eventName)
		{
			EventInfo @event;
			if (model.Implementation != null)
			{
				@event = model.Implementation.GetEvent(eventName);
				if (@event != null)
				{
					return @event;
				}
			}
			@event = model.Service.GetEvent(eventName);
			if (@event != null)
			{
				return @event;
			}

			throw new EventWiringException(
				string.Format("Could not locate event '{0}' on component {1}. Make sure you didn't mistype the event name.", @event,
				              model));
		}

		private static MethodInfo ExtractMethodInfo(ComponentModel model, string handlerName)
		{
			MethodInfo handler;
			if (model.Implementation != null)
			{
				handler = model.Implementation.GetMethod(handlerName,
				                                         BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (handler != null)
				{
					return handler;
				}
			}

			handler = model.Service.GetMethod(handlerName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (handler != null)
			{
				return handler;
			}

			throw new EventWiringException(
				string.Format(
					"Could not locate event handler '{0}' on component {1}. Make sure you didn't mistype the method name.", handlerName,
					model.Name));
		}

		private static bool IsPublisher(ComponentModel model)
		{
			return model.Configuration != null && model.Configuration.Children["publishedEvents"] != null;
		}

		private static bool IsSubscriber(ComponentModel model)
		{
			return model.Configuration != null && model.Configuration.Children["subscribedEvents"] != null;
		}

		private static void RegisterEventHandler(string id, string handlerName, ComponentModel model,
		                                         IDictionary<string, MethodInfo> handlers)
		{
			try
			{
				var handler = ExtractMethodInfo(model, handlerName);
				handlers.Add(id, handler);
			}
			catch (ArgumentException duplicatedKey)
			{
				throw new EventWiringException(
					string.Format(
						"There's already handler registered for event with id '{0}'. You can't have more than one handler for single event on a component.",
						id), duplicatedKey);
			}
		}

		private static void RegisterPublishedEvent(string id, string @event, ComponentModel model,
		                                           IDictionary<string, EventInfo> events)
		{
			try
			{
				var info = ExtractEventInfo(model, @event);
				events.Add(id, info);
			}
			catch (ArgumentException duplicatedKey)
			{
				throw new EventWiringException(
					string.Format("There's already event registered with id '{0}'. Ids must be unique.", id), duplicatedKey);
			}
		}

		private static void RegisterPublisher(ComponentModel model)
		{
			var events = new Dictionary<string, EventInfo>(StringComparer.OrdinalIgnoreCase);
			foreach (var @event in model.Configuration.Children["publishedEvents"].Children)
			{
				var id = @event.Attributes["id"];
				var eventName = @event.Attributes["name"];
				RegisterPublishedEvent(id, eventName, model, events);
			}

			model.ExtendedProperties["publishedEvents"] = events;
		}

		private static void RegisterSubscriber(ComponentModel model)
		{
			var handlers = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
			foreach (var @event in model.Configuration.Children["subscribedEvents"].Children)
			{
				var id = @event.Attributes["id"];
				var handlerName = @event.Attributes["handler"];
				RegisterEventHandler(id, handlerName, model, handlers);
			}
			model.ExtendedProperties["subscribedEvents"] = handlers;
		}
	}
}