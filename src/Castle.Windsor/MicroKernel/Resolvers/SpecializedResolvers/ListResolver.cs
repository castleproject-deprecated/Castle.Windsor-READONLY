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

namespace Castle.MicroKernel.Resolvers.SpecializedResolvers
{
	using System.Collections.Generic;
	using System.Linq;

	using Castle.Core;
	using Castle.Core.Internal;
	using Castle.MicroKernel.Context;

	/// <summary>
	/// Handle dependencies of services in the format of generic IList.
	/// </summary>
	/// <remarks>
	/// This is a complimentary <see cref="ISubDependencyResolver"/> implementation 
	/// that is capable of satisfying dependencies of services generic IList.
	/// <para>
	/// Note that it will take precedence over service override for lists defined 
	/// on the configuration.
	/// </para>
	/// </remarks>
	/// <example>
	/// In order to install the resolver:
	/// <code>
	/// var kernel = new DefaultKernel();
	/// kernel.Resolver.AddSubResolver(new ListResolver(kernel));
	/// </code>
	/// 
	/// <para>
	/// To use it, assuming that IService is on the container:
	/// </para>
	/// 
	/// <code>
	/// public class Component
	/// {
	///     public Component(IList&lt;IService&gt; services)
	///     {
	///     }
	/// }
	/// </code>
	/// </example>
	public class ListResolver : ISubDependencyResolver
	{
		private readonly IKernel kernel;
		private readonly bool allowEmptyList;

		public ListResolver(IKernel kernel)
			: this(kernel, false)
		{
		}

		public ListResolver(IKernel kernel, bool allowEmptyList)
		{
			this.kernel = kernel;
			this.allowEmptyList = allowEmptyList;
		}

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver,
							  ComponentModel model,
							  DependencyModel dependency)
		{
			var targetType = dependency.TargetItemType;
			var elementType = targetType.GetGenericArguments().Single();

			var items = kernel.ResolveAll(elementType, null);

			var listType = typeof(List<>).MakeGenericType(elementType);
			var list = listType.CreateInstance<object>(items);

			return list;
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver,
							   ComponentModel model,
							   DependencyModel dependency)
		{
			var targetType = dependency.TargetItemType;
			if (targetType == null || !targetType.IsGenericType || targetType.GetGenericTypeDefinition() != typeof(IList<>))
			{
				return false;
			}

			var elementType = targetType.GetGenericArguments().Single();
			return allowEmptyList || kernel.HasComponent(elementType);
		}
	}
}