﻿// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel
{
	using System;
	using System.Collections;
	using System.ComponentModel;

	using Castle.Core;
	using Castle.MicroKernel.ComponentActivator;

#if (SILVERLIGHT)
	public partial class DefaultKernel : IKernel, IKernelEvents
#else
	public partial class DefaultKernel
#endif
	{
		[Obsolete("Use Register(Component.For(classType).Named(key)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void AddComponent(String key, Type classType)
		{
			AddComponent(key, classType, classType);
		}

		[Obsolete("Use Register(Component.For(classType).Named(key).Lifestyle.Is(lifestyle)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponent(string key, Type classType, LifestyleType lifestyle)
		{
			AddComponent(key, classType, classType, lifestyle);
		}

		[Obsolete("Use Register(Component.For(classType).Named(key).Lifestyle.Is(lifestyle)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponent(string key, Type classType, LifestyleType lifestyle, bool overwriteLifestyle)
		{
			AddComponent(key, classType, classType, lifestyle, overwriteLifestyle);
		}

		[Obsolete("Use Register(Component.For(serviceType).ImplementedBy(classType).Named(key)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void AddComponent(String key, Type serviceType, Type classType)
		{
			AddComponent(key, serviceType, classType, LifestyleType.Singleton);
		}

		[Obsolete("Use Register(Component.For(serviceType).ImplementedBy(classType).Named(key).Lifestyle.Is(lifestyle)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyle)
		{
			AddComponent(key, serviceType, classType, lifestyle, false);
		}

		[Obsolete("Use Register(Component.For(serviceType).ImplementedBy(classType).Named(key).Lifestyle.Is(lifestyle)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyle,
								 bool overwriteLifestyle)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (classType == null) throw new ArgumentNullException("classType");
			if (LifestyleType.Undefined == lifestyle)
				throw new ArgumentException("The specified lifestyle must be Thread, Transient, or Singleton.", "lifestyle");
			ComponentModel model = ComponentModelBuilder.BuildModel(key, serviceType, classType, null);

			if (overwriteLifestyle || LifestyleType.Undefined == model.LifestyleType)
			{
				model.LifestyleType = lifestyle;
			}

			RaiseComponentModelCreated(model);

			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		[Obsolete("Use Register(Component.For(classType).Named(key).ExtendedProperties(extendedProperties)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void AddComponentWithExtendedProperties(String key, Type classType, IDictionary extendedProperties)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (extendedProperties == null) throw new ArgumentNullException("extendedProperties");
			if (classType == null) throw new ArgumentNullException("classType");

			ComponentModel model = ComponentModelBuilder.BuildModel(key, classType, classType, extendedProperties);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		[Obsolete("Use Register(Component.For(serviceType).ImplementedBy(classType).Named(key).ExtendedProperties(extendedProperties)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void AddComponentWithExtendedProperties(String key, Type serviceType, Type classType,
															   IDictionary extendedProperties)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (extendedProperties == null) throw new ArgumentNullException("extendedProperties");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (classType == null) throw new ArgumentNullException("classType");

			ComponentModel model = ComponentModelBuilder.BuildModel(key, serviceType, classType, extendedProperties);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}
		// NOTE: this is from IKernelInternal
		public virtual void AddCustomComponent(ComponentModel model)
		{
			if (model == null) throw new ArgumentNullException("model");

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);

			object skipRegistration = model.ExtendedProperties[ComponentModel.SkipRegistration];

			if (skipRegistration != null)
			{
				RegisterHandler(model.Name, handler, (bool)skipRegistration);
			}
			else
			{
				RegisterHandler(model.Name, handler);
			}
		}

		[Obsolete("Use Register(Component.For(instance.GetType()).Named(key).Instance(instance)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponentInstance(String key, object instance)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (instance == null) throw new ArgumentNullException("instance");

			Type classType = instance.GetType();

			var model = new ComponentModel(key, classType, classType)
			{
				LifestyleType = LifestyleType.Singleton,
				CustomComponentActivator = typeof(ExternalInstanceActivator)
			};
			model.ExtendedProperties["instance"] = instance;

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		[Obsolete("Use Register(Component.For(serviceType).Named(key).Instance(instance)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponentInstance(String key, Type serviceType, object instance)
		{
			AddComponentInstance(key, serviceType, instance.GetType(), instance);
		}

		[Obsolete("Use Register(Component.For(serviceType).ImplementedBy(classType).Named(key).Instance(instance)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponentInstance(string key, Type serviceType, Type classType, object instance)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (instance == null) throw new ArgumentNullException("instance");
			if (classType == null) throw new ArgumentNullException("classType");

			var model = new ComponentModel(key, serviceType, classType)
			{
				LifestyleType = LifestyleType.Singleton,
				CustomComponentActivator = typeof(ExternalInstanceActivator)
			};
			model.ExtendedProperties["instance"] = instance;

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		[Obsolete("Use Register(Component.For<T>()) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponent<T>()
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, classType);
		}

		[Obsolete("Use Register(Component.For<T>().Lifestyle.Is(lifestyle)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponent<T>(LifestyleType lifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, classType, lifestyle);
		}

		[Obsolete("Use Register(Component.For<T>().Lifestyle.Is(lifestyle)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponent<T>(LifestyleType lifestyle, bool overwriteLifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, classType, lifestyle, overwriteLifestyle);
		}

		[Obsolete("Use Register(Component.For(serviceType).ImplementedBy<T>()) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponent<T>(Type serviceType)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, serviceType, classType);
		}

		[Obsolete("Use Register(Component.For(serviceType).ImplementedBy<T>()) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponent<T>(Type serviceType, LifestyleType lifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, serviceType, classType, lifestyle);
		}

		[Obsolete("Use Register(Component.For(serviceType).ImplementedBy<T>().Lifestyle.Is(lifestyle)) or generic version instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponent<T>(Type serviceType, LifestyleType lifestyle, bool overwriteLifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, serviceType, classType, lifestyle, overwriteLifestyle);
		}

		[Obsolete("Use Register(Component.For<T>().Instance(instance)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponentInstance<T>(object instance)
		{
			Type serviceType = typeof(T);
			AddComponentInstance(serviceType.FullName, serviceType, instance);
		}

		[Obsolete("Use Register(Component.For<T>().Instance(instance)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void AddComponentInstance<T>(Type serviceType, object instance)
		{
			Type classType = typeof(T);
			AddComponentInstance(classType.FullName, serviceType, classType, instance);
		}
	}
}
