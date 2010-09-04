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

namespace Castle.Windsor
{
	using System;
	using System.Collections;
	using System.ComponentModel;

	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Registration;

	/// <summary>
	///   The <c>IWindsorContainer</c> interface exposes all the 
	///   functionality the Windsor implements.
	/// </summary>
	public interface IWindsorContainer : IServiceProviderEx, IDisposable
	{
		/// <summary>
		///   Shortcut to <see cref = "Resolve(string)" />
		/// </summary>
		[Obsolete("Use Resolve(key, new Arguments()) instead")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		object this[String key] { get; }

		/// <summary>
		///   Shortcut to <see cref = "Resolve(Type)" />
		/// </summary>
		[Obsolete("Use Resolve(service) or generic version instead")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		object this[Type service] { get; }

		/// <summary>
		///   Returns the inner instance of the MicroKernel
		/// </summary>
		IKernel Kernel { get; }

		/// <summary>
		///   Gets the container's name
		/// </summary>
		/// <remarks>
		///   Only useful when child containers are being used
		/// </remarks>
		/// <value>The container's name.</value>
		string Name { get; }

		/// <summary>
		///   Gets or sets the parent container if this instance
		///   is a sub container.
		/// </summary>
		IWindsorContainer Parent { get; set; }

		/// <summary>
		///   Registers a subcontainer. The components exposed
		///   by this container will be accessible from subcontainers.
		/// </summary>
		/// <param name = "childContainer"></param>
		void AddChildContainer(IWindsorContainer childContainer);

		/// <summary>
		///   Adds a component to be managed by the container
		/// </summary>
		/// <param name = "key">The key by which the component gets indexed.</param>
		/// <param name = "classType">The <see cref = "Type" /> to manage.</param>
		[Obsolete("Use Register(Component.For(classType).Named(key)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponent(String key, Type classType);

		/// <summary>
		///   Adds a component to be managed by the container
		/// </summary>
		/// <param name = "key">The key by which the component gets indexed.</param>
		/// <param name = "serviceType">The service <see cref = "Type" /> that the component implements.</param>
		/// <param name = "classType">The <see cref = "Type" /> to manage.</param>
		[Obsolete("Use Register(Component.For(serviceType).ImplementedBy(classType).Named(key)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponent(String key, Type serviceType, Type classType);

		/// <summary>
		///   Adds a component to be managed by the container.
		///   The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name = "T">The <see cref = "Type" /> to manage.</typeparam>
		[Obsolete("Use Register(Component.For<T>()) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponent<T>();

		/// <summary>
		///   Adds a component to be managed by the container
		/// </summary>
		/// <typeparam name = "T">The <see cref = "Type" /> to manage.</typeparam>
		/// <param name = "key">The key by which the component gets indexed.</param>
		[Obsolete("Use Register(Component.For<T>().Named(key)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponent<T>(String key);

		/// <summary>
		///   Adds a component to be managed by the container
		///   The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name = "I">The service <see cref = "Type" /> that the component implements.</typeparam>
		/// <typeparam name = "T">The <see cref = "Type" /> to manage.</typeparam>
		[Obsolete("Use Register(Component.For<I>().ImplementedBy<T>()) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponent<I, T>() where T : class;

		/// <summary>
		///   Adds a component to be managed by the container
		/// </summary>
		/// <typeparam name = "I">The service <see cref = "Type" /> that the component implements.</typeparam>
		/// <typeparam name = "T">The <see cref = "Type" /> to manage.</typeparam>
		/// <param name = "key">The key by which the component gets indexed.</param>
		[Obsolete("Use Register(Component.For<I>().ImplementedBy<T>().Named(key)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponent<I, T>(String key) where T : class;

		/// <summary>
		///   Adds a component to be managed by the container
		/// </summary>
		/// <param name = "key">The key by which the component gets indexed.</param>
		/// <param name = "classType">The <see cref = "Type" /> to manage.</param>
		/// <param name = "lifestyle">The <see cref = "LifestyleType" /> with which to manage the component.</param>
		[Obsolete("Use Register(Component.For(classType).Named(key)).Lifestyle.Is(lifestyle)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentLifeStyle(String key, Type classType, LifestyleType lifestyle);

		/// <summary>
		///   Adds a component to be managed by the container
		/// </summary>
		/// <param name = "key">The key by which the component gets indexed.</param>
		/// <param name = "serviceType">The service <see cref = "Type" /> that the component implements.</param>
		/// <param name = "classType">The <see cref = "Type" /> to manage.</param>
		/// <param name = "lifestyle">The <see cref = "LifestyleType" /> with which to manage the component.</param>
		[Obsolete(
			"Use Register(Component.For(serviceType).ImplementedBy(classType).Named(key)).Lifestyle.Is(lifestyle)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentLifeStyle(String key, Type serviceType, Type classType, LifestyleType lifestyle);

		/// <summary>
		///   Adds a component to be managed by the container.
		///   The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name = "T">The <see cref = "Type" /> to manage.</typeparam>
		/// <param name = "lifestyle">The <see cref = "LifestyleType" /> with which to manage the component.</param>
		[Obsolete("Use Register(Component.For<T>().Lifestyle.Is(lifestyle)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentLifeStyle<T>(LifestyleType lifestyle);

		/// <summary>
		///   Adds a component to be managed by the container
		/// </summary>
		/// <typeparam name = "T">The <see cref = "Type" /> to manage.</typeparam>
		/// <param name = "key">The key by which the component gets indexed.</param>
		/// <param name = "lifestyle">The <see cref = "LifestyleType" /> with which to manage the component.</param>
		[Obsolete("Use Register(Component.For<T>().Named(key).Lifestyle.Is(lifestyle)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentLifeStyle<T>(String key, LifestyleType lifestyle);

		/// <summary>
		///   Adds a component to be managed by the container
		///   The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name = "I">The service <see cref = "Type" /> that the component implements.</typeparam>
		/// <typeparam name = "T">The <see cref = "Type" /> to manage.</typeparam>
		/// <param name = "lifestyle">The <see cref = "LifestyleType" /> with which to manage the component.</param>
		[Obsolete("Use Register(Component.For<I>().ImplementedBy<T>().Lifestyle.Is(lifestyle)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentLifeStyle<I, T>(LifestyleType lifestyle) where T : class;

		/// <summary>
		///   Adds a component to be managed by the container
		/// </summary>
		/// <typeparam name = "I">The service <see cref = "Type" /> that the component implements.</typeparam>
		/// <typeparam name = "T">The <see cref = "Type" /> to manage.</typeparam>
		/// <param name = "key">The key by which the component gets indexed.</param>
		/// <param name = "lifestyle">The <see cref = "LifestyleType" /> with which to manage the component.</param>
		[Obsolete("Use Register(Component.For<I>().ImplementedBy<T>().Named(key).Lifestyle.Is(lifestyle)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentLifeStyle<I, T>(String key, LifestyleType lifestyle) where T : class;

		/// <summary>
		///   Adds a concrete class and an interface 
		///   as a component and specify the extended properties.
		///   Used by facilities, mostly.
		///   The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name = "I"></typeparam>
		/// <typeparam name = "T"></typeparam>
		/// <param name = "extendedProperties"></param>
		[Obsolete("Use Register(Component.For<I>().ImplementedBy<T>().ExtendedProperties(extendedProperties)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentProperties<I, T>(IDictionary extendedProperties) where T : class;

		/// <summary>
		///   Adds a concrete class and an interface 
		///   as a component and specify the extended properties.
		///   Used by facilities, mostly.
		/// </summary>
		/// <typeparam name = "I"></typeparam>
		/// <typeparam name = "T"></typeparam>
		/// <param name = "key"></param>
		/// <param name = "extendedProperties"></param>
		[Obsolete(
			"Use Register(Component.For<I>().ImplementedBy<T>().Named(key).ExtendedProperties(extendedProperties)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentProperties<I, T>(String key, IDictionary extendedProperties) where T : class;

		/// <summary>
		///   Adds a concrete class as a component and specify the extended properties.
		///   Used by facilities, mostly.
		/// </summary>
		/// <param name = "key"></param>
		/// <param name = "classType"></param>
		/// <param name = "extendedProperties"></param>
		[Obsolete("Use Register(Component.For(classType).Named(key)).ExtendedProperties(extendedProperties)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentWithProperties(String key, Type classType, IDictionary extendedProperties);

		/// <summary>
		///   Adds a concrete class and an interface 
		///   as a component and specify the extended properties.
		///   Used by facilities, mostly.
		/// </summary>
		/// <param name = "key"></param>
		/// <param name = "serviceType"></param>
		/// <param name = "classType"></param>
		/// <param name = "extendedProperties"></param>
		[Obsolete(
			"Use Register(Component.For(serviceType).ImplementedBy(classType).Named(key).ExtendedProperties(extendedProperties)) instead."
			)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentWithProperties(String key, Type serviceType, Type classType,
		                                             IDictionary extendedProperties);

		/// <summary>
		///   Adds a concrete class as a component and specify the extended properties.
		///   Used by facilities, mostly.
		///   The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name = "T"></typeparam>
		/// <param name = "extendedProperties"></param>
		[Obsolete("Use Register(Component.For<T>().ExtendedProperties(extendedProperties)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentWithProperties<T>(IDictionary extendedProperties);

		/// <summary>
		///   Adds a concrete class as a component and specify the extended properties.
		///   Used by facilities, mostly.
		/// </summary>
		/// <typeparam name = "T"></typeparam>
		/// <param name = "key"></param>
		/// <param name = "extendedProperties"></param>
		[Obsolete("Use Register(Component.For<T>().Named(key).ExtendedProperties(extendedProperties)) instead.")]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		IWindsorContainer AddComponentWithProperties<T>(String key, IDictionary extendedProperties);

		/// <summary>
		///   Registers a facility within the container.
		/// </summary>
		/// <param name = "key">The key by which the <see cref = "IFacility" /> gets indexed.</param>
		/// <param name = "facility">The <see cref = "IFacility" /> to add to the container.</param>
		IWindsorContainer AddFacility(String key, IFacility facility);

		/// <summary>
		///   Creates and adds an <see cref = "IFacility" /> facility to the container.
		/// </summary>
		/// <typeparam name = "T">The facility type.</typeparam>
		/// <param name = "key"></param>
		/// <returns></returns>
		IWindsorContainer AddFacility<T>(String key) where T : IFacility, new();

		/// <summary>
		///   Creates and adds an <see cref = "IFacility" /> facility to the container.
		/// </summary>
		/// <typeparam name = "T">The facility type.</typeparam>
		/// <param name = "key"></param>
		/// <param name = "onCreate">The callback for creation.</param>
		/// <returns></returns>
		IWindsorContainer AddFacility<T>(String key, Action<T> onCreate)
			where T : IFacility, new();

		/// <summary>
		///   Creates and adds an <see cref = "IFacility" /> facility to the container.
		/// </summary>
		/// <typeparam name = "T">The facility type.</typeparam>
		/// <param name = "key"></param>
		/// <param name = "onCreate">The callback for creation.</param>
		/// <returns></returns>
		IWindsorContainer AddFacility<T>(String key, Func<T, object> onCreate)
			where T : IFacility, new();

		/// <summary>
		///   Creates and adds an <see cref = "IFacility" /> facility to the container.
		/// </summary>
		/// <typeparam name = "T">The facility type.</typeparam>
		/// <returns></returns>
		IWindsorContainer AddFacility<T>() where T : IFacility, new();

		/// <summary>
		///   Creates and adds an <see cref = "IFacility" /> facility to the container.
		/// </summary>
		/// <typeparam name = "T">The facility type.</typeparam>
		/// <param name = "onCreate">The callback for creation.</param>
		/// <returns></returns>
		IWindsorContainer AddFacility<T>(Action<T> onCreate)
			where T : IFacility, new();

		/// <summary>
		///   Creates and adds an <see cref = "IFacility" /> facility to the container.
		/// </summary>
		/// <typeparam name = "T">The facility type.</typeparam>
		/// <param name = "onCreate">The callback for creation.</param>
		/// <returns></returns>
		IWindsorContainer AddFacility<T>(Func<T, object> onCreate)
			where T : IFacility, new();

		/// <summary>
		///   Gets a child container instance by name.
		/// </summary>
		/// <param name = "name">The container's name.</param>
		/// <returns>The child container instance or null</returns>
		IWindsorContainer GetChildContainer(string name);

		/// <summary>
		///   Installs the components provided by the <see cref = "IWindsorInstaller" />s
		///   with the <see cref = "IWindsorContainer" />.
		///   <param name = "installers">The component installers.</param>
		///   <returns>The container.</returns>
		/// </summary>
		IWindsorContainer Install(params IWindsorInstaller[] installers);

		/// <summary>
		///   Registers the components provided by the <see cref = "IRegistration" />s
		///   with the <see cref = "IWindsorContainer" />.
		///   <para />
		///   Create a new registration using <see cref = "MicroKernel.Registration.Component" />.For() or <see cref = "AllTypes" />.
		/// </summary>
		/// <example>
		///   <code>
		///     container.Register(Component.For&lt;IService&gt;().ImplementedBy&lt;DefaultService&gt;());
		///   </code>
		/// </example>
		/// <param name = "registrations">The component registrations.</param>
		/// <returns>The container.</returns>
		IWindsorContainer Register(params IRegistration[] registrations);

		/// <summary>
		///   Releases a component instance
		/// </summary>
		/// <param name = "instance"></param>
		void Release(object instance);

		/// <summary>
		///   Remove a child container
		/// </summary>
		/// <param name = "childContainer"></param>
		void RemoveChildContainer(IWindsorContainer childContainer);

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <param name = "key"></param>
		/// <param name = "arguments"></param>
		/// <returns></returns>
		object Resolve(String key, IDictionary arguments);

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <param name = "key"></param>
		/// <param name = "argumentsAsAnonymousType"></param>
		/// <returns></returns>
		object Resolve(String key, object argumentsAsAnonymousType);

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <param name = "key"></param>
		/// <param name = "service"></param>
		/// <returns></returns>
		object Resolve(String key, Type service);

		/// <summary>
		///   Returns a component instance by the service
		/// </summary>
		/// <param name = "service"></param>
		/// <returns></returns>
		object Resolve(Type service);

		/// <summary>
		///   Returns a component instance by the service
		/// </summary>
		/// <param name = "service"></param>
		/// <param name = "arguments"></param>
		/// <returns></returns>
		object Resolve(Type service, IDictionary arguments);

		/// <summary>
		///   Returns a component instance by the service
		/// </summary>
		/// <param name = "service"></param>
		/// <param name = "argumentsAsAnonymousType"></param>
		/// <returns></returns>
		object Resolve(Type service, object argumentsAsAnonymousType);

		/// <summary>
		///   Returns a component instance by the service
		/// </summary>
		/// <typeparam name = "T">Service type</typeparam>
		/// <returns>The component instance</returns>
		T Resolve<T>();

		/// <summary>
		///   Returns a component instance by the service
		/// </summary>
		/// <typeparam name = "T">Service type</typeparam>
		/// <param name = "arguments"></param>
		/// <returns>The component instance</returns>
		T Resolve<T>(IDictionary arguments);

		/// <summary>
		///   Returns a component instance by the service
		/// </summary>
		/// <typeparam name = "T">Service type</typeparam>
		/// <param name = "argumentsAsAnonymousType"></param>
		/// <returns>The component instance</returns>
		T Resolve<T>(object argumentsAsAnonymousType);

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <param name = "key">Component's key</param>
		/// <typeparam name = "T">Service type</typeparam>
		/// <returns>The Component instance</returns>
		T Resolve<T>(String key);

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <typeparam name = "T">Service type</typeparam>
		/// <param name = "key">Component's key</param>
		/// <param name = "arguments"></param>
		/// <returns>The Component instance</returns>
		T Resolve<T>(String key, IDictionary arguments);

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <typeparam name = "T">Service type</typeparam>
		/// <param name = "key">Component's key</param>
		/// <param name = "argumentsAsAnonymousType"></param>
		/// <returns>The Component instance</returns>
		T Resolve<T>(String key, object argumentsAsAnonymousType);

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <param name = "key"></param>
		/// <param name = "service"></param>
		/// <param name = "arguments"></param>
		/// <returns></returns>
		object Resolve(String key, Type service, IDictionary arguments);

		/// <summary>
		///   Returns a component instance by the key
		/// </summary>
		/// <param name = "key"></param>
		/// <param name = "service"></param>
		/// <param name = "argumentsAsAnonymousType"></param>
		/// <returns></returns>
		object Resolve(String key, Type service, object argumentsAsAnonymousType);

		/// <summary>
		///   Resolve all valid components that match this type.
		/// </summary>
		/// <typeparam name = "T">The service type</typeparam>
		T[] ResolveAll<T>();

		/// <summary>
		///   Resolve all valid components that mathc this service
		///   <param name = "service">the service to match</param>
		/// </summary>
		Array ResolveAll(Type service);

		/// <summary>
		///   Resolve all valid components that mathc this service
		///   <param name = "service">the service to match</param>
		///   <param name = "arguments">Arguments to resolve the service</param>
		/// </summary>
		Array ResolveAll(Type service, IDictionary arguments);

		/// <summary>
		///   Resolve all valid components that mathc this service
		///   <param name = "service">the service to match</param>
		///   <param name = "argumentsAsAnonymousType">Arguments to resolve the service</param>
		/// </summary>
		Array ResolveAll(Type service, object argumentsAsAnonymousType);

		/// <summary>
		///   Resolve all valid components that match this type.
		///   <typeparam name = "T">The service type</typeparam>
		///   <param name = "arguments">Arguments to resolve the service</param>
		/// </summary>
		T[] ResolveAll<T>(IDictionary arguments);

		/// <summary>
		///   Resolve all valid components that match this type.
		///   <typeparam name = "T">The service type</typeparam>
		///   <param name = "argumentsAsAnonymousType">Arguments to resolve the service</param>
		/// </summary>
		T[] ResolveAll<T>(object argumentsAsAnonymousType);
	}
}