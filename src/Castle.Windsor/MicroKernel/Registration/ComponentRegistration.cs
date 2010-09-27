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

namespace Castle.MicroKernel.Registration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;

	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.Core.Internal;
	using Castle.DynamicProxy;
	using Castle.MicroKernel;
	using Castle.MicroKernel.ComponentActivator;
	using Castle.MicroKernel.Context;
	using Castle.MicroKernel.LifecycleConcerns;
	using Castle.MicroKernel.Proxy;
	using Castle.MicroKernel.Registration.Interceptor;
	using Castle.MicroKernel.Registration.Lifestyle;
	using Castle.MicroKernel.Registration.Proxy;

	/// <summary>
	/// Delegate to filter component registration.
	/// </summary>
	/// <param name="kernel">The kernel.</param>
	/// <param name="model">The component model.</param>
	/// <returns>true if accepted.</returns>
	public delegate bool ComponentFilter(IKernel kernel, ComponentModel model);

	/// <summary>
	/// Registration for a single type as a component with the kernel.
	/// <para />
	/// You can create a new registration with the <see cref="Component"/> factory.
	/// </summary>
	/// <typeparam name="TService">The service type</typeparam>
	public class ComponentRegistration<TService> : IRegistration
	{
		private readonly ICollection<IRegistration> additionalRegistrations;
		private readonly ICollection<ComponentDescriptor<TService>> descriptors;
		private readonly ICollection<Type> forwardedTypes;
		private ComponentModel componentModel;
		private ComponentFilter ifFilter;
		private Type implementation;
		private String name;
		private bool overwrite;
		private bool registered;
		private Type serviceType;
		private ComponentFilter unlessFilter;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentRegistration{S}"/> class.
		/// </summary>
		public ComponentRegistration()
		{
			overwrite = false;
			registered = false;
			serviceType = typeof(TService);
			forwardedTypes = new List<Type>();
			descriptors = new List<ComponentDescriptor<TService>>();
			additionalRegistrations = new List<IRegistration>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentRegistration{S}"/> class
		/// with an existing <see cref="ComponentModel"/>.
		/// </summary>
		protected ComponentRegistration(ComponentModel componentModel)
			: this()
		{
			if (componentModel == null)
			{
				throw new ArgumentNullException("componentModel");
			}

			this.componentModel = componentModel;
			name = componentModel.Name;
			serviceType = componentModel.Service;
			implementation = componentModel.Implementation;
		}

		/// <summary>
		/// Gets the forwarded service types on behalf of this component.
		/// <para />
		/// Add more types to forward using <see cref="Forward(Type[])"/>.
		/// </summary>
		/// <value>The types of the forwarded services.</value>
		public Type[] ForwardedTypes
		{
			get { return forwardedTypes.ToArray(); }
		}

		/// <summary>
		/// The concrete type that implements the service.
		/// <para />
		/// To set the implementation, use <see cref="ImplementedBy"/>.
		/// </summary>
		/// <value>The implementation of the service.</value>
		public Type Implementation
		{
			get { return implementation; }
		}

		/// <summary>
		/// Set the lifestyle of this component.
		/// For example singleton and transient (also known as 'factory').
		/// </summary>
		/// <value>The with lifestyle.</value>
		public LifestyleGroup<TService> LifeStyle
		{
			get { return new LifestyleGroup<TService>(this); }
		}

		/// <summary>
		/// The name of the component. Will become the key for the component in the kernel.
		/// <para />
		/// To set the name, use <see cref="Named"/>.
		/// <para />
		/// If not set, the <see cref="Type.FullName"/> of the <see cref="Implementation"/>
		/// will be used as the key to register the component.
		/// </summary>
		/// <value>The name.</value>
		public String Name
		{
			get { return name; }
		}

		/// <summary>
		/// Set proxy for this component.
		/// </summary>
		/// <value>The proxy.</value>
		public ProxyGroup<TService> Proxy
		{
			get { return new ProxyGroup<TService>(this); }
		}

		/// <summary>
		/// The type of the service, the same as <typeparamref name="TService"/>.
		/// <para />
		/// This is the first type passed to <see cref="Component.For(Type)"/>.
		/// </summary>
		/// <value>The type of the service.</value>
		public Type ServiceType
		{
			get { return serviceType; }
			protected set { serviceType = value; }
		}

		internal bool IsOverWrite
		{
			get { return overwrite; }
		}

		/// <summary>
		/// Marks the components with one or more actors.
		/// </summary>
		/// <param name="actors">The component actors.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> ActAs(params object[] actors)
		{
			foreach (var actor in actors)
			{
				if (actor != null)
				{
					DependsOn(Property.ForKey(Guid.NewGuid().ToString()).Eq(actor));
				}
			}
			return this;
		}

		/// <summary>
		/// Set a custom <see cref="IComponentActivator"/> which creates and destroys the component.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<TService> Activator<A>() where A : IComponentActivator
		{
			return AddAttributeDescriptor("componentActivatorType", typeof(A).AssemblyQualifiedName);
		}

		/// <summary>
		/// Adds the attribute descriptor.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> AddAttributeDescriptor(string key, string value)
		{
			AddDescriptor(new AttributeDescriptor<TService>(key, value));
			return this;
		}

		/// <summary>
		/// Adds the descriptor.
		/// </summary>
		/// <param name="descriptor">The descriptor.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> AddDescriptor(ComponentDescriptor<TService> descriptor)
		{
			descriptor.Registration = this;
			descriptors.Add(descriptor);
			return this;
		}

		/// <summary>
		/// Creates an attribute descriptor.
		/// </summary>
		/// <param name="key">The attribute key.</param>
		/// <returns></returns>
		public AttributeKeyDescriptor<TService> Attribute(string key)
		{
			return new AttributeKeyDescriptor<TService>(this, key);
		}

		/// <summary>
		/// Apply more complex configuration to this component registration.
		/// </summary>
		/// <param name="configNodes">The config nodes.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> Configuration(params Node[] configNodes)
		{
			return AddDescriptor(new ConfigurationDescriptor<TService>(configNodes));
		}

		/// <summary>
		/// Apply more complex configuration to this component registration.
		/// </summary>
		/// <param name="configuration">The configuration <see cref="MutableConfiguration"/>.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> Configuration(IConfiguration configuration)
		{
			return AddDescriptor(new ConfigurationDescriptor<TService>(configuration));
		}

#if !SILVERLIGHT
		/// <summary>
		/// Obsolete, use <see cref="DependsOn(Property[])"/> instead.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		[Obsolete("Obsolete, use DependsOn(Property[]) instead.", true)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ComponentRegistration<TService> CustomDependencies(params Property[] dependencies)
		{
			return DependsOn(dependencies);
		}

		/// <summary>
		/// Obsolete, use <see cref="DependsOn(IDictionary)"/> instead.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		[Obsolete("Obsolete, use DependsOn(IDictionary) instead.", true)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ComponentRegistration<TService> CustomDependencies(IDictionary dependencies)
		{
			return DependsOn(dependencies);
		}

		/// <summary>
		/// Obsolete, use <see cref="DependsOn(object)"/> instead.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		[Obsolete("Obsolete, use DependsOn(object) instead.", true)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ComponentRegistration<TService> CustomDependencies(object dependencies)
		{
			return DependsOn(dependencies);
		}
#endif
		/// <summary>
		/// Specify custom dependencies using <see cref="Property.ForKey(string)"/> or <see cref="Property.ForKey(System.Type)"/>.
		/// <para />
		/// You can pass <see cref="ServiceOverride"/>s to specify the components
		/// this component should be resolved with.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> DependsOn(params Property[] dependencies)
		{
			if(dependencies == null || dependencies.Length == 0)
			{
				return this;
			}

			var serviceOverrides = dependencies.OfType<ServiceOverride>().ToArray();
			if(serviceOverrides.Length>0)
			{
				AddDescriptor(new ServiceOverrideDescriptor<TService>(serviceOverrides));
				dependencies = dependencies.Except(serviceOverrides).ToArray();
			}
			return AddDescriptor(new CustomDependencyDescriptor<TService>(dependencies));
		}

		/// <summary>
		/// Uses a dictionary of key/value pairs, to specify custom dependencies.
		/// <para />
		/// Use <see cref="ServiceOverrides(IDictionary)"/> to specify the components
		/// this component should be resolved with.
		/// </summary>
		/// <param name="dependencies">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> DependsOn(IDictionary dependencies)
		{
			return AddDescriptor(new CustomDependencyDescriptor<TService>(dependencies));
		}
		
		/// <summary>
		/// Uses an (anonymous) object as a dictionary, to specify custom dependencies.
		/// <para />
		/// Use <see cref="ServiceOverrides(object)"/> to specify the components
		/// this component should be resolved with.
		/// </summary>
		/// <param name="anonymous">The dependencies.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> DependsOn(object anonymous)
		{
			return AddDescriptor(new CustomDependencyDescriptor<TService>(anonymous));
		}

		/// <summary>
		/// Allows custom dependencies to by defined dyncamically.
		/// </summary>
		/// <param name="resolve">The delegate used for providing dynamic parameters.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> DynamicParameters(DynamicParametersDelegate resolve)
		{
			return DynamicParameters((k, c, d) =>
			{
				resolve(k, d);
				return null;
			});
		}

		/// <summary>
		/// Allows custom dependencies to by defined dyncamically with releasing capability.
		/// </summary>
		/// <param name="resolve">The delegate used for providing dynamic parameters.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> DynamicParameters(DynamicParametersResolveDelegate resolve)
		{
			return DynamicParameters((k, c, d) => resolve(k, d));
		}

		/// <summary>
		/// Allows custom dependencies to by defined dynamically with releasing capability.
		/// </summary>
		/// <param name="resolve">The delegate used for providing dynamic parameters.</param>
		/// <returns></returns>
		/// <remarks>
		/// Use <see cref="CreationContext"/> when resolving components from <see cref="IKernel"/> in order to detect cycles.
		/// </remarks>
		public ComponentRegistration<TService> DynamicParameters(DynamicParametersWithContextResolveDelegate resolve)
		{
			AddDescriptor(new DynamicParametersDescriptor<TService>(resolve));
			return this;
		}

		/// <summary>
		/// Sets <see cref="ComponentModel.ExtendedProperties"/> for this component.
		/// </summary>
		/// <param name="properties">The extended properties.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> ExtendedProperties(params Property[] properties)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<TService>(properties));
		}

		/// <summary>
		/// Sets <see cref="ComponentModel.ExtendedProperties"/> for this component.
		/// </summary>
		/// <param name="anonymous">The extendend properties as key/value pairs.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> ExtendedProperties(object anonymous)
		{
			return AddDescriptor(new ExtendedPropertiesDescriptor<TService>(anonymous));
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <param name="types">The types to forward.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> Forward(params Type[] types)
		{
			return Forward((IEnumerable<Type>)types);
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <typeparam name="TSecondService">The forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public ComponentRegistration<TService> Forward<TSecondService>()
		{
			return Forward(new[] { typeof(TSecondService) });
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <typeparam name="TSecondService">The first forwarded type.</typeparam>
		/// <typeparam name="TThirdService">The second forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public ComponentRegistration<TService> Forward<TSecondService, TThirdService>()
		{
			return Forward(new[] { typeof(TSecondService), typeof(TThirdService) });
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <typeparam name="TSecondService">The first forwarded type.</typeparam>
		/// <typeparam name="TThirdService">The second forwarded type.</typeparam>
		/// <typeparam name="TFourthService">The third forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public ComponentRegistration<TService> Forward<TSecondService, TThirdService, TFourthService>()
		{
			return Forward(new[] { typeof(TSecondService), typeof(TThirdService), typeof(TFourthService) });
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <typeparam name="TSecondService">The first forwarded type.</typeparam>
		/// <typeparam name="TThirdService">The second forwarded type.</typeparam>
		/// <typeparam name="TFourthService">The third forwarded type.</typeparam>
		/// <typeparam name="TFifthService">The fourth forwarded type.</typeparam>
		/// <returns>The component registration.</returns>
		public ComponentRegistration<TService> Forward<TSecondService, TThirdService, TFourthService, TFifthService>()
		{
			return Forward(new[] { typeof(TSecondService), typeof(TThirdService), typeof(TFourthService), typeof(TFifthService) });
		}

		/// <summary>
		/// Registers the service types on behalf of this component.
		/// </summary>
		/// <param name="types">The types to forward.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> Forward(IEnumerable<Type> types)
		{
			foreach (Type type in types)
			{
				if (!forwardedTypes.Contains(type) && type != serviceType)
				{
					forwardedTypes.Add(type);
				}
			}
			return this;
		}

		/// <summary>
		/// Assigns a conditional predication which must be satisfied.
		/// <para />
		/// The component will only be registered into the kernel 
		/// if this predicate is satisfied (or not assigned at all).
		/// </summary>
		/// <param name="ifFilter">The predicate to satisfy.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> If(ComponentFilter ifFilter)
		{
			this.ifFilter += ifFilter;
			return this;
		}

		/// <summary>
		/// Sets the concrete type that implements the service to <typeparamref name="TImpl"/>.
		/// <para />
		/// If not set, the <see cref="ServiceType"/> will be used as the implementation for this component.
		/// </summary>
		/// <typeparam name="TImpl">The type that is the implementation for the service.</typeparam>
		/// <returns></returns>
		public ComponentRegistration<TService> ImplementedBy<TImpl>() where TImpl : TService
		{
			return ImplementedBy(typeof(TImpl));
		}

		/// <summary>
		/// Sets the concrete type that implements the service to <paramref name="type"/>.
		/// <para />
		/// If not set, the <see cref="ServiceType"/> will be used as the implementation for this component.
		/// </summary>
		/// <param name="type">The type that is the implementation for the service.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> ImplementedBy(Type type)
		{
			if (implementation != null && implementation != typeof(LateBoundComponent))
			{
				var message = String.Format("This component has already been assigned implementation {0}",
				                            implementation.FullName);
				throw new ComponentRegistrationException(message);
			}

			implementation = type;
			return this;
		}

		/// <summary>
		/// Assigns an existing instance as the component for this registration.
		/// </summary>
		/// <param name="instance">The component instance.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> Instance(TService instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return ImplementedBy(instance.GetType())
				.Activator<ExternalInstanceActivator>()
				.ExtendedProperties(Property.ForKey("instance").Eq(instance));
		}

		/// <summary>
		/// Set the interceptors for this component.
		/// </summary>
		/// <param name="interceptors">The interceptors.</param>
		/// <returns></returns>
		public InterceptorGroup<TService> Interceptors(params InterceptorReference[] interceptors)
		{
			return new InterceptorGroup<TService>(this, interceptors);
		}

		/// <summary>
		/// Set the interceptors for this component.
		/// </summary>
		/// <param name="interceptors">The interceptors.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> Interceptors(params Type[] interceptors)
		{
			return AddDescriptor(new InterceptorDescriptor<TService>(interceptors.Select(t => new InterceptorReference(t)).ToArray()));
		}

		/// <summary>
		/// Set the interceptor for this component.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<TService> Interceptors<TInterceptor>() where TInterceptor : IInterceptor
		{
			return AddDescriptor(new InterceptorDescriptor<TService>(new[] { new InterceptorReference(typeof(TInterceptor)) }));
		}

		/// <summary>
		/// Set the interceptor for this component.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<TService> Interceptors<TInterceptor1, TInterceptor2>()
			where TInterceptor1 : IInterceptor
			where TInterceptor2 : IInterceptor
		{
			return Interceptors<TInterceptor1>().Interceptors<TInterceptor2>();
		}

		/// <summary>
		/// Set the interceptor for this component.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<TService> Interceptors(params string[] keys)
		{
			return AddDescriptor(new InterceptorDescriptor<TService>(keys.Select(InterceptorReference.ForKey).ToArray()));
		}

		/// <summary>
		/// Change the name of this registration. 
		/// This will be the key for the component in the kernel.
		/// <para />
		/// If not set, the <see cref="Type.FullName"/> of the <see cref="Implementation"/>
		/// will be used as the key to register the component.
		/// </summary>
		/// <param name="name">The name of this registration.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> Named(String name)
		{
			if (this.name != null)
			{
				var message = String.Format("This component has already been assigned name '{0}'", this.name);
				throw new ComponentRegistrationException(message);
			}

			this.name = name;
			return this;
		}

		/// <summary>
		/// Stores a set of <see cref="OnCreateActionDelegate{T}"/> which will be invoked when the component
		/// is created and before it's returned from the container.
		/// </summary>
		/// <param name="actions">A set of actions to be executed right after the component is created and before it's returned from the container.</param>
		public ComponentRegistration<TService> OnCreate(params OnCreateActionDelegate<TService>[] actions)
		{
			AddDescriptor(new OnCreateComponentDescriptor<TService>(actions));
			return this;
		}

		/// <summary>
		/// With the overwrite.
		/// </summary>
		/// <returns></returns>
		public ComponentRegistration<TService> OverWrite()
		{
			overwrite = true;
			return this;
		}

		/// <summary>
		/// Set configuration parameters with string or <see cref="IConfiguration"/> values.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> Parameters(params Parameter[] parameters)
		{
			return AddDescriptor(new ParametersDescriptor<TService>(parameters));
		}

		/// <summary>
		/// Sets the interceptor selector for this component.
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ComponentRegistration<TService> SelectInterceptorsWith(IInterceptorSelector selector)
		{
			return SelectInterceptorsWith(s => s.Instance(selector));
		}

		/// <summary>
		/// Sets the interceptor selector for this component.
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		public ComponentRegistration<TService> SelectInterceptorsWith(Action<ItemRegistration<IInterceptorSelector>> selector)
		{
			var registration = new ItemRegistration<IInterceptorSelector>();
			selector.Invoke(registration);
			return AddDescriptor(new InterceptorSelectorDescriptor<TService>(registration.Item));
		}

		/// <summary>
		/// Override (some of) the services that this component needs.
		/// Use <see cref="ServiceOverride.ForKey(string)"/> to create an override.
		/// <para />
		/// Each key represents the service dependency of this component, for example the name of a constructor argument or a property.
		/// The corresponding value is the key of an other component registered to the kernel, and is used to resolve the dependency.
		/// <para />
		/// To specify dependencies which are not services, use <see cref="DependsOn(Property[])"/>
		/// </summary>
		/// <param name="overrides">The service overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> ServiceOverrides(params ServiceOverride[] overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<TService>(overrides));
		}

		/// <summary>
		/// Override (some of) the services that this component needs, using a dictionary.
		/// <para />
		/// Each key represents the service dependency of this component, for example the name of a constructor argument or a property.
		/// The corresponding value is the key of an other component registered to the kernel, and is used to resolve the dependency.
		/// <para />
		/// To specify dependencies which are not services, use <see cref="DependsOn(IDictionary)"/>
		/// </summary>
		/// <param name="overrides">The service overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> ServiceOverrides(IDictionary overrides)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<TService>(overrides));
		}
		
		/// <summary>
		/// Override (some of) the services that this component needs, using an (anonymous) object as a dictionary.
		/// <para />
		/// Each key represents the service dependency of this component, for example the name of a constructor argument or a property.
		/// The corresponding value is the key of an other component registered to the kernel, and is used to resolve the dependency.
		/// <para />
		/// To specify dependencies which are not services, use <see cref="DependsOn(object)"/>
		/// </summary>
		/// <param name="anonymous">The service overrides.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> ServiceOverrides(object anonymous)
		{
			return AddDescriptor(new ServiceOverrideDescriptor<TService>(anonymous));
		}

		/// <summary>
		/// Assigns a conditional predication which must not be satisfied. 
		/// <para />
		/// The component will only be registered into the kernel 
		/// if this predicate is not satisfied (or not assigned at all).
		/// </summary>
		/// <param name="unlessFilter">The predicate not to satisify.</param>
		/// <returns></returns>
		public ComponentRegistration<TService> Unless(ComponentFilter unlessFilter)
		{
			this.unlessFilter += unlessFilter;
			return this;
		}

		/// <summary>
		/// Uses a factory to instantiate the component
		/// </summary>
		/// <typeparam name="U">Factory type. This factory has to be registered in the kernel.</typeparam>
		/// <typeparam name="V">Implementation type.</typeparam>
		/// <param name="factory">Factory invocation</param>
		/// <returns></returns>
		public ComponentRegistration<TService> UsingFactory<U, V>(Converter<U, V> factory) where V : TService
		{
			return UsingFactoryMethod(kernel => factory.Invoke(kernel.Resolve<U>()));
		}

		/// <summary>
		/// Uses a factory method to instantiate the component.
		/// </summary>
		/// <typeparam name="TImpl">Implementation type</typeparam>
		/// <param name="factoryMethod">Factory method</param>
		/// <returns></returns>
		public ComponentRegistration<TService> UsingFactoryMethod<TImpl>(Func<TImpl> factoryMethod) where TImpl : TService
		{
			return UsingFactoryMethod((k, m, c) => factoryMethod());
		}

		/// <summary>
		/// Uses a factory method to instantiate the component.
		/// </summary>
		/// <typeparam name="TImpl">Implementation type</typeparam>
		/// <param name="factoryMethod">Factory method</param>
		/// <returns></returns>
		public ComponentRegistration<TService> UsingFactoryMethod<TImpl>(Converter<IKernel, TImpl> factoryMethod) where TImpl : TService
		{
			return UsingFactoryMethod((k, m, c) => factoryMethod(k));
		}

		/// <summary>
		/// Uses a factory method to instantiate the component.
		/// </summary>
		/// <typeparam name="TImpl">Implementation type</typeparam>
		/// <param name="factoryMethod">Factory method</param>
		/// <returns></returns>
		public ComponentRegistration<TService> UsingFactoryMethod<TImpl>(Func<IKernel, ComponentModel, CreationContext, TImpl> factoryMethod)
			where TImpl : TService
		{
			Activator<FactoryMethodActivator<TImpl>>()
				.ExtendedProperties(Property.ForKey("factoryMethodDelegate").Eq(factoryMethod));

			if (implementation == null && serviceType.IsSealed == false)
			{
				implementation = typeof(LateBoundComponent);
			}
			return this;
		}

		/// <summary>
		/// Uses a factory method to instantiate the component.
		/// </summary>
		/// <typeparam name="TImpl">Implementation type</typeparam>
		/// <param name="factoryMethod">Factory method</param>
		/// <returns></returns>
		public ComponentRegistration<TService> UsingFactoryMethod<TImpl>(Func<IKernel, CreationContext, TImpl> factoryMethod) where TImpl : TService
		{
			return UsingFactoryMethod((k, m, c) => factoryMethod(k, c));
		}

		internal void AddParameter(IKernel kernel, ComponentModel model, String key, String value)
		{
			var parameters = EnsureParametersConfiguration(kernel);
			var parameter = new MutableConfiguration(key, value);
			parameters.Children.Add(parameter);
			model.Parameters.Add(key, value);
		}

		internal void AddParameter(IKernel kernel, ComponentModel model, String key, IConfiguration value)
		{
			var parameters = EnsureParametersConfiguration(kernel);
			var parameter = new MutableConfiguration(key);
			parameter.Children.Add(value);
			parameters.Children.Add(parameter);
			model.Parameters.Add(key, value);
		}

		private IConfiguration EnsureComponentConfiguration(IKernel kernel)
		{
			var configuration = kernel.ConfigurationStore.GetComponentConfiguration(name);
			if (configuration == null)
			{
				configuration = new MutableConfiguration("component");
				kernel.ConfigurationStore.AddComponentConfiguration(name, configuration);
			}
			return configuration;
		}

		private IConfiguration EnsureParametersConfiguration(IKernel kernel)
		{
			var configuration = EnsureComponentConfiguration(kernel);
			var parameters = configuration.Children["parameters"];
			if (parameters == null)
			{
				parameters = new MutableConfiguration("parameters");
				configuration.Children.Add(parameters);
			}
			return parameters;
		}

		private bool ExecuteIfCondition(IKernel kernel)
		{
			if (ifFilter == null)
			{
				return true;
			}
			foreach (ComponentFilter filter in ifFilter.GetInvocationList())
			{
				if (filter(kernel, componentModel) == false)
				{
					return false;
				}
			}
			return true;
		}

		private bool ExecuteUnlessCondition(IKernel kernel)
		{
			if (unlessFilter == null)
			{
				return false;
			}
			foreach (ComponentFilter filter in unlessFilter.GetInvocationList())
			{
				if (filter(kernel, componentModel))
				{
					return true;
				}
			}
			return false;
		}

		private IKernelInternal GetInternalKernel(IKernel kernel)
		{
			var internalKernel = kernel as IKernelInternal;
			if (internalKernel == null)
			{
				throw new ArgumentException(
					string.Format("The kernel does not implement {0}.", typeof(IKernelInternal)), "kernel");
			}
			return internalKernel;
		}

		private void InitializeDefaults()
		{
			if (implementation == null)
			{
				implementation = serviceType;
			}

			if (String.IsNullOrEmpty(name))
			{
				
				if(implementation == typeof(LateBoundComponent))
				{
					name = "Late bound " + serviceType.FullName;
				}
				else
				{
					name = implementation.FullName;
				}
			}
		}

		private bool IsMatch(IKernel kernel)
		{
			return ExecuteIfCondition(kernel) && !ExecuteUnlessCondition(kernel);
		}

		/// <summary>
		/// Registers this component with the <see cref="IKernel"/>.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		void IRegistration.Register(IKernel kernel)
		{
			if (!registered)
			{
				registered = true;
				InitializeDefaults();

				var configuration = EnsureComponentConfiguration(kernel);

				foreach (var descriptor in descriptors)
				{
					descriptor.ApplyToConfiguration(kernel, configuration);
				}

				if (componentModel == null)
				{
					componentModel = kernel.ComponentModelBuilder.BuildModel(name, serviceType, implementation, null);
				}

				foreach (var descriptor in descriptors)
				{
					descriptor.ApplyToModel(kernel, componentModel);
				}

				if (componentModel.Implementation.IsInterface && componentModel.Interceptors.Count > 0)
				{
					var options = ProxyUtil.ObtainProxyOptions(componentModel, true);
					options.OmitTarget = true;
				}

				if (IsMatch(kernel))
				{
					var internalKernel = GetInternalKernel(kernel);
					internalKernel.AddCustomComponent(componentModel);
					if (forwardedTypes.Count > 0)
					{
						foreach (var type in forwardedTypes)
						{
							internalKernel.RegisterHandlerForwarding(type, name);
						}
					}

					foreach (var r in additionalRegistrations)
					{
						r.Register(kernel);
					}
				}
			}
		}
	}
}