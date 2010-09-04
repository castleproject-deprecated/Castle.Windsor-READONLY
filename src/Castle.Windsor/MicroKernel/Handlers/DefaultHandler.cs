// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.MicroKernel.Handlers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Castle.Core;
	using Castle.MicroKernel.Context;

	/// <summary>
	/// Summary description for DefaultHandler.
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public class DefaultHandler : AbstractHandler
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultHandler"/> class.
		/// </summary>
		/// <param name="model"></param>
		public DefaultHandler(ComponentModel model) : base(model)
		{
		}

		/// <summary>
		/// Returns an instance of the component this handler
		/// is responsible for
		/// </summary>
		/// <param name="context"></param>
		/// <param name="requiresDecommission"></param>
		/// <param name="instanceRequired"></param>
		/// <returns></returns>
		protected override object ResolveCore(CreationContext context, bool requiresDecommission, bool instanceRequired)
		{
			if (CanResolvePendingDependencies(context) == false)
			{
				if (instanceRequired == false)
				{
					return null;
				}

				AssertNotWaitingForDependency();
			}
			using (var resolutionContext = context.EnterResolutionContext(this))
			{
				var instance = lifestyleManager.Resolve(context);

				resolutionContext.Burden.SetRootInstance(instance, this, HasDecomission(requiresDecommission));
				context.ReleasePolicy.Track(instance, resolutionContext.Burden);

				return instance;
			}
		}

		private bool HasDecomission(bool track)
		{
			return track || ComponentModel.Lifecycle.HasDecommissionConcerns;
		}

		private bool CanResolvePendingDependencies(CreationContext context)
		{
			if (CurrentState == HandlerState.Valid)
			{
				return true;
			}
			// detect circular dependencies
			if (IsBeingResolvedInContext(context))
			{
				return context.HasAdditionalParameters;
			}
			var canResolveAll = true;
			foreach (var dependency in DependenciesByService.Values.ToArray())
			{
				
				// a self-dependency is not allowed
				var handler = Kernel.GetHandler(dependency.TargetItemType);
				if (handler == this)
				{
					canResolveAll = false;
					continue;
				}
				if(handler == null)
				{
					// ask the kernel
					if (Kernel.LazyLoadComponentByType(dependency.DependencyKey, dependency.TargetItemType, context.AdditionalParameters) == false)
					{
						canResolveAll = false;
						continue;
					}
				}
			}
			return (canResolveAll && DependenciesByKey.Count == 0) || context.HasAdditionalParameters;
		}

		/// <summary>
		/// disposes the component instance (or recycle it)
		/// </summary>
		/// <param name="instance"></param>
		/// <returns>true if destroyed</returns>
		public override bool ReleaseCore(object instance)
		{
			return lifestyleManager.Release(instance);
		}

		protected void AssertNotWaitingForDependency()
		{
			if (CurrentState == HandlerState.WaitingDependency)
			{
				String message = String.Format("Can't create component '{1}' " +
					"as it has dependencies to be satisfied. {0}",
					ObtainDependencyDetails(new List<object>()), ComponentModel.Name);

				throw new HandlerException(message);
			}
		}
	}
}
